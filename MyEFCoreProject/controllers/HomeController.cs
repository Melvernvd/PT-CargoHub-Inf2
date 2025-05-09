using Microsoft.AspNetCore.Mvc;

namespace MyEFCoreProject.Controllers;

[Route("api/v1")]
[ApiController]
public class HomeController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Home()
    {
        return Redirect("/swagger"); // This will redirect users to the Swagger UI
    }
}