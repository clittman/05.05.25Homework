using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05._05._25Homework.Data
{
    public class QADataContextFactory : IDesignTimeDbContextFactory<QADataContext>
    {
        public QADataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(),
   $"..{Path.DirectorySeparatorChar}05.05.25Homework.Web"))
   .AddJsonFile("appsettings.json")
   .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new QADataContext(config.GetConnectionString("ConStr"));
        }
    }
}
