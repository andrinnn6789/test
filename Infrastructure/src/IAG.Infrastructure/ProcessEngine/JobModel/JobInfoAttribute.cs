using System;
using System.Linq;
using System.Reflection;

namespace IAG.Infrastructure.ProcessEngine.JobModel;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class JobInfoAttribute : Attribute
{
    public Guid TemplateId { get; }

    public string Name { get; }

    public bool AutoActivate { get; }

    public JobInfoAttribute(string templateId, string name, bool autoActivate = false)
    {
        TemplateId = new Guid(templateId);
        Name = name;
        AutoActivate = autoActivate;
    }


    public static JobInfoAttribute GetJobInfo(Type type)
    {
        if (type.GetCustomAttributes(typeof(JobInfoAttribute)).FirstOrDefault() is not JobInfoAttribute jobInfoAttribute)
        {
            throw new System.Exception($"No job info attribute defined for {type}");
        }

        return jobInfoAttribute;
    }

    public static Guid GetTemplateId(Type type)
    {
        return GetJobInfo(type).TemplateId;
    }

    public static string GetName(Type type)
    {
        return GetJobInfo(type).Name;
    }

}