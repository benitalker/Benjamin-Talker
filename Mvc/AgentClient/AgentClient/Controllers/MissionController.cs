﻿using AgentClient.Service;
using Microsoft.AspNetCore.Mvc;

namespace AgentClient.Controllers
{
    public class MissionController(IMissionService missionService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await missionService.ShowAllMissions());
        }

        public async Task<IActionResult> Details(long id)
        {
            try
            {
                var missionDetails = await missionService.GetMissionDetails(id);
                if (missionDetails == null)
                {
                    return NotFound();
                }
                return View(missionDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> AssignMission(long id)
        {
            try
            {
                await missionService.AssignMissionToAgent(id);
                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
