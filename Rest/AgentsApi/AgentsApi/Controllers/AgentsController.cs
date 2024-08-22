using AgentsApi.Data;
using AgentsApi.Dto;
using AgentsApi.Models;
using AgentsApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgentsApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AgentsController(IAgentService agentService) : ControllerBase
	{
		// Post: api/agents
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<long>> Create([FromBody] AgentDto agentDto)
		{
			try
			{
				var agent = await agentService.CreateAgentAsync(agentDto);
				return Created("succses", agent.Id);
			}
			catch (Exception ex)
			{
				return NotFound(ex);
			}
		}

		// GET: api/agents
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<IEnumerable<TargetModel>>> Get()
		{
			try
			{
				var agents = await agentService.GetAgentsAsync();
				if (!agents.Any()) 
				{ 
					return NotFound();
				}
				return Ok(agents);
			}
			catch (Exception ex)
			{
				return NotFound(ex);
			}
		}

		//PUT: /agents/{id}/pin
		[HttpPut("{id}/pin")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Pin(long id, [FromBody] PositionDto position)
		{
			try
			{
				var targetModel = await agentService.GetAgentByIdAsync(id);

				if (targetModel == null)
				{
					return NotFound();
				}
				await agentService.UpdateAgentLocation(id, position);
				return Ok();
			}
			catch (Exception ex)
			{
				return NotFound(ex);
			}
		}

		//PUT:/agents/{id}/move
		[HttpPut("{id}/move")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Move(long id, [FromBody] DirectionsDto direction)
		{
			try { 
			var agentModel = await agentService.GetAgentByIdAsync(id);

			if (agentModel == null)
			{
				return NotFound();
			}
			await agentService.MoveAgent(id, direction);
			return Ok();
			}
			catch (Exception ex)
			{
				return NotFound(ex);
			}
		}
	}
}
