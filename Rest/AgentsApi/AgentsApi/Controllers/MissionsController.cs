using AgentsApi.Models;
using AgentsApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AgentsApi.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace AgentsApi.Controllers
{
    [Route("missions")]
    [ApiController]
    public class MissionsController(IMissionService _missionService) : ControllerBase
    {

        // GET: missions
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TargetModel>>> GetMissions()
        {
            try
            {
                var missions = await _missionService.GetMissionsAsync();
                return missions.Any()
                    ? Ok(missions)
                    : NotFound("No missions found.");
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving missions.");
            }
        }

        // PUT: missions/{id}
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MissionUpdateDto>> MissionStatusUpdate(long id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid mission ID.");
            }

            try
            {
                var result = await _missionService.MissionStatusUpdate(id);
                return Ok(new MissionUpdateDto { Status = result });
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Mission with ID {id} not found.");
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the mission status.");
            }
        }

        // POST: missions/update
        [HttpPost("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MissionsUpdate()
        {
            try
            {
                await _missionService.UpdateMissions();
                return Ok("Missions updated successfully.");
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating missions.");
            }
        }
    }
}