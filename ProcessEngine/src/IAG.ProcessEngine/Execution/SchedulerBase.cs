using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using IAG.ProcessEngine.Execution.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IAG.ProcessEngine.Execution;

public delegate void JobBeforeEnqueueEventHandler(IJobInstance jobInstance);

public abstract class SchedulerBase
{
    private const int CheckJobsAheadSeconds = 60;
    private const int WaitAfterExceptionSeconds = 10;

    private readonly ManualResetEvent _stopSignal;
    private Thread _schedulingThread;

    protected SchedulerBase(IServiceProvider serviceProvider, ILogger logger)
    {
        ServiceProvider = serviceProvider;
        _stopSignal = new ManualResetEvent(true);
        Logger = logger;
    }

    public event JobBeforeEnqueueEventHandler OnBeforeEnqueueJob;

    public bool IsRunning => !_stopSignal.WaitOne(0);

    protected IServiceProvider ServiceProvider { get; }

    protected ILogger Logger { get; }

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        _stopSignal.Reset();
        _schedulingThread = new Thread(CheckForJobsToRun);

        _schedulingThread.Start();
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        _stopSignal.Set();
        _schedulingThread.Join();
    }

    protected abstract SortedDictionary<DateTime, IJobInstance> GetNextExecutionTimes(DateTime lastCheckTimeUtc, int secondsAhead);

    private void CheckForJobsToRun()
    {
        var nextChecktimeUtc = DateTime.UtcNow;
        var upcomingJobs = new SortedDictionary<DateTime, IJobInstance>();

        while (IsRunning)
        {
            if (nextChecktimeUtc <= DateTime.UtcNow)
            {
                try
                {
                    upcomingJobs = GetNextExecutionTimes(nextChecktimeUtc, CheckJobsAheadSeconds);
                }
                catch (System.Exception ex)
                {
                    Logger.LogError("Failed to get next job to run: " + ex.Message);   
                    Logger.LogDebug(ex.StackTrace);

                    nextChecktimeUtc = DateTime.UtcNow.AddSeconds(WaitAfterExceptionSeconds);
                    continue;
                }

                nextChecktimeUtc = DateTime.UtcNow.AddSeconds(CheckJobsAheadSeconds);
            }

            List<DateTime> jobsToRemove = new List<DateTime>();
            foreach (var timeToRun in upcomingJobs.Keys.TakeWhile(timeToRun => timeToRun <= DateTime.UtcNow))
            {
                jobsToRemove.Add(timeToRun);
                var jobInstance = upcomingJobs[timeToRun];
                try
                {
                    OnBeforeEnqueueJob?.Invoke(jobInstance);
                    var jobService = jobInstance.ServiceProvider.GetRequiredService<IJobService>();
                    jobService.EnqueueJob(jobInstance).ContinueWith(_ => jobInstance.Dispose());
                }
                catch (System.Exception ex)
                {
                    Logger.LogError("Failed to enqueue job: " + ex.Message);
                    Logger.LogDebug(ex.StackTrace);
                }
            }

            foreach (var time in jobsToRemove)
            {
                upcomingJobs.Remove(time);
            }

            var nextEventUtc = upcomingJobs.Any() ? upcomingJobs.Keys.First() : nextChecktimeUtc;
            var sleepTime = (int)(nextEventUtc - DateTime.UtcNow).TotalMilliseconds;
            sleepTime = sleepTime > 0 ? sleepTime : 0;

            _stopSignal.WaitOne(sleepTime);
        }
    }
}