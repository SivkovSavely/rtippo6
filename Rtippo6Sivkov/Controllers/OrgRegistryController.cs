using Microsoft.AspNetCore.Mvc;

namespace Rtippo6Sivkov.Controllers;

[Route("OrgRegistry")]
public class OrgRegistryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}