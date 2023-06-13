//
// inspired by https://github.com/JasonRowe/KestrelMock
//

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace IAG.Infrastructure.TestHelper.MockHost;

[UsedImplicitly]
public class KestrelMock
{
	private const string DefaultUrl = "http://localhost";
	private const string DefaultPort = "60000";

	public const string MockConfigSection = "MockSettings";

	private static int _nextFreePort = 60000;
	public static int NextFreePort => Interlocked.Increment(ref _nextFreePort);

	public static void Run(string configFile, int port)
	{
		var assembly = Assembly.GetCallingAssembly();
		using var stream = assembly.GetManifestResourceStream(configFile);
		var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
		Run(config, new List<string>
		{
			"http://localhost:" + port
		});
	}

	private static void Run(IConfiguration configuration, List<string> urls = null)
	{
		if (urls == null || !urls.Any())
		{
			urls = new List<string> { DefaultUrl + ":" + DefaultPort };
		}

		var mockSettingsConfigSectionExists = configuration.GetChildren().Any(x => x.Key == MockConfigSection);

		if(!mockSettingsConfigSectionExists)
		{
			throw new System.Exception("Configuration must include 'MockSettings' section");
		}

		Task.Run(() => CreateWebHostBuilder(urls, configuration).Build().RunAsync());
	}

	private static IWebHostBuilder CreateWebHostBuilder(List<string> urls, IConfiguration configuration) =>
		WebHost.CreateDefaultBuilder()
			.UseConfiguration(configuration)
			.UseUrls(urls.ToArray())
			.UseStartup<Startup>();
}