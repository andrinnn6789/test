using System.Collections.Generic;
using System.IO;

namespace IAG.Infrastructure.Pdf;

public interface IPdfMerger
{
    public void MergePdfs(IEnumerable<Stream> inputPfdStreams, Stream outputPdfStream);
}