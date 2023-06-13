//
// inspired by https://github.com/JasonRowe/KestrelMock
//

using System.Collections.Generic;

using JetBrains.Annotations;

using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.TestHelper.MockHost;

[UsedImplicitly]
public class Response
{
	public int Status { get; set; }

	public List<Dictionary<string, string>> Headers { get; set; }

	internal JToken BodyJson { get; set; }

	internal string BodyText { get; set; }
}