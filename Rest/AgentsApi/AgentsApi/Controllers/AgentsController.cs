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
	public class AgentsController(ApplicationDbContext _context, ITargetService targetService) : ControllerBase
	{
		// Post: api/Agents
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<TargetModel>> CreateTarget([FromBody] TargetDto targetDto)
		{
			try
			{
				return Created("succses", await targetService.CreateTargetAsync(targetDto));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		// GET: api/Agents
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<TargetModel>>> GetTargets()
		{
			return await targetService.GetTargetsAsync();
		}

		// GET: api/Agents/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<TargetModel>> GetTargetById(long id)
		{
			var targetModel = await targetService.GetTargetByIdAsync(id);

			if (targetModel == null)
			{
				return NotFound();
			}

			return targetModel;
		}

		// PUT: api/Agents/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]

		public async Task<IActionResult> PutTargetModel(long id, TargetModel targetModel)
		{
			try
			{
				return Ok(await targetService.UpdateTargetAsync(id, targetModel));
			}
			catch (DbUpdateConcurrencyException)
			{
				return NotFound();
			}
		}

		// DELETE: api/Agents/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteTargetModel(long id)
		{
			var targetModel = await targetService.GetTargetByIdAsync(id);

			if (targetModel == null)
			{
				return NotFound();
			}
			else
			{
				var target = await targetService.DeleteTargetAsync(id);
				return Ok(target);
			}
		}
	}
}
