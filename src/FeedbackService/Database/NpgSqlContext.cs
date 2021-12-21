using FeedbackService.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackService.Database
{
    public class NpgSqlContext : DbContext
    {

        public DbSet<Feedback>? Feedbacks { get; set; }

        public NpgSqlContext()
        {

        }

        public NpgSqlContext(DbContextOptions options) : base(options)
        {

        }
    }
}
