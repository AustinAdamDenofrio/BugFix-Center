using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tasket.Data;
using Tasket.Models;
using Tasket.Services.Interfaces;

namespace Tasket.Services
{
    public class CompanyRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
        IServiceProvider svcProvider) : ICompanyRepository
    {
        public Task AddUserToRoleAsync(string userId, string roleName, string adminId)
        {
            throw new NotImplementedException();
        }

        public async Task<Company?> GetCompanyByIdAsync(int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            // get the company by ID and include projects, members, and invites
            Company? company = await context.Companies
                                                    .Include(c => c.Projects)
                                                    .Include(c => c.Members)
                                                    .Include(c => c.Invites)
                                                    .FirstOrDefaultAsync(c => c.Id == companyId);

            return company;
        }


        public async Task<string> GetUserRoleAsync(string userId, int companyId)
        {
            // create a UserManager just for this method, similar to creating a DbContext
            using IServiceScope scope = svcProvider.CreateScope();
            UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // find the user
            ApplicationUser? user = await userManager.FindByIdAsync(userId);

            string role = "Unknown";

            // make sure the user belongs to the company
            if (user?.CompanyId == companyId)
            {
                // get their roles
                IList<string> roles = await userManager.GetRolesAsync(user);

                // some users have their assigned role AND a DemoUser role, but we don't want to show that, 
                // so look up their first role that isn't DemoUser
                role = roles.FirstOrDefault(r => r != nameof(Roles.DemoUser), role);
            }

            // return whatever role we found
            return role;
        }

        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCompanyAsync(Company company, string adminId)
        {
            throw new NotImplementedException();
        }



    }
}

