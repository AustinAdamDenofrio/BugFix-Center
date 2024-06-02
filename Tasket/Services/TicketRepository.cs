using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Tasket.Data;
using Tasket.Models;
using Tasket.Services.Interfaces;

namespace Tasket.Services
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public TicketRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }


        #region Get List of Items
        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            IEnumerable<Ticket> tickets = await context.Tickets
                                                .Where(t => t.Project!.CompanyId == companyId)
                                                .OrderBy(t => t.Created)
                                                .Include(c => c.Attachments)
                                                    .ThenInclude(a => a.Upload)
                                                .ToListAsync();
            return tickets;
        }
        #endregion


        public async Task<Ticket?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Ticket? ticket = await context.Tickets
                                    .Include(t => t.SubmitterUser)
                                    .FirstOrDefaultAsync(t => t.Id == ticketId);
            return ticket;
        }


        public async Task<Ticket> AddTicketAsync(Ticket ticket, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            ticket.Created = DateTime.Now;

            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            return ticket;
        }

        #region Get one item

        #endregion



        #region Update DB Item
        #endregion
    }
}

