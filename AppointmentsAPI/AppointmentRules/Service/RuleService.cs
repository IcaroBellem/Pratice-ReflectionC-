using AppointmentRules.Data;
using AppointmentRules.Models;
using AppointmentRules.Service.DTOs;
using AppointmentRules.Service.Interface;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Reflection;


namespace AppointmentRules.Service
{
    public class RuleService : IRuleService
    {
        private readonly AppDbContext _context;
        private static readonly ConcurrentDictionary<string, Func<TimeEntry, bool>> _compiledRulesCache
        = new ConcurrentDictionary<string, Func<TimeEntry, bool>>();

        public RuleService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> MakeTimeEntryAsync(TimeEntryDTO timeEntryDTO)
        {
            var timeEntry = new TimeEntry
            {
                MemberId = timeEntryDTO.MemberId,
                StartTime = timeEntryDTO.Entry,
                EndTime = timeEntryDTO.Exit,
                IsApproved = false

            };

            _context.TimeEntries.Add(timeEntry);
            await _context.SaveChangesAsync();
            await ApplyRulesAsync(timeEntry);

            return true;
        }
        public async Task<List<TimeEntryDTO>> GetTimeEntriesByIdAsync(int memberId)
        {
            var timeEntry = await _context.TimeEntries
                   .AsNoTracking()
                   .Where(t => t.MemberId == memberId)
                   .Select(t => new TimeEntryDTO
                   {
                       MemberId = t.MemberId,
                       Entry = t.StartTime,
                       Exit = t.EndTime,
                   })
                   .ToListAsync();

            return timeEntry;
        }
        public async Task ApplyRulesAsync(TimeEntry timeEntry)
        {
            var rules = await File.ReadAllTextAsync("Rules/rules.txt");

            foreach (var rule in rules.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var approved = CompileAndExecuteRule(rule.Trim(), timeEntry);
                if (approved.HasValue)
                {
                    timeEntry.IsApproved = approved.Value;
                    break;
                }

            }
            await _context.SaveChangesAsync();
           
        }
        public bool? CompileAndExecuteRule(string rule, TimeEntry timeEntry)
        {
            if (!_compiledRulesCache.TryGetValue(rule, out var compiledRule))
            {
                string code = $@"
                    using System;
                    using ProjetoMarcacaoPonto.Models;

                    public class RuleEvaluator
                    {{
                        public bool Evaluate(Marcacao marcacao)
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
                    MetadataReference.CreateFromFile(typeof(TimeEntry).Assembly.Location)
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

                    compiledRule = (Func<TimeEntry, bool>)Delegate.CreateDelegate(typeof(Func<TimeEntry, bool>), evaluator, method);
                    _compiledRulesCache.TryAdd(rule, compiledRule);
                }
            }
            return compiledRule(timeEntry);
        }
    }
} 
