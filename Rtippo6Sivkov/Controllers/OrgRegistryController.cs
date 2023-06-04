using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rtippo6Sivkov.Data;
using Rtippo6Sivkov.Models;

namespace Rtippo6Sivkov.Controllers;

[Route("OrgRegistry")]
[Authorize]
public class OrgRegistryController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public OrgRegistryController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("Add")]
    public IActionResult GetAdd()
    {
        return View("Add");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PostAdd([FromForm] AddOrganizationViewModel addOrganizationViewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("X", "YZ");
            return RedirectToAction("GetAdd");
        }

        var locality = _dbContext.Localities.Single(x => x.Id == addOrganizationViewModel.LocalityId);
        var type = _dbContext.OrganizationsTypes.Single(x => x.Id == addOrganizationViewModel.TypeId);

        var org = new Organization
        {
            Name = addOrganizationViewModel.Name,
            Address = addOrganizationViewModel.Address,
            Inn = addOrganizationViewModel.Inn.ToString(),
            Kpp = addOrganizationViewModel.Kpp.ToString(),
            IsPhysical = addOrganizationViewModel.IsPhysical,
            Locality = locality,
            Type = type
        };

        _dbContext.Organizations.Add(org);
        _dbContext.SaveChanges();

        return View("Index");
    }
}

public class AddOrganizationViewModel
{
    [Required(ErrorMessage = "Введите наименование организации")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Введите ИНН организации")]
    [Range(1, int.MaxValue, ErrorMessage = "ИНН должен быть положительным числом")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "ИНН содержит 10 цифр")]
    public int Inn { get; set; }

    [Required(ErrorMessage = "Введите КПП организации")]
    [Range(1, int.MaxValue, ErrorMessage = "КПП должен быть положительным числом")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "КПП содержит 9 цифр")]
    public int Kpp { get; set; }

    [Required(ErrorMessage = "Введите адрес регистрации организации")]
    public string Address { get; set; }

    [Required] public bool IsPhysical { get; set; }

    [Required] public int TypeId { get; set; }
    [Required] public int LocalityId { get; set; }
}