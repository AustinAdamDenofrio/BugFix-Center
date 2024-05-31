using Tasket.Client.Models;

namespace Tasket.Client.Services.Interfaces
{
    public interface IProjectDTOService
    {

        #region Get list
        Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId);
        Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync(int companyId);
        #endregion

        #region Get One Item
        Task<ProjectDTO?> GetProjectByIdAsync(int projectId, int companyId);
        #endregion

        #region Update DB item/items
        //Task<ProjectDTO> AddProjectAsync(ProjectDTO project, int companyId);
        Task UpdateProjectAsync(ProjectDTO project, int companyId);
        //Task ArchiveProjectAsync(int projectId, int companyId);
        //Task RestoreProjectAsync(int projectId, int companyId);
        #endregion
    }
}
