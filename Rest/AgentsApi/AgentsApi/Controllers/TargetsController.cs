using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgentsApi.Data;
using AgentsApi.Models;
using AgentsApi.Dto;
using Microsoft.AspNetCore.Authorization;
using AgentsApi.Service;

namespace AgentsApi.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class TargetsController(ITargetService targetService) : ControllerBase
	{
		// Post: api/Targets
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<long>> Create([FromBody] TargetDto targetDto)
		{
			try
			{
				var target = await targetService.CreateTargetAsync(targetDto);
				return Created("succses", new IdDto() { Id = target.Id });
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		// GET: api/Targets
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<TargetModel>>> Get()
		{
			try
			{
				var targets = await targetService.GetTargetsAsync();
				return Ok(targets);
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		//PUT: /targets/{id}/pin
		[HttpPut("{id}/pin")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Pin(long id, [FromBody] PositionDto position)
		{
			try
			{
				var targetModel = await targetService.GetTargetByIdAsync(id);

				if (targetModel == null)
				{
					return NotFound();
				}
				await targetService.UpdateTargetLocation(id, position);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		//PUT:/targets/{id}/move
		[HttpPut("{id}/move")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Move(long id, [FromBody] DirectionsDto direction)
		{
			try
			{
				var targetModel = await targetService.GetTargetByIdAsync(id);

				if (targetModel == null)
				{
					return NotFound();
				}
				await targetService.MoveTarget(id, direction);
				return Ok();
			}
			catch
			{
				return NotFound();
			}
		}
	}
}
