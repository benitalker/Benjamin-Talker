using AgentClient.Service;
using Microsoft.AspNetCore.Mvc;

namespace AgentClient.Controllers
{
	public class GeneralController(IGeneralService agentService) : Controller
	{
		public async Task<IActionResult> Index()
		{
			return View(await agentService.GetGeneralStatisticsAsync());
		}
	}
}
