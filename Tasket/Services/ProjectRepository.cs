
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
                                                .Where(p => p.Archived == false && p.CompanyId == companyId)
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
        //Task<Project> AddProjectAsync(Project project, int companyId);
        //Task UpdateProjectAsync(Project project, int companyId);
        //Task ArchiveProjectAsync(int projectId, int companyId);
        //Task RestoreProjectAsync(int projectId, int companyId);

        //Task<IEnumerable<Project>> IProjectRepository.GetAllProjectsAsync(int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<IEnumerable<Project>> IProjectRepository.GetArchivedProjectsAsync(int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<Project?> IProjectRepository.GetProjectByIdAsync(int projectId, int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Project> AddProjectAsync(Project project, int companyId)
        //{
        //    throw new NotImplementedException();
        //}

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

        //Task IProjectRepository.ArchiveProjectAsync(int projectId, int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task IProjectRepository.RestoreProjectAsync(int projectId, int companyId)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
