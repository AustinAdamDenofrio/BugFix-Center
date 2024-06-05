using System.ComponentModel.Design;
using System.Net.Http.Json;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;

namespace Tasket.Client.Services
{
    public class WASMCompanyDTOService(HttpClient _httpClient) : ICompanyDTOService
    {
        #region Get
        #endregion
        public async Task<CompanyDTO?> GetCompanyByIdAsync(int companyId)
        {
            CompanyDTO? company = await _httpClient.GetFromJsonAsync<CompanyDTO>($"api/company");
            return company;
        }
        
        public Task<string> GetUserRoleAsync(string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task AddUserToRoleAsync(string userId, string roleName, string adminId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCompanyAsync(CompanyDTO company, string adminId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDTO>> GetCompanyMembersAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserRoleAsync(UserDTO user, string adminId)
        {
            throw new NotImplementedException();
        }
    }
}
