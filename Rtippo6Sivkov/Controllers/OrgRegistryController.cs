using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

    [HttpPost("Add")]
    [ValidateAntiForgeryToken]
    public IActionResult PostAdd([FromForm] AddOrganizationViewModel addOrganizationViewModel)
    {
        if (!ModelState.IsValid)
        {
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

        return RedirectToAction("Index");
    }

    [HttpGet("Search")]
    public IActionResult GetSearch()
    {
        return View("Search");
    }

    [HttpPost("Search")]
    [ValidateAntiForgeryToken]
    public IActionResult PostSearch([FromForm] SearchOrganizationsViewModel searchOrganizationsViewModel)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("GetSearch");
        }

        var locality = _dbContext.Localities
            .SingleOrDefault(x => x.Id == (searchOrganizationsViewModel.LocalityId ?? -1));
        var type = _dbContext.OrganizationsTypes
            .SingleOrDefault(x => x.Id == (searchOrganizationsViewModel.TypeId ?? -1));

        searchOrganizationsViewModel.Locality = locality;
        searchOrganizationsViewModel.Type = type;

        TempData["Filter"] = JsonConvert.SerializeObject(searchOrganizationsViewModel);

        return RedirectToAction("Index");
    }

    [HttpGet("ClearSearchFilter")]
    public IActionResult ClearSearchFilter()
    {
        TempData["Filter"] = null;
        return RedirectToAction("Index");
    }

    [HttpGet("Sort")]
    public IActionResult Sort([FromQuery] string sort)
    {
        TempData["Sort"] = sort;
        return RedirectToAction("Index");
    }

    [HttpGet("Delete")]
    public IActionResult Delete([FromQuery] int id)
    {
        var entity = _dbContext.Organizations.Find(id);
        if (entity != null)
        {
            _dbContext.Organizations.Remove(entity);
            _dbContext.SaveChanges();
        }
        return RedirectToAction("Index");
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

public class SearchOrganizationsViewModel
{
    public string? Name { get; set; }

    public int? Inn { get; set; }

    public int? Kpp { get; set; }

    public string? Address { get; set; }

    public bool? IsPhysical { get; set; }

    public int? TypeId { get; set; }
    public int? LocalityId { get; set; }

    [BindNever, JsonIgnore] public OrganizationType? Type { get; set; }

    [BindNever, JsonIgnore] public Locality? Locality { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        void AppendIfNotNull<T>(T? value, string label)
        {
            if (value == null) return;
            var valueString = value is bool b
                ? b
                    ? "да"
                    : "нет"
                : value is string s
                    ? $"\"{s}\""
                    : value.ToString()!;
            if (sb.Length == 0)
            {
                sb.Append($"{label}: {valueString}");
            }
            else
            {
                sb.Append($" и {label}: {valueString}");
            }
        }

        AppendIfNotNull(Name, "Наименование организации включает в себя");
        AppendIfNotNull(Inn, "ИНН");
        AppendIfNotNull(Kpp, "КПП");
        AppendIfNotNull(Address, "Адрес регистрации");
        AppendIfNotNull(IsPhysical, "Физическое лицо");
        AppendIfNotNull(Type?.Name, "Тип");
        AppendIfNotNull(Locality?.Name, "Населенный пункт");

        return sb.Length == 0 ? "все записи" : sb.ToString();
    }
}