using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using AgentClient.Dto;
using System.Net.Http.Headers;
using AgentClient.Service;

namespace AgentClient.Controllers
{
    public class LoginController(ILoginService loginService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string id)
        {
            bool success = await loginService.PerformLoginAsync(id);
            if (success)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("LoginError");
        }

        public IActionResult LoginError()
        {
            return View();
        }
    }
}