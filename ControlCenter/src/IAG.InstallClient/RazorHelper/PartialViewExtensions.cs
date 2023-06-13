using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace IAG.InstallClient.RazorHelper;

[ExcludeFromCodeCoverage]
public static class PartialViewExtensions
{
    private const string PartialViewScriptKey = "_partial_view_script_";

    public static HtmlString PartialViewScript(this IHtmlHelper htmlHelper, Func<object, HelperResult> template)
    {
        htmlHelper.ViewContext.HttpContext.Items[PartialViewScriptKey + Guid.NewGuid()] = template;
        return HtmlString.Empty;
    }

    public static HtmlString RenderPartialViewScripts(this IHtmlHelper htmlHelper)
    {
        foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
        {
            if (key?.ToString()?.StartsWith(PartialViewScriptKey) == true)
            {
                if (htmlHelper.ViewContext.HttpContext.Items[key] is Func<object, HelperResult> template)
                {
                    htmlHelper.ViewContext.Writer.Write(template(null));
                }
            }
        }
        return HtmlString.Empty;
    }
}