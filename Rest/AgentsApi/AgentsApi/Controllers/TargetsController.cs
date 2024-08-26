using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AgentsApi.Models;
using AgentsApi.Dto;
using AgentsApi.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace AgentsApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TargetsController(ITargetService _targetService, IMissionService _missionService) : ControllerBase
    {
        // POST: api/Targets
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<long>> Create([FromBody] TargetDto targetDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var target = await _targetService.CreateTargetAsync(targetDto);
                return CreatedAtAction(nameof(Get), new { id = target.Id }, new IdDto { Id = target.Id });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the target.");
            }
        }

        // GET: api/Targets
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TargetModel>>> Get()
        {
            try
            {
                var targets = await _targetService.GetTargetsAsync();
                return Ok(targets);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving targets.");
            }
        }

        // PUT: /targets/{id}/pin
        [HttpPut("{id}/pin")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Pin(long id, [FromBody] PositionDto position)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var targetExists = await _targetService.GetTargetByIdAsync(id);
                if (targetExists == null)
                {
                    return NotFound($"Target with ID {id} not found.");
                }

                await _targetService.UpdateTargetLocation(id, position);
                var agents = await _targetService.GetAgentsForMissions(id);

                await Task.WhenAll(agents.Select(a => _missionService.CreateMission(id, a.Id)));

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the target's location.");
            }
        }

        // PUT: /targets/{id}/move
        [HttpPut("{id}/move")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Move(long id, [FromBody] DirectionsDto direction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var targetExists = await _targetService.GetTargetByIdAsync(id);
                if (targetExists == null)
                {
                    return NotFound($"Target with ID {id} not found.");
                }

                await _targetService.MoveTarget(id, direction);
                var agents = await _targetService.GetAgentsForMissions(id);

                await Task.WhenAll(agents.Select(a => _missionService.CreateMission(id, a.Id)));

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while moving the target.");
            }
        }
    }
}