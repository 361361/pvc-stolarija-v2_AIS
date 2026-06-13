using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using PvcStolarija.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PvcStolarija.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IKupacService _kupacService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IKupacService kupacService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _kupacService = kupacService;
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Neispravno korisničko ime ili lozinka.");
            }
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Register()
        {
            ViewBag.Kupci = await _kupacService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.KupacId.HasValue && model.KupacId.Value > 0)
                {
                    var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.KupacId == model.KupacId.Value);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("", "Ovaj kupac već ima kreiran nalog!");
                        ViewBag.Kupci = await _kupacService.GetAllAsync();
                        return View(model);
                    }
                }
                var user = new ApplicationUser
                {
                    UserName = model.Username, Email = model.Email, Ime = model.Ime,
                    Prezime = model.Prezime, KupacId = model.KupacId > 0 ? model.KupacId : null
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.KupacId.HasValue && model.KupacId.Value > 0 ? "Kupac" : "Administrator");
                    TempData["SuccessMessage"] = $"Korisnik {user.UserName} uspešno kreiran!";
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            ViewBag.Kupci = await _kupacService.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}
