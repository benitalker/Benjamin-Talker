using AgentClient.Service;
using Microsoft.AspNetCore.Mvc;

namespace AgentClient.Controllers
{
    public class AgentController(IAgentService agentService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var agents = await agentService.GetAllAgentsDetails();
            return View(agents);
        }
    }
}
