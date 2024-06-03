using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
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
                                                .Include(t => t.SubmitterUser)
                                                .Include(t => t.DeveloperUser)
                                                .Include(t => t.Project)    
                                                .ToListAsync();
            return tickets;
        }
        #endregion




        #region Get one item
        public async Task<Ticket?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Ticket? ticket = await context.Tickets
                                    .Include(t => t.SubmitterUser)
                                    .Include(t => t.DeveloperUser)
                                    .Include(t => t.Attachments)
                                        .ThenInclude(a => a.Upload)
                                    .Include(t => t.Comments)
                                    .Include(t => t.Project)
                                    .FirstOrDefaultAsync(t => t.Id == ticketId && t.Project!.CompanyId == companyId);
            return ticket;
        }

        #endregion



        #region Update DB Item
        public async Task<Ticket> AddTicketAsync(Ticket ticket, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            ticket.Created = DateTime.Now;

            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            return ticket;
        }

        //Are we supposed to have a return value? is that a requirements typo?
        public async Task UpdateTicketAsync(Ticket ticket, int companyId, string userId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            bool shouldUpdate = await context.Tickets
                                    .AnyAsync(t => t.Id == ticket.Id && t.Project!.CompanyId == companyId);

            if (shouldUpdate)
            {
                context.Tickets.Update(ticket);
                await context.SaveChangesAsync();
            }
        }


        public async Task ArchiveTicketAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Ticket? ticket = await context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.Project!.CompanyId == companyId);

            if (ticket is not null)
            {
                ticket.Archived = true;

                context.Tickets.Update(ticket);
                await context.SaveChangesAsync();
            }
        }

        public async Task RestoreTicketAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Ticket? ticket = await context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.Project!.CompanyId == companyId);

            if (ticket is not null)
            {
                ticket.Archived = false;

                context.Tickets.Update(ticket);
                await context.SaveChangesAsync();
            }
        }
        #endregion
    }
}

