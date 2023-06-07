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

    public async Task<IActionResult> CreateUsers(
        [FromServices] UserManager<AppUser> userManager,
        [FromServices] ApplicationDbContext dbContext)
    {
        var guestCreationResult = await CreateUser("Guest", "__Gu3sT__", "Москва", "Гость");
        if (!guestCreationResult.Succeeded)
        {
            return Conflict(new { guestCreationResult.Errors });
        }

        var adminCreationResult = await CreateUser("Admin", "__@Dm1n__", "Москва", "Администратор");
        if (!adminCreationResult.Succeeded)
        {
            return Conflict(new { adminCreationResult.Errors });
        }

        var vetCuratorCreationResult = await CreateUser("VetCurator", "__V3tCur@t0r__", "Тюмень", "Куратор Ветслужбы");
        if (!vetCuratorCreationResult.Succeeded)
        {
            return Conflict(new { vetCuratorCreationResult.Errors });
        }

        var omsuCuratorCreationResult = await CreateUser("OmsuCurator", "__0msUCur@t0r__", "Тюмень", "Куратор ОМСУ");
        if (!omsuCuratorCreationResult.Succeeded)
        {
            return Conflict(new { omsuCuratorCreationResult.Errors });
        }

        var vetOperatorCreationResult =
            await CreateUser("VetOperator", "__V3t0p3r@T0r__", "Тюмень", "Оператор Ветслужбы");
        if (!vetOperatorCreationResult.Succeeded)
        {
            return Conflict(new { vetOperatorCreationResult.Errors });
        }

        var omsuOperatorCreationResult = await CreateUser("OmsuOperator", "__0msU0p3r@T0r__", "Тюмень", "Оператор ОМСУ");
        if (!omsuOperatorCreationResult.Succeeded)
        {
            return Conflict(new { omsuOperatorCreationResult.Errors });
        }

        return Ok();

        async Task<IdentityResult> CreateUser(string username, string password, string localityName, string roleName)
        {
            _logger.LogInformation(
                "@Creating '{Username}' user with password '{Password}', locality {Locality} and role {RoleName}",
                username, password, localityName, roleName);

            var userToDelete = dbContext.Users.SingleOrDefault(x => x.UserName == username);
            if (userToDelete != null)
            {
                await userManager.DeleteAsync(userToDelete);
            }

            var localityEntity = _dbContext.Localities.SingleOrDefault(x => x.Name.ToLower() == localityName);
            var roleEntity = _dbContext.AppUserRoles.SingleOrDefault(x => x.Name.ToLower() == roleName);

            var userToCreate = new AppUser(username)
            {
                Email = $"{username}@localhost.com",
                Locality = localityEntity ?? throw new Exception($"Locality with name {localityName} not found"),
                Role = roleEntity ?? throw new Exception($"Role with name {roleName} not found")
            };
            var result = await userManager.CreateAsync(userToCreate, password);

            return result;
        }
    }
}