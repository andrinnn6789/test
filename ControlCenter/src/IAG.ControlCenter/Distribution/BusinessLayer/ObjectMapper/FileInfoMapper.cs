using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.ControlCenter.Distribution.BusinessLayer.ObjectMapper;

public class FileInfoMapper : ObjectMapper<FileStore, FileMetaInfo>
{
    protected override FileMetaInfo MapToDestination(FileStore source, FileMetaInfo destination)
    {
        destination.Id = source.Id;
        destination.Name = source.Name;
        destination.FileVersion = source.FileVersion;
        destination.ProductVersion = source.ProductVersion;
        destination.Checksum = source.Checksum;
        destination.FileLastModifiedDate = source.FileLastModifiedDate;

        return destination;
    }
}