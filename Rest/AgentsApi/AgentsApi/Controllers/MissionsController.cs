using AgentsApi.Models;
using AgentsApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AgentsApi.Dto;

namespace AgentsApi.Controllers
{
	[Route("missions")]
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

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<string>> MissionStatusUpdate(long id)
		{
			try
			{
				string result = await missionService.MissionStatusUpdate(id);
				return Ok(new MissionUpdateDto() { Status = result });
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("update")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task MissionsUpdate()
		{
			await missionService.UpdateMissions();
		}
	}
}
