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

        /*public async Task<ActionResult<string>> AfterLogin()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (token == null) { return BadRequest("Not authenticated"); }

            var httpClient = clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{loginApi}/protected");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            return BadRequest();
        }*/
    }

}