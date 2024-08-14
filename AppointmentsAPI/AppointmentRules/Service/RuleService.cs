using AppointmentRules.Data;
using AppointmentRules.Models;
using AppointmentRules.Service.DTOs;
using AppointmentRules.Service.Interface;
using Azure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Reflection;

public class RuleService : IRuleService
{
    private readonly AppDbContext _context;
    private static readonly ConcurrentDictionary<string, Func<TimeEntry, bool>> _compiledRulesCache
        = new ConcurrentDictionary<string, Func<TimeEntry, bool>>();

    public RuleService(AppDbContext context)
    {
        _context = context;
    }
    private bool? CompileAndExecuteRule(string rule, TimeEntry timeEntryComplile)
    {
        if (!_compiledRulesCache.TryGetValue(rule, out var compiledRule))
        {
            string code = $@"
                using System;
                using appointmentrules.models;

                public class RuleEvaluator
                {{
                    public bool Evaluate(TimeEntry timeEntry)
                    {{
                        return {rule};
                    }}
                }}
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var assemblyName = Path.GetRandomFileName();
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(TimeEntryDTO).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);

                var assembly = Assembly.Load(ms.ToArray());
                var evaluatorType = assembly.GetType("RuleEvaluator");
                var evaluator = Activator.CreateInstance(evaluatorType);
                var method = evaluatorType.GetMethod("Evaluate");

                compiledRule = (Func<TimeEntry, bool>)Delegate.CreateDelegate(typeof(Func<TimeEntryDTO, bool>), evaluator, method);
                _compiledRulesCache.TryAdd(rule, compiledRule);
            }
        }
        return compiledRule(timeEntryComplile);
    }

    public async Task<List<TimeEntryDTO>> GetTimeEntriesByIdAsync(int memberId)
    {
        var timeEntries = await _context.TimeEntries
            .AsNoTracking()
            .Where(t => t.MemberId == memberId)
            .Select(t => new TimeEntryDTO
            {
                MemberId = t.MemberId,
                TaskId = t.TaskId,
                Entry = t.StartTime,
                Exit = t.EndTime,
                IsApproved = t.IsApproved,
                
            })
            .ToListAsync();

        return timeEntries;
    }

    public async Task<TimeEntryDTO> ApplyRulesAsync(TimeEntryDTO timeEntryDto)
    {
        var timeEntry = new TimeEntry
        {
            MemberId = timeEntryDto.MemberId,
            TaskId = timeEntryDto.TaskId,
            StartTime = timeEntryDto.Entry,
            EndTime = timeEntryDto.Exit,
            IsApproved = timeEntryDto.IsApproved,
        };

        var rules = await File.ReadAllTextAsync("rules/rules.txt");

        foreach (var ruleline in rules.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            var rule = ruleline.Trim(); 

            var result = CompileAndExecuteRule(rule, timeEntry);
            if (result.HasValue)
            {
                timeEntry.IsApproved = result.Value;

                if(timeEntry.IsApproved == null)
                {
                    timeEntryDto.ApprovalMessage = "Not Approved";
                }
                else
                {
                    timeEntryDto.ApprovalMessage = "Approved";
                }
                break;
            }
        }
        _context.TimeEntries.Update(timeEntry);
        await _context.SaveChangesAsync();

        return timeEntryDto;
    }

    public async Task<bool> MakeTimeEntryAsync(TimeEntryResponseDTO timeEntryResponseDto)
    {
        if (timeEntryResponseDto == null)
        {
            return false;
        }

        var timeEntry = new TimeEntry
        {
            MemberId = timeEntryResponseDto.MemberId,
            TaskId = timeEntryResponseDto.TaskId,
            StartTime = timeEntryResponseDto.Entry,
            EndTime = timeEntryResponseDto.Exit,
        };

        _context.TimeEntries.Add(timeEntry);

        var timeEntryDto = new TimeEntryDTO
        {
            MemberId = timeEntry.MemberId,
            TaskId = timeEntry.TaskId,
            Entry = timeEntry.StartTime,
            Exit = timeEntry.EndTime,
            IsApproved = timeEntry.IsApproved,
        };

        await ApplyRulesAsync(timeEntryDto);
        await _context.SaveChangesAsync();

        return true;
    }

}



