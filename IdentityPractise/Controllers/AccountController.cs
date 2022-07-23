using IdentityPractise.DAL;
using IdentityPractise.Helpers;
using IdentityPractise.Models;
using IdentityPractise.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityPractise.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(AppDbContext context, UserManager<AppUser>userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        

        /// <summary>
        /// Register Action 
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            
            return View();
        }

        /// <summary>
        /// Register Action Post 
        /// </summary>
        /// <param name="registerVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM) 
        {
           if (!ModelState.IsValid) return View();

            AppUser appUser = new AppUser
            {
                FullName=registerVM.FullName,
                UserName=registerVM.UserName,
                Email=registerVM.Email,
            };

          IdentityResult result= await _userManager.CreateAsync(appUser,registerVM.Password);
            if (!result.Succeeded) 
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);
                }
                return View(registerVM);
            }
            await _userManager.AddToRoleAsync(appUser, UserRoles.Admin.ToString());

            return RedirectToAction("login");
        }

        /// <summary>
        /// LOgin Action
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }


        /// <summary>
        /// Login Post Action
        /// </summary>
        /// <param name="loginVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser appUser=await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null) 
            {
                ModelState.AddModelError("", "email or password is not correct");
                return View(loginVM);
            }
            SignInResult result = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "is Lockouted");
                return View(loginVM);
            }
            if (result.Succeeded)
            {
                ModelState.AddModelError("", "email or password is not correct");
                return View(loginVM);
            }

           await _signInManager.SignInAsync(appUser, loginVM.RememberMe);


            return RedirectToAction("index","home");
        }


        /// <summary>
        /// Create role action
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateRole()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });

                };
            };
            return RedirectToAction("index", "home");
        }
    }
}
