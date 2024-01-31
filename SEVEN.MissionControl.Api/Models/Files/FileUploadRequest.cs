using SEVEN.Core.Models.Files;

namespace SEVEN.MissionControl.Api.Models.Files;

public class FileUploadRequest
{
    public IFormFile? FormFile { get; set; }
    public FileType Type { get; set; }
}