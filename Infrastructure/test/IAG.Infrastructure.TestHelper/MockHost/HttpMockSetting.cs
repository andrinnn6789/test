//
// inspired by https://github.com/JasonRowe/KestrelMock
//

using JetBrains.Annotations;

namespace IAG.Infrastructure.TestHelper.MockHost;

[UsedImplicitly]
public class HttpMockSetting
{
	public Request Request { get; set; }

	public Response Response { get; set; }
}