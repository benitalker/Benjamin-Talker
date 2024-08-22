using AgentsApi.Models;
using AgentsApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgentsApi.Controllers
{
	[Route("api/missions")]
	[ApiController]
	public class MissionsController(IMissionService missionService) : ControllerBase
	{
		// GET: api/missions
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<IEnumerable<TargetModel>>> GetMissions()
		{
			try
			{
				var missions = await missionService.GetMissionsAsync();
				if (!missions.Any())
				{
					return NotFound();
				}
				return Ok(missions);
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
	}
}
