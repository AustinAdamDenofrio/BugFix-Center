using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using NuGet.Protocol.Core.Types;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using Tasket.Helper;
using Tasket.Models;
using Tasket.Services.Interfaces;

namespace Tasket.Services
{
    public class TicketDTOService : ITicketDTOService
    {
        private readonly ITicketRepository _repository;
        public TicketDTOService(ITicketRepository repository)
        {
            _repository = repository;
        }


        #region Get many Items
        public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(int companyId)
        {
            IEnumerable<Ticket> tickets = await _repository.GetAllTicketsAsync(companyId);

            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());

            return dtos;
        }
        #endregion



        #region Get Item
        public async Task<TicketDTO?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            Ticket? ticket = await _repository.GetTicketByIdAsync(ticketId, companyId);

            return ticket?.ToDTO();
        }
        #endregion



        #region Update DB item
        public async Task<TicketDTO> AddTicketAsync(TicketDTO ticket, int companyId)
        {
            Ticket newTicket = new Ticket()
            {
                Title = ticket.Title,
                Description = ticket.Description,
                Created = DateTimeOffset.Now,
                ArchivedByProject = ticket.ArchivedByProject,
                Status = TicketStatus.New,
                Priority = ticket.Priority,
                ProjectId = ticket.ProjectId,
                SubmitterUserId = ticket.SubmitterUserId,
                DeveloperUserId = ticket.DeveloperUserId,
            };

            newTicket = await _repository.AddTicketAsync(newTicket, companyId);


            return (await _repository.AddTicketAsync(newTicket, companyId)).ToDTO();
        }
        public async Task UpdateTicketAsync(TicketDTO ticket, int companyId, string userId)
        {
            //remove attachments from ticket before adding in new attachments

            Ticket? ticketToUpdate = await _repository.GetTicketByIdAsync(ticket.Id, companyId);

            if (ticketToUpdate is not null)
            {
                ticketToUpdate.Title = ticket.Title;
                ticketToUpdate.Description = ticket.Description;
                ticketToUpdate.Updated = DateTimeOffset.Now;
                ticketToUpdate.Archived = ticket.Archived;
                ticketToUpdate.ArchivedByProject = ticket.ArchivedByProject;
                ticketToUpdate.Priority = ticket.Priority;
                ticketToUpdate.Type = ticket.Type;
                ticketToUpdate.Status = ticket.Status;
                ticketToUpdate.DeveloperUserId = ticket.DeveloperUserId;

                await _repository.UpdateTicketAsync(ticketToUpdate, companyId, userId);
            }
        }
        public async Task ArchiveTicketAsync(int ticketId, int companyId)
        {
            await _repository.ArchiveTicketAsync(ticketId, companyId);
        }
        public async Task RestoreTicketAsync(int ticketId, int companyId)
        {
            await _repository.RestoreTicketAsync(ticketId, companyId);
        }
        #endregion



    }
}
