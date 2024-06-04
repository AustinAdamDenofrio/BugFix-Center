using Tasket.Client.Models;
using Tasket.Data;
using Tasket.Models;

namespace Tasket.Services.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompanyByIdAsync(int id);
        Task<string> GetUserRoleAsync(string userId, int companyId);
        Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName, int companyId);
        Task AddUserToRoleAsync(string userId, string roleName, string adminId);
        Task UpdateCompanyAsync(Company company, string adminId);        
    }
}
