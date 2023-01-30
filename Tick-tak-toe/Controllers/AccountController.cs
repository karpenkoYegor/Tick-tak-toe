using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tick_tak_toe.Models;

namespace Tick_tak_toe.Controllers
{
    public class AccountController : Controller
    {
        public async Task<ActionResult> Login()
        {
            await HttpContext.SignOutAsync("UserCookie");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                LocalData.AuthUsers.Add(model.Login);
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.Login) };
                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                HttpContext.User.AddIdentity(claimsIdentity);
                var principal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync("UserCookie", principal);
                return RedirectToAction("Lobby", "Lobby");
            }
            return View(model);
        }

        public bool UserIsNotAuth(string login)
        {
            return !(LocalData.AuthUsers.Contains(login));
        }

        public async Task<IActionResult> Logout()
        {
            LocalData.AuthUsers.Remove(User.Identity.Name);
            await HttpContext.SignOutAsync("UserCookie");
            return RedirectToAction("Login");
        }
    }
}
