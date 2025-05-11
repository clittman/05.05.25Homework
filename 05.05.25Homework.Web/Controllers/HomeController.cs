using _05._05._25Homework.Data;
using _05._05._25Homework.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Diagnostics;

namespace _05._05._25Homework.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            QARepository repo = new(_connectionString);
            IndexViewModel vm = new()
            {
                Questions = repo.GetQuestions()
            };
            return View(vm);
        }

        [Authorize]
        public IActionResult NewQuestion()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewQuestion(Question q, List<string> stringTags)
        {
            if(q.Title == null || q.Content == null)
            {
                return Redirect("/");
            }

            QARepository repo = new(_connectionString);
            User u = repo.GetUser(User.Identity.Name);

            q.DatePosted = DateTime.Now;
            q.UserId = u.Id;

            repo.NewQuestion(q);

            List<Tag> tags = stringTags.Select(t => new Tag()
            {
                Name = t
            }).ToList();

            foreach(Tag t in tags)
            {
                repo.AddTag(t);
                repo.AddQuestionTag(new QuestionTag()
                {
                    QuestionId = q.Id,
                    TagId = t.Id
                });
            }

            return Redirect("/home");
        }

        public IActionResult ViewQuestion(int id)
        {
            QARepository repo = new(_connectionString);
            Question q = repo.GetQuestion(id);

            if(q == null)
            {
                return Redirect("/");
            }

            return View(q);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddAnswer(Answer a)
        {
            if (a.Content == null)
            {
                return Redirect("/");
            }

            QARepository repo = new(_connectionString);

            if (!repo.QuestionExists(a.QuestionId))
            {
                return Redirect("/");
            }

            a.UserId = repo.GetUser(User.Identity.Name).Id;
            a.DatePosted = DateTime.Now;

            repo.AddAnswer(a);

            return Redirect("/");
        }
    }
}
