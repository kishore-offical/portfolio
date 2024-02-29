using ChatWe.Persistance.Context;
using ChatWe.Persistance.Entities;
using ChatWe.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatWe.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public readonly IChatWeContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IChatWeContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        #region Login/Register

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Home", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(model.EmailAddress) || string.IsNullOrEmpty(model.Password))
                    return View();

                var user = await _userManager.FindByEmailAsync(model.EmailAddress) ?? throw new Exception($"No accounts registered with {model.EmailAddress}.");
                if (user == null)
                    return RedirectToAction("Register", "Account");

                var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, false, lockoutOnFailure: true);

                if (result.IsNotAllowed)
                    throw new Exception("You are not allowed to sign in. Please confirm your email address.");

                if (result.IsLockedOut)
                    throw new Exception("Your account has been temporarily locked for security reasons. Please wait for 15 minutes and try again, or reach out to the administrator for further assistance.");

                if (!result.Succeeded)
                    throw new Exception($"Invalid Credentials for '{model.EmailAddress}'.");

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.ErrorMessage = ex.Message;
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                var user = new User
                {
                    Email = model.EmailAddress,
                    UserName = $"{model.FirstName}{model.LastName}",
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                    return RedirectToAction("Login", "Account");
                else
                    throw new Exception(result.Errors?.FirstOrDefault()!.Description);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.ErrorMessage = ex.Message;
            }
            return View();
        }

        #endregion Login/Register

        #region Home

        [HttpGet]
        public IActionResult Index()
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            return View();
        }

        #endregion Home

        #region Message

        [HttpGet]
        public async Task<IActionResult> Conversation(string userId)
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var user = await _userManager.FindByIdAsync(userId);
            var applicationUser = new ApplicationUser
            {
                EmailAddress = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.Id,
            };
            return View("Index", applicationUser);
        }

        #endregion Message

        #region Home

        [HttpGet]
        public IActionResult Home()
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = User?.Claims.FirstOrDefault().Value;
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var users = _userManager.Users.AsNoTracking().Where(x => x.Id != userId).Select(x => new ApplicationUser
            {
                EmailAddress = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserId = x.Id
            }

            ).ToList();

            return View(users);
        }

        #endregion Home

        #region Logout

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        #endregion Logout
         
        
        #region Delete

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string userId)
        {
           if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                    await _userManager.DeleteAsync(user);
                return RedirectToAction("Home", "Account");
            }
            return RedirectToAction("Login", "Account");
        }

        #endregion Delete



        #region Internal

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Account");
        }

        #endregion Internal
    }
}