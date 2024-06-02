using Tasket.Models;

namespace Tasket.Services.Interfaces
{
    public interface ITicketRepository
    {

        #region Get List Items
        Task<IEnumerable<Ticket>> GetAllTicketsAsync(int companyId);
        #endregion


        #region Get One Item

        //Task<Ticket?> GetTicketByIdAsync(int ticketId, int companyId);
        #endregion



        #region Update DB Item/Items

        #endregion

        Task<Ticket> AddTicketAsync(Ticket ticket, int companyId);
        //Task<Ticket> UpdateTicketAsync(Ticket ticket, int companyId, string userId);

        //Task ArchiveTicketAsync(int ticketId, int companyId);
        //Task RestoreTicketAsync(int ticketId, int companyId);

    }
}
