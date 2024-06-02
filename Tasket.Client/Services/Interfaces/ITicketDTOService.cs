using System.Net.Sockets;
using Tasket.Client.Models;

namespace Tasket.Client.Services.Interfaces
{
    public interface ITicketDTOService
    {
        #region Get List Items
        Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(int companyId);

        #endregion


        #region Get One Item
        Task<TicketDTO> AddTicketAsync(TicketDTO ticket, int companyId);
        #endregion
        //Task<TicketDTO?> GetTicketByIdAsync(int ticketId, int companyId);



        #region Update DB Item/Items

        #endregion


        //Task<Ticket> UpdateTicketAsync(Ticket ticket, int companyId, string userId);

        //Task ArchiveTicketAsync(int ticketId, int companyId);
        //Task RestoreTicketAsync(int ticketId, int companyId);
    }
}
