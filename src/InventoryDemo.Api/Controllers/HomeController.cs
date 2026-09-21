using Microsoft.AspNetCore.Mvc;

namespace InventoryDemo.Api.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
