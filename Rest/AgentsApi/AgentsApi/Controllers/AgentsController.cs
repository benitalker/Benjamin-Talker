using AgentsApi.Data;
using AgentsApi.Dto;
using AgentsApi.Models;
using AgentsApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AgentsApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AgentsController(IAgentService _agentService, IMissionService _missionService) : ControllerBase
    {
        // POST: api/agents
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<long>> Create([FromBody] AgentDto agentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var agent = await _agentService.CreateAgentAsync(agentDto);
                return CreatedAtAction(nameof(Get), new { id = agent.Id }, new IdDto { Id = agent.Id });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the agent.");
            }
        }

        // GET: api/agents
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TargetModel>>> Get()
        {
            try
            {
                var agents = await _agentService.GetAgentsAsync();
                return agents.Any() ? Ok(agents) : NotFound("No agents found.");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving agents.");
            }
        }

        // PUT: /agents/{id}/pin
        [HttpPut("{id}/pin")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Pin(long id, [FromBody] PositionDto position)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var agentExists = await _agentService.GetAgentByIdAsync(id);
                if (agentExists == null)
                {
                    return NotFound($"Agent with ID {id} not found.");
                }

                await _agentService.UpdateAgentLocation(id, position);
                var targets = await _agentService.GetTargetsForMissions(id);

                await Task.WhenAll(targets.Select(t => _missionService.CreateMission(id, t.Id)));

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the agent's location.");
            }
        }

        // PUT: /agents/{id}/move
        [HttpPut("{id}/move")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Move(long id, [FromBody] DirectionsDto direction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var agentExists = await _agentService.GetAgentByIdAsync(id);
                if (agentExists == null)
                {
                    return NotFound($"Agent with ID {id} not found.");
                }

                await _agentService.MoveAgent(id, direction);
                var targets = await _agentService.GetTargetsForMissions(id);

                await Task.WhenAll(targets.Select(t => _missionService.CreateMission(id, t.Id)));

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while moving the agent.");
            }
        }
    }
}