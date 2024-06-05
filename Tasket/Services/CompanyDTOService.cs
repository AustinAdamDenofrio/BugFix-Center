using Tasket.Client;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using Tasket.Data;
using Tasket.Helper;
using Tasket.Models;
using Tasket.Services.Interfaces;

namespace Tasket.Services
{
    public class CompanyDTOService(ICompanyRepository _repository) : ICompanyDTOService
    {
        public async Task<CompanyDTO?> GetCompanyByIdAsync(int companyId)
        {
            Company? company = await _repository.GetCompanyByIdAsync(companyId);

            return company?.ToDTO();
        }
        public async Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            // get the users in this role
            IEnumerable<ApplicationUser> users = await _repository.GetUsersInRoleAsync(roleName, companyId);
            // make them DTOs
            IEnumerable<UserDTO> userDTOs = users.Select(u => u.ToDTO());

            // we don't have to look up their role, we already know what it is
            foreach (UserDTO user in userDTOs)
            {
                // so just assign the role
                user.Role = roleName;
            }
            return userDTOs;
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
        public async Task<string> GetUserRoleAsync(string userId, int companyId)
        {
            string? role = await _repository.GetUserRoleAsync(userId, companyId);

            return role;
        }
        public async Task UpdateCompanyAsync(CompanyDTO company, string adminId)
        {
            Company? companyToUpdate = await _repository.GetCompanyByIdAsync(company.Id);

            if (companyToUpdate is not null)
            {
                companyToUpdate.Name = company.Name;
                companyToUpdate.Description = company.Description;

                if (company.ImageUrl?.StartsWith("data:") == true)
                {
                    companyToUpdate.Image = UploadHelper.GetFileUpload(company.ImageUrl);
                }
                else
                {
                    companyToUpdate.Image = null;
                }

                await _repository.UpdateCompanyAsync(companyToUpdate, adminId);
            }
        }
        public async Task UpdateUserRoleAsync(UserDTO user, string adminId)
        {
            if (string.IsNullOrEmpty(user.Role)) return;

            await _repository.AddUserToRoleAsync(user.Id!, user.Role, adminId);
        }


    }
}
