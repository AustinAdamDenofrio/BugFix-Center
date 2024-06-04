using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using Tasket.Data;
using Tasket.Models;
using Tasket.Services.Interfaces;

namespace Tasket.Services
{
    public class CompanyDTOService(ICompanyRepository _repository) : ICompanyDTOService
    {
        public Task AddUserToRoleAsync(string userId, string roleName, string adminId)
        {
            throw new NotImplementedException();
        }

        public async Task<CompanyDTO?> GetCompanyByIdAsync(int companyId)
        {
            Company? company = await _repository.GetCompanyByIdAsync(companyId);

            return company?.ToDTO();
        }




        public async Task<string> GetUserRoleAsync(string userId, int companyId)
        {
            //Company? company = await _repository.GetCompanyByIdAsync(companyId);

            //if (company is null) return null;

            //string? Role = await _repository.GetUserRoleAsyncAsync(userId, companyId);
            throw new NotImplementedException();

        }





        public async Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(string roleName, int companyId)
        {

            throw new NotImplementedException();
        }

        public Task UpdateCompanyAsync(CompanyDTO company, string adminId)
        {
            throw new NotImplementedException();
        }




        public async Task<IEnumerable<UserDTO>> GetCompanyMembersAsync(int companyId)
        {
            Company? company = await _repository.GetCompanyByIdAsync(companyId);

            if (company is null) return [];

            List<UserDTO> members = [];
            foreach (ApplicationUser user in company.Members)
            {
                UserDTO member = user.ToDTO();
                member.Role = await _repository.GetUserRoleAsync(user.Id, companyId);
                members.Add(member);
            }
            return members;
        }
    }
}
