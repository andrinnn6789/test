using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

using IAG.Infrastructure.Cron;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Models;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;

using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("JobDocs")]
    public class JobDocsController : Controller
    {
        private readonly IPluginLoader _pluginLoader;

        public JobDocsController(IPluginLoader pluginLoader)
        {
            _pluginLoader = pluginLoader;
        }
        
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            var markdownJobDocs = GetMarkdownJobDocs();
            return View(new JobDocsViewModel() { Docs = markdownJobDocs.ToList() });
        }

        [HttpGet]
        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<JobDoc> GetMarkdownJobDocs()
        {
            var docs = new List<JobDoc>();
            var jobConfigProvider = GetJobConfigProvider();
            var jobClasses = _pluginLoader.GetTypesWithAttribute(typeof(JobInfoAttribute));
            var jobConfigs = jobConfigProvider!.GetAll().ToList();
            
            foreach (var jobClass in jobClasses)
            {
                var content = GetMarkdownContent(jobClass);
                var attribute = (JobInfoAttribute)jobClass.GetCustomAttribute(typeof(JobInfoAttribute), inherit: true);
                var jobConfig = jobConfigs.FirstOrDefault(config => config.TemplateId == attribute?.TemplateId);
                var jobState = jobConfig?.Active ?? false;
                var cronExpression = jobConfig?.CronExpression ?? string.Empty;
                
                docs.Add(new JobDoc
                {
                    TemplateId = attribute?.TemplateId ?? Guid.Empty,
                    JobName = attribute?.Name ?? string.Empty,
                    IsJobActive = jobState,
                    JobSchedule = ParseCronExpression(cronExpression),
                    ContentAsMarkdown = content,
                    IsCustomerSpecific = jobClass.Assembly.FullName != null && 
                                         (jobClass.Assembly.FullName.StartsWith("IAG.VinX.") ||
                                         jobClass.Assembly.FullName.StartsWith("IAG.PerformX."))
                });
            }

            return docs;
        }

        private IJobConfigProvider GetJobConfigProvider()
        {
            var jobConfigProviders = _pluginLoader.GetImplementations<IJobConfigProvider>().ToList();

            return jobConfigProviders
                .Select(Activator.CreateInstance)
                .Select(instance => (IJobConfigProvider)instance)
                .FirstOrDefault();
        }
        
        [ExcludeFromCodeCoverage]
        private static string GetMarkdownContent(Type jobClass)
        {
            var markdownResource = jobClass.Assembly.GetManifestResourceNames()
                .FirstOrDefault(resource => resource.EndsWith($"{jobClass.Name}.md"));
            if (markdownResource == null) return "";

            using var stream = jobClass.Assembly.GetManifestResourceStream(markdownResource);
            using var reader = new StreamReader(stream!);
            var content = reader.ReadToEnd();
            return content;
        }
        
        private static string ParseCronExpression(string cronExpression)
        {
            return !string.IsNullOrEmpty(cronExpression) ? CronParser.GetHumanReadableFormat(cronExpression) : "nicht festgelegt";
        }
    }
}
