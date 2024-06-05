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

        public async Task<string> GetUserRoleAsync(string userId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/company/{userId}/role");
            response.EnsureSuccessStatusCode();

            string role = await response.Content.ReadAsStringAsync();
            return role;
        }

        public async Task UpdateCompanyAsync(CompanyDTO company, string adminId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/company/update", company);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<UserDTO>> GetCompanyMembersAsync(int companyId)
        {
            IEnumerable<UserDTO> members = await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>($"api/company/members") ?? [];
            return members;
        }

        public async Task UpdateUserRoleAsync(UserDTO user, string adminId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/company/update/role", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            IEnumerable<UserDTO> UsersRoles = await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>($"api/company/{roleName}/members") ?? [];
            return UsersRoles;
        }
    }
}
