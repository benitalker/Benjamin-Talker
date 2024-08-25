using AgentClient.Service;
using Microsoft.AspNetCore.Mvc;

namespace AgentClient.Controllers
{
    public class MissionController(IMissionService missionService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await missionService.ShowAllMissions());
        }
    }
}
