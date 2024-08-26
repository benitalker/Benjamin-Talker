using AgentClient.Service;
using Microsoft.AspNetCore.Mvc;

namespace AgentClient.Controllers
{
    public class TargetController(ITargetService targetService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var targets = await targetService.GetAllTargetsDetails();
            return View(targets);
        }
    }
}
