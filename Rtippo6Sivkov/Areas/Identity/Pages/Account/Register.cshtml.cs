using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Rtippo6Sivkov.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    public Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }

    public Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }
}