using Tasket.Models;

namespace Tasket.Services.Interfaces
{
    public interface IProjectRepository
    {


        #region Get list
        Task<IEnumerable<Project>> GetAllProjectsAsync(int companyId);
        Task<IEnumerable<Project>> GetArchivedProjectsAsync(int companyId);
        #endregion


        #region Get One Item
        Task<Project?> GetProjectByIdAsync(int projectId, int companyId);
        #endregion


        #region Update DB item/items
        //Task<Project> AddProjectAsync(Project project, int companyId);
        Task UpdateProjectAsync(Project project, int companyId);
        //Task ArchiveProjectAsync(int projectId, int companyId);
        //Task RestoreProjectAsync(int projectId, int companyId);
        #endregion
    }
}
