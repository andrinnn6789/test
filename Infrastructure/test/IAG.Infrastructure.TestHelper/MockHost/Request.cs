//
// inspired by https://github.com/JasonRowe/KestrelMock
//

using System.Collections.Generic;

using JetBrains.Annotations;

namespace IAG.Infrastructure.TestHelper.MockHost;

[UsedImplicitly]
public class Request
{
	public List<string> Methods { get; set; }

	public string Path { get; set; }

	public string PathStartsWith { get; set; }

	public string BodyContains { get; set; }

	public string BodyDoesNotContain { get; set; }
}