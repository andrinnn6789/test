using IAG.VinX.Zweifel.S1M.Dto.RequestModels;

namespace IAG.VinX.Zweifel.S1M.BusinessLogic;

public interface IS1MMediaWriter
{
    bool WriteMedia(UploadMediaRequestModel reqModel);
}