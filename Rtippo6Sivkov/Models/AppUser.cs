using Microsoft.AspNetCore.Identity;

namespace Rtippo6Sivkov.Models;

public class AppUser : IdentityUser
{
    public AppUserRole Role { get; set; } = null!;
}