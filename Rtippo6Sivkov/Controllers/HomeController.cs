using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rtippo6Sivkov.Data;
using Rtippo6Sivkov.Models;

namespace Rtippo6Sivkov.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext dbContext,
        UserManager<AppUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult MyAccount()
    {
        var currentUser = _dbContext
            .AppUsers
            .Include(x => x.Locality)
            .Include(x => x.Role)
            .Single(x => x.Id == _userManager.GetUserId(User));
        return View(currentUser);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> CreateAdminUser(
        [FromServices] UserManager<AppUser> userManager,
        [FromServices] ApplicationDbContext dbContext)
    {
        _logger.LogInformation("Creating Admin user with password '__@Dm1n__'");
        var user = new AppUser("Admin")
        {
            Email = "admin@localhost.com"
        };
        var userToDelete = dbContext.Users.Single(x => x.UserName == "Admin");
        await userManager.DeleteAsync(userToDelete);
        var result = await userManager.CreateAsync(user, "__@Dm1n__");
        
        if (result.Succeeded)
        {
            return Ok(user);
        }

        return Conflict(new { result.Errors });
    }
}