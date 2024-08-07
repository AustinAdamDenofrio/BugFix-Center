﻿using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Net.Sockets;
using Tasket.Client.Models;
using Tasket.Data;
using Tasket.Models;
using Tasket.Services.Interfaces;

namespace Tasket.Services
{
    public class TicketRepository(ICompanyRepository companyRepository, IDbContextFactory<ApplicationDbContext> _dbContextFactory) : ITicketRepository
    {



        #region Tickets
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

        public async Task<IEnumerable<Ticket>> GetUsersRecentlyEditedTicketsAsync(int companyId, string userId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            IEnumerable<Ticket> tickets = await context.Tickets.Where(t => t.Project!.CompanyId == companyId 
                                                                        && t.SubmitterUserId == userId
                                                                        && t.DeveloperUserId == userId)
                                                                .OrderByDescending(t => t.Updated)
                                                                .Take(5)
                                                                .ToListAsync();
            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(int companyId, string userId)
        {

            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            string currentRole = await companyRepository.GetUserRoleAsync(userId, companyId);

            if (currentRole == nameof(Roles.ProjectManager))
            {
                IEnumerable<Ticket> tickets = await context.Tickets
                                        .Include(t => t.Project)
                                            .ThenInclude(p => p.Members)
                                        .Where(t => t.Project!.CompanyId == companyId
                                                            && (t.Project.Members.Any(m => m.Id == userId)
                                                            || t.SubmitterUserId == userId))
                                        .OrderBy(t => t.Created)
                                        .ToListAsync();
                return tickets;
            }
            else
            {
                IEnumerable<Ticket> tickets = await context.Tickets
                                            .Include(t => t.Project)
                                                .ThenInclude(p => p.Members)
                                            .Where(t => t.Project!.CompanyId == companyId)
                                            .Where(t => t.SubmitterUserId == userId || t.DeveloperUserId == userId)
                                            .OrderBy(t => t.Created)
                                            .ToListAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<TicketComment>> GetTicketCommentsAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();


            IEnumerable<TicketComment> ticketComments = await context.TicketComments
                                                    .Where(t => t.TicketId == ticketId && t.User!.CompanyId == companyId)
                                                    .OrderBy(t => t.Created)
                                                    .Include(c => c.User)
                                                    .Include(c => c.Ticket)
                                                    .ToListAsync();
            return ticketComments;
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
                                    .Include(t => t.Comments.OrderBy(c => c.Created))
                                        .ThenInclude(c => c.User)
                                    .Include(t => t.Project)
                                    .FirstOrDefaultAsync(t => t.Id == ticketId && t.Project!.CompanyId == companyId);
            return ticket;
        }


        public async Task<TicketComment?> GetCommentByIdAsync(int ticketCommentId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            TicketComment? ticketComment = await context.TicketComments
                                            .Include(t => t.User)
                                            .FirstOrDefaultAsync(t => t.Id == ticketCommentId && t.Ticket!.Project!.CompanyId == companyId);
            return ticketComment;
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


        public async Task AddCommentAsync(TicketComment comment, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            comment.Created = DateTime.Now;

            context.TicketComments.Add(comment);
            await context.SaveChangesAsync();
        }
        public async Task DeleteCommentAsync(int commentId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            TicketComment? comment = await context.TicketComments
                                    .FirstOrDefaultAsync(t => t.Id == commentId && t.Ticket!.Project!.CompanyId == companyId);

            if (comment is not null)
            {
                context.TicketComments.Remove(comment);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateCommentAsync(TicketComment comment, string userId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            bool shouldUpdate = await context.TicketComments
                                    .AnyAsync(tc => tc.Id == comment.Id && tc.UserId == userId);

            if (shouldUpdate)
            {
                context.TicketComments.Update(comment);
                await context.SaveChangesAsync();
            }
        }
        #endregion
        #endregion




        #region Attachments
        public async Task<TicketAttachment> AddTicketAttachment(TicketAttachment attachment, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            // make sure the ticket exists and belongs to this company
            var ticket = await context.Tickets
                .FirstOrDefaultAsync(t => t.Id == attachment.TicketId && t.Project!.CompanyId == companyId);

            // save it if it does
            if (ticket is not null)
            {
                attachment.Created = DateTimeOffset.Now;
                context.TicketAttachments.Add(attachment);
                await context.SaveChangesAsync();

                return attachment;
            }
            else
            {
                throw new ArgumentException("Ticket not found");
            }
        }

        public async Task DeleteTicketAttachment(int attachmentId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            var attachment = await context.TicketAttachments
                .Include(a => a.Upload)
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.Ticket!.Project!.CompanyId == companyId);

            if (attachment is not null)
            {
                context.Remove(attachment);
                context.Remove(attachment.Upload!);
                await context.SaveChangesAsync();
            }
        }

        public async Task<TicketAttachment?> GetTicketAttachmentByIdAsync(int ticketAttachmentId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            TicketAttachment? ticket = await context.TicketAttachments
                                    .Include(a => a.Upload)
                                    .Include(a => a.User)
                                    .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId && t.Ticket.Project.CompanyId == companyId);
            return ticket;
        }
        #endregion
    }
}

