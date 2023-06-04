using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Rtippo6Sivkov.Controllers;

[Route("OrgRegistry")]
public class OrgRegistryController : Controller
{
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
        return Json(addOrganizationViewModel);
    }
}

public class AddOrganizationViewModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public int Inn { get; set; }
    [Required]
    public int Kpp { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public bool IsPhysical { get; set; }
    [Required]
    public int TypeId { get; set; }
    [Required]
    public int LocalityId { get; set; }
}