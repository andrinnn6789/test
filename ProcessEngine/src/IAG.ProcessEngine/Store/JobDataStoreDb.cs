using System;
using System.Linq;

using IAG.Infrastructure.ProcessEngine.JobData;
using IAG.ProcessEngine.DataLayer.State.Context;

using JetBrains.Annotations;

using Newtonsoft.Json;

using JobData = IAG.ProcessEngine.DataLayer.State.Model.JobData;

namespace IAG.ProcessEngine.Store;

public class JobDataStoreDb : IJobDataStore
{
    private readonly ProcessEngineStateDbContext _context;

    public JobDataStoreDb(ProcessEngineStateDbContext context)
    {
        _context = context;
    }

    public void Set<T>(IJobData jobData)
        where T : IJobData
    {
        var dataDb = _context.JobDataEntries.FirstOrDefault(t => t.Id == jobData.Id);
        if (dataDb == null)
        {
            dataDb = new JobData() {Id = jobData.Id};
            _context.JobDataEntries.Add(dataDb);
        }

        dataDb.Data = JsonConvert.SerializeObject(jobData, typeof(T), null);

        _context.SaveChanges();
    }

    public T Get<T>(Guid id)
        where T : IJobData, new()
    {
        T data;
        var dataDb = _context.JobDataEntries.FirstOrDefault(t => t.Id == id);
        if (dataDb == null)
        {
            data = new T() {Id = id};
            dataDb = new JobData() {Id = id, Data = JsonConvert.SerializeObject(data, typeof(T), null)};
            _context.JobDataEntries.Add(dataDb);
            _context.SaveChanges();
        }
        else
        {
            data = JsonConvert.DeserializeObject<T>(dataDb.Data);
        }

        return data;
    }

    public void Remove(Guid id)
    {
        var dataDb = _context.JobDataEntries.FirstOrDefault(t => t.Id == id);
        if (dataDb == null)
        {
            return;
        }

        _context.JobDataEntries.Remove(dataDb);
        _context.SaveChanges();
    }

    public void SetRaw(Guid id, [NotNull] string data)
    {
        var dataDb = _context.JobDataEntries.FirstOrDefault(t => t.Id == id);
        if (dataDb == null)
        {
            dataDb = new JobData() { Id = id };
            _context.JobDataEntries.Add(dataDb);
        }

        dataDb.Data = data;

        _context.SaveChanges();
    }

    public string GetRaw(Guid id)
    {
        var dataDb = _context.JobDataEntries.FirstOrDefault(t => t.Id == id);

        return dataDb?.Data;
    }
}