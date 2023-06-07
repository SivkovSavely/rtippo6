using Microsoft.AspNetCore.Identity;

namespace Rtippo6Sivkov.Models;

public class AppUser : IdentityUser
{
    public Locality Locality { get; set; } = null!;
    public AppUserRole Role { get; set; } = null!;
}