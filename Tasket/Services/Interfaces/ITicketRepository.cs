using Tasket.Models;

namespace Tasket.Services.Interfaces
{
    public interface ITicketRepository
    {
        #region Tickets
            #region Get List Items
            Task<IEnumerable<Ticket>> GetAllTicketsAsync(int companyId);
            #endregion


            #region Get One Item

            Task<Ticket?> GetTicketByIdAsync(int ticketId, int companyId);
            #endregion



            #region Update DB Item (Returns a value)
            Task<Ticket> AddTicketAsync(Ticket ticket, int companyId);
            #endregion


            #region Update DB Item (Returns Nothing)
            Task UpdateTicketAsync(Ticket ticket, int companyId, string userId);
            Task ArchiveTicketAsync(int ticketId, int companyId);
            Task RestoreTicketAsync(int ticketId, int companyId);
        #endregion

        #endregion



        #region Comments
            #region Get List Items
                Task<IEnumerable<TicketComment>> GetTicketCommentsAsync(int ticketId,int companyId);
            #endregion


            #region Get One Item
                Task<TicketComment?> GetCommentByIdAsync(int ticketId,int companyId);
            #endregion


            #region Update DB Item (Returns Nothing)
                Task AddCommentAsync(TicketComment comment, int companyId);
                Task DeleteCommentAsync(int commentId, int companyId);
                Task UpdateCommentAsync(TicketComment comment, string userId);
        #endregion
        #endregion





        #region Attachments
        Task<TicketAttachment> AddTicketAttachment(TicketAttachment attachment, int companyId);
        Task DeleteTicketAttachment(int attachmentId, int companyId);
        #endregion

    }
}
