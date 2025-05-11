using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05._05._25Homework.Data
{
    public class QARepository
    {
        private readonly string _connectionString;

        public QARepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user)
        {            
            string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hash;

            using var ctx = new QADataContext(_connectionString);
            ctx.Users.Add(user);
            ctx.SaveChanges();
        }

        public User Login(string email, string password)
        {
            using var ctx = new QADataContext(_connectionString);
            User user = GetUser(email);

            bool isMatch = false;

            if(user != null)
            {
                isMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
            }

            return isMatch ? user : null;
        }

        public User GetUser(string email)
        {
            using var ctx = new QADataContext(_connectionString);
            return ctx.Users.FirstOrDefault(u => u.Email == email);
        }

        public void NewQuestion(Question q)
        {
            using var ctx = new QADataContext(_connectionString);
            ctx.Questions.Add(q);
            ctx.SaveChanges();
        }

        public void AddTag(Tag t)
        {
            using var ctx = new QADataContext(_connectionString);
            if (ctx.Tags.Contains(t))
            {
                return;
            }

            ctx.Tags.Add(t);
            ctx.SaveChanges();
        }

        public void AddQuestionTag(QuestionTag qt)
        {
            using var ctx = new QADataContext(_connectionString);
            ctx.QuestionTags.Add(qt);
            ctx.SaveChanges();
        }

        public List<Question> GetQuestions()
        {
            using var ctx = new QADataContext(_connectionString);
            return ctx.Questions.Include(q => q.Answers).Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag).OrderByDescending(q => q.DatePosted).ToList();
        }

        public Question GetQuestion(int id)
        {
            using var ctx = new QADataContext(_connectionString);
            return ctx.Questions.Include(q => q.User).Include(q => q.Answers).Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag).FirstOrDefault(q => q.Id == id);
        }

        public void AddAnswer(Answer a)
        {
            using var ctx = new QADataContext(_connectionString);
            ctx.Answers.Add(a);
            ctx.SaveChanges();
        }

        public bool QuestionExists(int id)
        {
            using var ctx = new QADataContext(_connectionString);
            return ctx.Questions.Select(q => q.Id).Contains(id);
        }
    }
}
