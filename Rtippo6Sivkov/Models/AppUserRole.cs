namespace Rtippo6Sivkov.Models;

public class AppUserRole
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public List<AppUser> UsersWithRole { get; set; } = null!;
}