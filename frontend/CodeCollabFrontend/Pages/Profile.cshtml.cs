using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeCollabFrontend.Pages;

public class ProfileModel : PageModel
{
    public IActionResult OnPostUpdate()
    {
        return RedirectToPage("/Profile");
    }
}