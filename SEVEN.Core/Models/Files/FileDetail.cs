using System.ComponentModel.DataAnnotations;

namespace SEVEN.Core.Models.Files;

public class FileDetail
{
    [Key]
    public Guid Id { get; set; }
    public string? FileName { get; set; }
    public byte[]? FileData { get; set; }
    public FileType Type { get; set; }
}