﻿using Tasket.Client.Models;

namespace Tasket.Client.Services.Interfaces
{
    public interface ICompanyDTOService
    {
        Task<CompanyDTO?> GetCompanyByIdAsync(int id);
        Task<string> GetUserRoleAsync(string userId, int companyId);
        Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(string roleName, int companyId);
        Task AddUserToRoleAsync(string userId, string roleName, string adminId);
        Task UpdateCompanyAsync(CompanyDTO company, string adminId);
        Task<IEnumerable<UserDTO>> GetCompanyMembersAsync(int companyId);
    }
}
