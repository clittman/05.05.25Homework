using _05._05._25Homework.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _05._05._25Homework.Web.Controllers
{
    public class AccountController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;

        public AccountController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            if (user.Name == null || user.Email == null || user.Password == null)
            {
                return Redirect("/account/signup");
            }

            QARepository repo = new(_connectionString);
            repo.AddUser(user);

            LoginNow(user.Email);
            return Redirect("/");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            QARepository repo = new(_connectionString);
            User user = repo.Login(email, password);
            if(user == null)
            {
                return Redirect("/account/login");
            }

            LoginNow(email);
            return Redirect("/home");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }

        public void LoginNow(string email)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "roles"))
                ).Wait();
        }
    }
}
