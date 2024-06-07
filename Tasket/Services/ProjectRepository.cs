
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.Design;
using Tasket.Data;
using Tasket.Models;
using Tasket.Services.Interfaces;

namespace Tasket.Services
{
    public class ProjectRepository(IServiceProvider svcProvider, IDbContextFactory<ApplicationDbContext> _dbContextFactory) : IProjectRepository
    {

        #region Project CRUD
        #region Get list
        public async Task<IEnumerable<Project>> GetAllProjectsAsync(int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                                                .Where(p => p.CompanyId == companyId && p.Archived == false)
                                                .Include(p => p.Tickets)
                                                .ToListAsync();
            return projects;
        }
        public async Task<IEnumerable<Project>> GetArchivedProjectsAsync(int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                                                .Where(p => p.Archived == true && p.CompanyId == companyId)
                                                .ToListAsync();
            return projects;
        }
        #endregion
        #region Get One Item
        public async Task<Project?> GetProjectByIdAsync(int projectId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Project? project = await context.Projects
                                    .Include(p => p.Members)
                                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);



            return project;
        }
        #endregion
        #region Update DB item/items
        public async Task<Project> AddProjectAsync(Project project, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            project.Created = DateTime.Now;

            context.Projects.Add(project);
            await context.SaveChangesAsync();

            return project;
        }
        public async Task UpdateProjectAsync(Project project, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            bool shouldUpdate = await context.Projects.AnyAsync(p => p.CompanyId == companyId);

            if (shouldUpdate)
            {
                context.Projects.Update(project);
                await context.SaveChangesAsync();
            }
        }
        public async Task ArchiveProjectAsync(int projectId, int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Project? project = await context.Projects
                                    .Include(p => p.Tickets)
                                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            if (project is not null)
            {
                project.Archived = true;
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.Archived = true;
                }
                context.Projects.Update(project);
                await context.SaveChangesAsync();
            }
        }
        public async Task RestoreProjectAsync(int projectId, int companyId)
        {

            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            Project? project = await context.Projects
                                    .Include(p => p.Tickets)
                                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
            if (project is not null)
            {
                project.Archived = false;
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.Archived = false;
                }

                context.Projects.Update(project);
                await context.SaveChangesAsync();
            }
        }
        #endregion
        #endregion


        #region Project Members
            public async Task AssignProjectManagerAsync(int projectId, string userId, string adminId)
            {
                using ApplicationDbContext context = _dbContextFactory.CreateDbContext();
                using IServiceScope scope = svcProvider.CreateScope();
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // get the user attempting to assign a PM
                ApplicationUser? admin = await context.Users.FindAsync(adminId);
                if (admin is null) return;

                bool isAdmin = admin is not null && await userManager.IsInRoleAsync(admin, nameof(Roles.Admin));
                bool isPM = admin is not null && await userManager.IsInRoleAsync(admin, nameof(Roles.ProjectManager));

                // if they're an admin OR a PM assigning themselves a project, continue
                // (e.g. a PM creating a new project should be assigned to it by default)

                if (isAdmin == true || (isPM == true && userId == adminId))
                {
                    ApplicationUser? projectManager = await context.Users.FindAsync(userId);

                    // make sure the user being assigned is a PM in the same company
                    if (projectManager is not null
                    && projectManager.CompanyId == admin!.CompanyId
                    && await userManager.IsInRoleAsync(projectManager, nameof(Roles.ProjectManager)))
                    {
                        // remove any existing PM, since there can only be one
                        await RemoveProjectManagerAsync(projectId, adminId);

                        Project? project = await context.Projects
                            .Include(p => p.Members)
                            .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == admin.CompanyId);

                        // finally, add this user as a PM if the project exists
                        if (project is not null)
                        {
                            context.Users.Update(projectManager);
                            project.Members.Add(projectManager);
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }

            public async Task RemoveProjectManagerAsync(int projectId, string adminId)
            {
                using IServiceScope scope = svcProvider.CreateScope();
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                ApplicationUser? admin = await userManager.FindByIdAsync(adminId);

                if (admin is null) return;

                ApplicationUser? projectManager = await GetProjectManagerAsync(projectId, admin.CompanyId);

                // there's nothing to do if there isn't a PM
                if (projectManager is null) return;

                // if there is a PM, make sure the user attempting to remove them is an admin
                if (await userManager.IsInRoleAsync(admin, nameof(Roles.Admin)))
                {
                    await RemoveMemberFromProjectAsync(projectId, projectManager.Id, adminId);
                }
            }

            public async Task RemoveMemberFromProjectAsync(int projectId, string userId, string managerId)
            {
                using ApplicationDbContext context = _dbContextFactory.CreateDbContext();
                using IServiceScope scope = svcProvider.CreateScope();
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Look up the user who is trying to remove a member
                ApplicationUser? manager = await userManager.FindByIdAsync(managerId);
                if (manager is null) return;


                // look up the project by ID and ensure it has the same CompanyId
                Project? project = await context.Projects
                                                .Include(p => p.Members)
                                                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == manager.CompanyId);
                if (project is null) return;

                // Look for the user to remove from the project's members
                ApplicationUser? memberToRemove = project.Members.FirstOrDefault(m => m.Id == userId);
                if (memberToRemove is null) return;

                // finally, remove the requested member and save our changes
                project.Members.Remove(memberToRemove);
                await context.SaveChangesAsync();
            }


            public async Task<ApplicationUser?> GetProjectManagerAsync(int projectId, int companyId)
            {
                IEnumerable<ApplicationUser> projectMembers = await GetProjectMembersAsync(projectId, companyId);

                using IServiceScope scope = svcProvider.CreateScope();
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                foreach (ApplicationUser member in projectMembers)
                {
                    bool isProjectManager = await userManager.IsInRoleAsync(member, nameof(Roles.ProjectManager));

                    if (isProjectManager == true)
                    {
                        // exit the loop early and return the project manager
                        return member;

                        // no need to loop through any more members, since there's only one PM per project 
                        // and we just found it

                    }
                }

                // if we finished the loop and didnt find a PM, there must not be one
                return null;
            }


            public async Task<IEnumerable<ApplicationUser>> GetProjectMembersAsync(int projectId, int companyId)
            {
                using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

                Project? project = await context.Projects
                                .Include(p => p.Members)
                                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

                return project?.Members ?? [];
            }


            public async Task AddMemberToProjectAsync(int projectId, string userId, string managerId)
            {
                using ApplicationDbContext context = _dbContextFactory.CreateDbContext();
                using IServiceScope scope = svcProvider.CreateScope();
                UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // get the user trying to add a member to a project
                ApplicationUser? manager = await userManager.FindByIdAsync(managerId);

                if (manager is null) return; // if we can't find them, we can't do anything
            
                // if they're an admin, they can do whatever they like
                bool isAdmin = await userManager.IsInRoleAsync(manager, nameof(Roles.Admin));


                // if they're not an admin, they must be the assigned PM of this project
                if (isAdmin == false)
                {
                    ApplicationUser? projectManager = await GetProjectManagerAsync(projectId, manager.CompanyId);

                    // they're not an admin nor are they the assigned PM for this project, so don't let them add members 
                    if (projectManager?.Id != managerId) return;
                }

                // look for a user with the given ID in the same company as the manager
                ApplicationUser? userToAdd = await context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == manager.CompanyId); 
                if (userToAdd is null) return;

                // if the new member is a PM, they need to be added using AssignProject ManagerAsync instead
                bool userIsProjectManager = await userManager.IsInRoleAsync(userToAdd, nameof(Roles.ProjectManager));
                if (userIsProjectManager) return;

                // admins are never assigned projects, so don't add them if they're an admin
                bool userIsAdmin = await userManager.IsInRoleAsync(userToAdd, nameof(Roles.Admin));
                if (userIsAdmin) return;

                // we've checked the user, so now we can check the project
                Project? project = await context.Projects
                                    .Include(p => p.Members)
                                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == manager.CompanyId);

                // quit if we don't find a project
                if (project is null) return;

                // if the user isn't already a member, add them
                if (project.Members.Any(m => m.Id == userToAdd.Id) == false)
                {
                    project.Members.Add(userToAdd);
                    await context.SaveChangesAsync();
                }
            }


        #endregion
    }
}
