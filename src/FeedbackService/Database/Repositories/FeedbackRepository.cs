using FeedbackService.Database.Models;
using FeedbackService.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackService.Database.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {

        private readonly NpgSqlContext _context;

        public FeedbackRepository(NpgSqlContext context)
        {
            _context = context;
        }

        public async Task AddFeedbackAsync(Guid id, string text, int starsAmount)
        {
            var feedback = new Feedback(id, text, starsAmount);

            await _context.Feedbacks!
                .AddAsync(feedback);

            await _context.SaveChangesAsync();
        }

        public async Task<Feedback> GetFeedbackAsync(Guid id)
        {
            var feedback = await _context.Feedbacks!
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            return feedback!;
        }

        public async Task<Feedback?> FindFeedbackAsync(Guid id)
        {
            var feedback = await _context.Feedbacks!
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            return feedback;
        }
    }
}
