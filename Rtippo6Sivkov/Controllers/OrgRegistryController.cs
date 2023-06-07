using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<AppUser> _userManager;

    private async Task<AppUser> GetUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            throw new InvalidOperationException("User is null");
        return _dbContext
            .AppUsers
            .Include(x => x.Locality)
            .Include(x => x.Role)
            .Single(x => x.Id == user.Id);
    }

    public OrgRegistryController(
        ApplicationDbContext dbContext,
        UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("Add")]
    public async Task<IActionResult> GetAdd()
    {
        if ((await GetUser()).CanCreateOrEditRegistryEntries() != true)
        {
            return View("AccessDenied", new AccessDeniedViewModel("Доступ к добавлению записей в реестр запрещен"));
        }
        return View("Add");
    }

    [HttpPost("Add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostAdd([FromForm] AddOrganizationViewModel addOrganizationViewModel)
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
        
        if ((await GetUser()).CanCreateOrEditRegistryEntries(org.Locality) != true)
        {
            return View("AccessDenied", new AccessDeniedViewModel("Доступ к добавлению записей в реестр запрещен"));
        }

        _dbContext.Organizations.Add(org);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet("Search")]
    public IActionResult GetSearch()
    {
        return View("Search");
    }

    [HttpPost("Search")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostSearch([FromForm] SearchOrganizationsViewModel searchOrganizationsViewModel)
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

        HttpContext.Session.SetString("Filter", JsonConvert.SerializeObject(searchOrganizationsViewModel));
        await HttpContext.Session.CommitAsync();

        return RedirectToAction("Index");
    }

    [HttpGet("ClearSearchFilter")]
    public async Task<IActionResult> ClearSearchFilter()
    {
        HttpContext.Session.Remove("Filter");
        await HttpContext.Session.CommitAsync();
        return RedirectToAction("Index");
    }

    [HttpGet("Sort")]
    public async Task<IActionResult> Sort([FromQuery] string sort)
    {
        HttpContext.Session.SetString("Sort", sort);
        await HttpContext.Session.CommitAsync();
        return RedirectToAction("Index");
    }

    [HttpGet("Read")]
    public async Task<IActionResult> Read([FromQuery] int id)
    {
        var entity = _dbContext.Organizations
            .Include(x => x.Type)
            .Include(x => x.Locality)
            .SingleOrDefault(x => x.Id == id);

        if (entity != null)
        {
            if ((await GetUser()).CanReadRegistryEntries(entity.Locality) != true)
            {
                return View("AccessDenied", new AccessDeniedViewModel("Доступ к чтению данной записи запрещен"));
            }
            
            return View(entity);
        }

        return NotFound();
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

    [HttpGet("Edit")]
    public async Task<IActionResult> GetEdit([FromQuery] int id)
    {
        var entity = _dbContext.Organizations
            .Include(x => x.Type)
            .Include(x => x.Locality)
            .SingleOrDefault(x => x.Id == id);

        if (entity == null)
        {
            Console.WriteLine($"Entity with id {id} not found");
            return NotFound();
        }
        
        if ((await GetUser()).CanReadRegistryEntries(entity.Locality) != true)
        {
            return View("AccessDenied", new AccessDeniedViewModel("Доступ к изменению данной записи запрещен"));
        }

        var viewModel = new AddOrganizationViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Inn = long.Parse(entity.Inn),
            Kpp = long.Parse(entity.Kpp),
            Address = entity.Address,
            LocalityId = entity.Locality.Id,
            TypeId = entity.Type.Id,
            IsPhysical = entity.IsPhysical
        };

        return View("Edit", viewModel);
    }

    [HttpPost("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostEdit([FromQuery] int id, [FromForm] AddOrganizationViewModel addOrganizationViewModel)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Model state is invalid");
            return RedirectToAction("GetEdit");
        }

        var org = _dbContext.Organizations.SingleOrDefault(x => x.Id == id);

        if (org == null)
        {
            Console.WriteLine($"Entity with id {id} not found");
            return NotFound();
        }
        
        if ((await GetUser()).CanReadRegistryEntries(org.Locality) != true)
        {
            return View("AccessDenied", new AccessDeniedViewModel("Доступ к изменению данной записи запрещен"));
        }

        var locality = _dbContext.Localities.Single(x => x.Id == addOrganizationViewModel.LocalityId);
        var type = _dbContext.OrganizationsTypes.Single(x => x.Id == addOrganizationViewModel.TypeId);

        org.Name = addOrganizationViewModel.Name;
        org.Address = addOrganizationViewModel.Address;
        org.Inn = addOrganizationViewModel.Inn.ToString();
        org.Kpp = addOrganizationViewModel.Kpp.ToString();
        org.IsPhysical = addOrganizationViewModel.IsPhysical;
        org.Locality = locality;
        org.Type = type;

        _dbContext.Entry(org).State = EntityState.Modified;

        _dbContext.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpGet("Export")]
    public async Task<IActionResult> Export()
    {
        var sort = HttpContext.Session.GetString("Sort") ?? "default";
        var filterString = HttpContext.Session.GetString("Filter");
        var filter = filterString != null
            ? JsonConvert.DeserializeObject<SearchOrganizationsViewModel>(filterString)!
            : null;
        
        await using var textWriter = new StringWriter();
        await using var csvWriter = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ","
        });

        var org = new OrganizationFiltererSorter(_dbContext.Organizations, filter, sort, await GetUser());
        csvWriter.WriteField("id");
        csvWriter.WriteField("name");
        csvWriter.WriteField("type");
        csvWriter.WriteField("inn");
        csvWriter.WriteField("kpp");
        csvWriter.WriteField("locality");
        csvWriter.WriteField("address");
        csvWriter.WriteField("is_physical");
        await csvWriter.NextRecordAsync();

        foreach (var organization in org.Organizations)
        {
            csvWriter.WriteField(organization.Id);
            csvWriter.WriteField(organization.Name);
            csvWriter.WriteField(organization.Type.Name);
            csvWriter.WriteField(organization.Inn);
            csvWriter.WriteField(organization.Kpp);
            csvWriter.WriteField(organization.Locality.Name);
            csvWriter.WriteField(organization.Address);
            csvWriter.WriteField(organization.IsPhysical);
            await csvWriter.NextRecordAsync();
        }

        return File(Encoding.UTF8.GetBytes(textWriter.ToString()), "text/csv", DateTimeOffset.Now.ToString("s") + ".csv");
    }
}

