using System;
using System.Linq;

using IAG.Infrastructure.Exception.HttpException;
using IAG.ProcessEngine.DataLayer.State.Context;
using IAG.ProcessEngine.DataLayer.State.ObjectMapper;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;

using Microsoft.EntityFrameworkCore;

namespace IAG.ProcessEngine.Store;

public class JobStoreDb : IJobStore
{
    private readonly JobStateToDbMapper _toDbMapper;

    private readonly JobStateFromDbMapper _fromDbMapper;

    private readonly ProcessEngineStateDbContext _context;

    public JobStoreDb(ProcessEngineStateDbContext context)
    {
        _context = context;
        _toDbMapper = new JobStateToDbMapper();
        _fromDbMapper = new JobStateFromDbMapper();
    }

    public void Delete(Guid id)
    {
        var item = _context
            .JobStateEntries
            .FirstOrDefault(t => t.Id == id);
        if (item == null)
        {
            throw new NotFoundException(id.ToString());
        }
            
        _context.JobStateEntries.Remove(item);
        _context.SaveChanges();
    }

    public void DeleteOldJobs(int archiveDays, int errorDays)
    {
        var dateToDelete = DateTime.Today.AddDays(-archiveDays);
        _context.JobStateEntries.RemoveRange(_context
            .JobStateEntries
            .Where(x => (x.DateRunEnd < dateToDelete) & (x.ExecutionState == JobExecutionStateEnum.Success)));
        var dateToDeleteAll = DateTime.Today.AddDays(-errorDays);
        _context.JobStateEntries.RemoveRange(_context
            .JobStateEntries
            .Where(x => x.DateRunEnd < dateToDeleteAll));
        _context.SaveChanges();
    }

    public void DeleteScheduledJobs()
    {
        _context
            .JobStateEntries
            .RemoveRange(_context
                .JobStateEntries
                .Where(x => x.ExecutionState == JobExecutionStateEnum.Scheduled));
        _context.SaveChanges();
    }

    public IJobState Get(Guid id)
    {
        var item = _context
            .JobStateEntries
            .AsNoTracking()
            .Include(p => p.ParentJob)
            .Include(c => c.ChildJobs)
            .FirstOrDefault(t => t.Id == id);
        if (item == null)
        {
            throw new NotFoundException(id.ToString());
        }

        return _fromDbMapper.NewDestination(item);
    }

    public IQueryable<IJobState> GetJobs()
    {
        var jobs = _context.JobStateEntries
            .OrderByDescending(o => o.DateDue).ThenByDescending(o => o.DateRunStart)
            .Take(1000)     // limit, filters from call from controller are applied in-memory
            .AsNoTracking()
            .Include(p => p.ParentJob)
            .Include(c => c.ChildJobs).ToList().AsQueryable();

        return jobs.Select(j => (IJobState)_fromDbMapper.NewDestination(j));
    }

    public int GetJobCount(JobExecutionStateEnum[] executionStateFilter = null)
    {
        var jobs = _context.JobStateEntries.AsNoTracking();
        if (executionStateFilter != null && executionStateFilter.Length > 0)
        {
            jobs = jobs.Where(j => executionStateFilter.Contains(j.ExecutionState));
        }
            
        return jobs.Count();
    }

    public void Insert(IJobState job)
    {
        if (_context.JobStateEntries.Any(t => t.Id == job.Id))
        {
            throw new DuplicateKeyException(job.Id.ToString());
        }
            
        _context.JobStateEntries.Add(_toDbMapper.NewDestination(job));
        _context.SaveChanges();
    }

    public void Update(IJobState job)
    {
        var item = _context.JobStateEntries.FirstOrDefault(t => t.Id == job.Id);
        if (item == null)
        {
            throw new NotFoundException(job.Id.ToString());
        }
            
        _context.JobStateEntries.Update(_toDbMapper.UpdateDestination(item, job));
        _context.SaveChanges();
    }

    public void Upsert(IJobState job)
    {
        var item = _context.JobStateEntries.FirstOrDefault(t => t.Id == job.Id);
        if (item == null)
            _context.JobStateEntries.Add(_toDbMapper.NewDestination(job));
        else
            _context.JobStateEntries.Update(_toDbMapper.UpdateDestination(item, job));
        _context.SaveChanges();
    }
}