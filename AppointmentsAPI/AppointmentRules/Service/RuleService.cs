using AppointmentRules.Data;
using AppointmentRules.Models;
using AppointmentRules.Service.DTOs;
using AppointmentRules.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

public class RuleService : IRuleService
{
    private readonly AppDbContext _context;
    private static readonly ConcurrentDictionary<string, Func<TimeEntry, bool>> _compiledRulesCache
        = new ConcurrentDictionary<string, Func<TimeEntry, bool>>();
    private static readonly Lazy<string[]> _rules = new Lazy<string[]>(() => File.ReadAllLines("rules/rules.txt"));

    public RuleService(AppDbContext context)
    {
        _context = context;
    }

    private Func<TimeEntry, bool> BuildRuleFunction(string rule)
    {
        if (_compiledRulesCache.TryGetValue(rule, out var compiledRule))
        {
            return compiledRule;
        }

        var parameter = Expression.Parameter(typeof(TimeEntry), "timeEntry");

        var expression = DynamicExpressionParser.ParseLambda(new[] { parameter }, typeof(bool), rule);

        compiledRule = (Func<TimeEntry, bool>)expression.Compile();
        _compiledRulesCache.TryAdd(rule, compiledRule);

        return compiledRule;
    }

    public async Task<TimeEntryDTO> ApplyRulesAsync(TimeEntryDTO timeEntryDto)
    {
        var timeEntry = new TimeEntry
        {
            MemberId = timeEntryDto.MemberId,
            TaskId = timeEntryDto.TaskId,
            StartTime = timeEntryDto.Entry,
            EndTime = timeEntryDto.Exit,
            IsApproved = false
        };

        var rules = _rules.Value;
        bool isApproved = false;

        foreach (var rule in rules)
        {
            if (string.IsNullOrWhiteSpace(rule)) continue;

            var ruleFunction = BuildRuleFunction(rule.Trim());
            if (ruleFunction(timeEntry))
            {
                isApproved = true;
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
            IsApproved = false 
        };

        _context.TimeEntries.Add(timeEntry);

        var timeEntryDto = new TimeEntryDTO
        {
            MemberId = timeEntry.MemberId,
            TaskId = timeEntry.TaskId,
            Entry = timeEntry.StartTime,
            Exit = timeEntry.EndTime,
            IsApproved = timeEntry.IsApproved
        };

        await ApplyRulesAsync(timeEntryDto);

        return true;
    }
    public async Task<List<TimeEntryDTO>> GetTimeEntriesByIdAsync(int memberId)
    {
        return await _context.TimeEntries
            .AsNoTracking()
            .Where(t => t.MemberId == memberId)
            .Select(t => new TimeEntryDTO
            {
                MemberId = t.MemberId,
                TaskId = t.TaskId,
                Entry = t.StartTime,
                Exit = t.EndTime,
                IsApproved = t.IsApproved
            })
            .ToListAsync();
    }

}
