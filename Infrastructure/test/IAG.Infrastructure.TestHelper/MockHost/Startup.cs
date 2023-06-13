//
// inspired by https://github.com/JasonRowe/KestrelMock
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.TestHelper.MockHost;

public class Startup
{
	private readonly IConfiguration _config;
	private readonly Dictionary<string, HttpMockSetting> _pathMappings;
	private readonly Dictionary<string, HttpMockSetting> _pathStartsWithMappings;
	private readonly Dictionary<string, List<HttpMockSetting>> _bodyCheckMappings;
	private readonly List<Assembly> _resourceAssemblies;

	public Startup(IConfiguration config)
	{
		_config = config;
		_pathMappings = new Dictionary<string, HttpMockSetting>();
		_pathStartsWithMappings = new Dictionary<string, HttpMockSetting>();
		_bodyCheckMappings = new Dictionary<string, List<HttpMockSetting>>();
		_resourceAssemblies = new PluginLoader().GetAssemblies().ToList();
	}

	public void Configure(IApplicationBuilder app, IHostEnvironment env)
	{
		SetupPathMappings();
		app.Run(async context =>
		{
			var path = (context.Request.Path + context.Request.QueryString.ToString())
				.TrimEnd('/')
				.ToLowerInvariant();
				
			using var reader = new StreamReader(context.Request.Body);
			string body = await reader.ReadToEndAsync();

			var matchResult = FindMatches(BuildPath(context.Request.Method, path), body);

			if (matchResult != null)
			{
				context.Response.StatusCode = matchResult.Status;
				if (matchResult.Headers != null)
				{
					foreach (var header in matchResult.Headers)
					{
						foreach (var key in header.Keys)
						{
							context.Response.Headers.Add(key, header[key]);
						}
					}
				}

				var responseBody = await GetBodyAsync(matchResult);
				await context.Response.WriteAsync(responseBody);
			}
			else
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
		});
	}

	private async Task<string> GetBodyAsync(Response response)
	{
		if (response.BodyText != null)
		{
			return response.BodyText;
		}

		if (response.BodyJson == null)
		{
			response.BodyText = string.Empty;
		}
		else if (response.BodyJson.Type != JTokenType.String)
		{
			response.BodyText = response.BodyJson.ToString(Formatting.Indented);
		}
		else
		{
			var filePath = response.BodyJson.Value<string>();
			if (string.IsNullOrEmpty(filePath))
			{
				response.BodyText = filePath ?? string.Empty;
			}
			else
			{
				try
				{
					response.BodyText = await File.ReadAllTextAsync(filePath);

				}
				catch (FileNotFoundException)
				{
					try
					{
						response.BodyText = await GetBodyFromResourceAsync(filePath);
					}
					catch (FileNotFoundException)
					{
						response.BodyText = filePath;
					}
				}
			}
		}

		return response.BodyText;
	}

	private Task<string> GetBodyFromResourceAsync(string resourcePath)
	{
		foreach (var assembly in _resourceAssemblies)
		{
			var resource = assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(resourcePath));
			if (resource != null)
			{
				using var stream = assembly.GetManifestResourceStream(resource);
				if (stream != null)
				{
					using var streamReader = new StreamReader(stream);
					return streamReader.ReadToEndAsync();
				}
			}
		}
		throw new FileNotFoundException();
	}

	private Response FindMatches(string path, string body)
	{
		if (_pathMappings.ContainsKey(path))
		{
			return _pathMappings[path].Response;
		}

		if (_pathStartsWithMappings != null)
		{
			foreach (var pathStart in _pathStartsWithMappings)
			{
				if (path.StartsWith(pathStart.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					return pathStart.Value.Response;
				}
			}
		}

		if (_bodyCheckMappings?.ContainsKey(path) == true)
		{
			foreach (var possibleResult in _bodyCheckMappings[path])
			{
				if (!string.IsNullOrEmpty(possibleResult.Request.BodyContains))
				{
					if (body.Contains(possibleResult.Request.BodyContains))
					{
						return possibleResult.Response;
					}
				}
				else if (!string.IsNullOrEmpty(possibleResult.Request.BodyDoesNotContain))
				{
					if (!body.Contains(possibleResult.Request.BodyDoesNotContain))
					{
						return possibleResult.Response;
					}
				}
			}
		}

		return null;
	}

	private void SetupPathMappings()
	{
		var mockSettingsConfigSection = _config.GetSection(KestrelMock.MockConfigSection);
		foreach (var childSetting in mockSettingsConfigSection.GetChildren())
		{
			var httpMockSetting = childSetting.Get<HttpMockSetting>();

			var body = childSetting.GetSection("Response:Body");
			httpMockSetting.Response.BodyJson = body?.GetConfigAsJson();

			if (!string.IsNullOrEmpty(httpMockSetting.Request.Path))
			{
				var path = httpMockSetting.Request.Path;
				if (!string.IsNullOrEmpty(httpMockSetting.Request.BodyContains) || !string.IsNullOrEmpty(httpMockSetting.Request.BodyDoesNotContain))
				{
					foreach (var method in httpMockSetting.Request.Methods)
					{
						var pathWithMethod = BuildPath(method, path);
						if (_bodyCheckMappings.ContainsKey(pathWithMethod))
						{
							_bodyCheckMappings[pathWithMethod].Add(httpMockSetting);
						}
						else
						{
							_bodyCheckMappings.TryAdd(pathWithMethod, new List<HttpMockSetting> {httpMockSetting});
						}
					}
				}
				else
				{
					foreach (var method in httpMockSetting.Request.Methods)
					{
						_pathMappings.TryAdd(BuildPath(method, path), httpMockSetting);
					}
				}
			}
			else if (!string.IsNullOrEmpty(httpMockSetting.Request.PathStartsWith))
			{
				var path = httpMockSetting.Request.PathStartsWith;
				foreach (var method in httpMockSetting.Request.Methods)
				{
					_pathStartsWithMappings.TryAdd(BuildPath(method, path), httpMockSetting);
				}
			}
		}
	}

	private string BuildPath(string method, string path)
	{
		return $"{method.ToUpperInvariant()}:{path.ToLowerInvariant()}";
	}
}