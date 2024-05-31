
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using Tasket.Data;
using Tasket.Models;
using Tasket.Services.Interfaces;

namespace Tasket.Services
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ProjectRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }


        #region Get list
        public async Task<IEnumerable<Project>> GetAllProjectsAsync(int companyId)
        {
            using ApplicationDbContext context = _dbContextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                                                .Where(p => p.CompanyId == companyId)
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




    }
}
