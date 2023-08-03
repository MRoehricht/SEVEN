using System.ComponentModel.DataAnnotations;

namespace SEVEN.Core.Models;

public class SevenUser
{
    [Key] public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl  { get; set; }
    public UserRoles? Roles { get; set; }
}