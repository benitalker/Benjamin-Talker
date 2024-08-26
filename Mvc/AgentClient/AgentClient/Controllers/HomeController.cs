using AgentClient.Service;
using AgentClient.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

public class HomeController(IMatrixService matrixService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await matrixService.InitMatrix());
    }
}