public class OrganizationFiltererSorter
{
    public IEnumerable<Organization> Organizations { get; set; }

    public OrganizationFiltererSorter(
        DbSet<Organization> organizations,
        SearchOrganizationsViewModel? filter,
        string sort,
        AppUser user)
    {
        if (user.Role is null)
        {
            throw new InvalidOperationException("user.Role is null. Скорее всего забыли заIncludeить роль.");
        }
        var organizationsQueryable = Filter(organizations, filter);
        organizationsQueryable = Sort(organizationsQueryable, sort);

        Organizations = organizationsQueryable
            .AsEnumerable()
            .Where(org => user.CanReadRegistryEntries(org.Locality));
    }

    private static IQueryable<Organization> Filter(
        DbSet<Organization> organizations,
        SearchOrganizationsViewModel? filter)
    {
        IQueryable<Organization> organizationsQueryable = organizations
            .Include(x => x.Type)
            .Include(x => x.Locality);

        if (filter != null)
        {
            if (filter.Name != null)
            {
                organizationsQueryable = organizationsQueryable.Where(x => x.Name.Contains(filter.Name));
            }

            if (filter.Inn != null)
            {
                organizationsQueryable = organizationsQueryable.Where(x => x.Inn == filter.Inn.ToString());
            }

            if (filter.Kpp != null)
            {
                organizationsQueryable = organizationsQueryable.Where(x => x.Kpp == filter.Kpp.ToString());
            }

            if (filter.Address != null)
            {
                organizationsQueryable = organizationsQueryable.Where(x => x.Address == filter.Address);
            }

            if (filter.IsPhysical != null)
            {
                organizationsQueryable = organizationsQueryable.Where(x => x.IsPhysical == filter.IsPhysical);
            }

            if (filter.TypeId != null)
            {
                organizationsQueryable = organizationsQueryable.Where(x => x.Type.Id == filter.TypeId);
            }

            if (filter.LocalityId != null)
            {
                organizationsQueryable = organizationsQueryable.Where(x => x.Locality.Id == filter.LocalityId);
            }
        }

        return organizationsQueryable;
    }

    private static IQueryable<Organization> Sort(
        IQueryable<Organization> organizationsQueryable,
        string sort)
    {
        switch (sort)
        {
            case "default":
                break;
            case "defaultR":
                organizationsQueryable = organizationsQueryable.AsEnumerable().Reverse().AsQueryable();
                break;
            case "name":
                organizationsQueryable = organizationsQueryable.OrderBy(x => x.Name);
                break;
            case "nameR":
                organizationsQueryable = organizationsQueryable.OrderByDescending(x => x.Name);
                break;
            case "type":
                organizationsQueryable = organizationsQueryable.OrderBy(x => x.Type.Name);
                break;
            case "typeR":
                organizationsQueryable = organizationsQueryable.OrderByDescending(x => x.Type.Name);
                break;
            case "locality":
                organizationsQueryable = organizationsQueryable.OrderBy(x => x.Locality.Name);
                break;
            case "localityR":
                organizationsQueryable = organizationsQueryable.OrderByDescending(x => x.Locality.Name);
                break;
            case "address":
                organizationsQueryable = organizationsQueryable.OrderBy(x => x.Address);
                break;
            case "addressR":
                organizationsQueryable = organizationsQueryable.OrderByDescending(x => x.Address);
                break;
            case "inn":
                organizationsQueryable = organizationsQueryable.OrderBy(x => x.Inn);
                break;
            case "innR":
                organizationsQueryable = organizationsQueryable.OrderByDescending(x => x.Inn);
                break;
            case "kpp":
                organizationsQueryable = organizationsQueryable.OrderBy(x => x.Kpp);
                break;
            case "kppR":
                organizationsQueryable = organizationsQueryable.OrderByDescending(x => x.Kpp);
                break;
        }

        return organizationsQueryable;
    }
}

public class AddOrganizationViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите наименование организации")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Введите ИНН организации")]
    [Range(1, long.MaxValue, ErrorMessage = "ИНН должен быть положительным числом")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "ИНН содержит 10 цифр")]
    public long Inn { get; set; }

    [Required(ErrorMessage = "Введите КПП организации")]
    [Range(1, long.MaxValue, ErrorMessage = "КПП должен быть положительным числом")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "КПП содержит 10 цифр")]
    public long Kpp { get; set; }

    [Required(ErrorMessage = "Введите адрес регистрации организации")]
    public string Address { get; set; }

    [Required] public bool IsPhysical { get; set; }

    [Required] public int TypeId { get; set; }
    [Required] public int LocalityId { get; set; }
}

public class SearchOrganizationsViewModel
{
    public string? Name { get; set; }

    public long? Inn { get; set; }

    public long? Kpp { get; set; }

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