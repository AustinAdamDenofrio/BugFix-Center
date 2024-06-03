using System.Net.Http;
using System.Net.Http.Json;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;

namespace Tasket.Client.Services
{
    public class WASMTicketDTOService(HttpClient _httpClient) : ITicketDTOService
    {

        #region Get Many
        public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(int companyId)
        {
            IEnumerable<TicketDTO> tickets = await _httpClient.GetFromJsonAsync<IEnumerable<TicketDTO>>("api/tickets") ?? [];
            return tickets;
        }
        #endregion



        #region Get One
        public async Task<TicketDTO?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            TicketDTO? ticket = await _httpClient.GetFromJsonAsync<TicketDTO>($"api/tickets/{ticketId}");
            return ticket;
        }
        #endregion



        #region Update W/ Return
        public async Task<TicketDTO> AddTicketAsync(TicketDTO ticket, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/tickets/{ticket.Id}", ticket);
            response.EnsureSuccessStatusCode();

            TicketDTO? ticketDTO = await response.Content.ReadFromJsonAsync<TicketDTO>();
            return ticketDTO!;
        }
        #endregion




        #region Update DB item
        public async Task UpdateTicketAsync(TicketDTO ticket, int companyId, string userId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/tickets/{ticket.Id}", ticket);
            response.EnsureSuccessStatusCode();
        }
        public async Task ArchiveTicketAsync(int ticketId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/tickets/archive/{ticketId}", ticketId);
            response.EnsureSuccessStatusCode();
        }
        public async Task RestoreTicketAsync(int ticketId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/tickets/restore/{ticketId}", ticketId);
            response.EnsureSuccessStatusCode();
        }

        public Task<IEnumerable<TicketCommentDTO>> GetTicketCommentsAsync(int ticketId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<TicketCommentDTO?> GetCommentByIdAsync(int ticketId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task AddCommentAsync(TicketCommentDTO comment, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCommentAsync(int commentId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCommentAsync(TicketCommentDTO comment, int companyId, string userId)
        {
            throw new NotImplementedException();
        }
        #endregion







    }
}
