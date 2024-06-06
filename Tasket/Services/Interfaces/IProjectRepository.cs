using Tasket.Client.Models;
using Tasket.Data;
using Tasket.Models;

namespace Tasket.Services.Interfaces
{
    public interface IProjectRepository
    {

        #region Project CRUD
        #region Get list
        Task<IEnumerable<Project>> GetAllProjectsAsync(int companyId);
        Task<IEnumerable<Project>> GetArchivedProjectsAsync(int companyId);
        #endregion
        #region Get One Item
        Task<Project?> GetProjectByIdAsync(int projectId, int companyId);
        #endregion
        #region Update DB item/items
        Task<Project> AddProjectAsync(Project project, int companyId);
        Task UpdateProjectAsync(Project project, int companyId);
        Task ArchiveProjectAsync(int projectId, int companyId);
        Task RestoreProjectAsync(int projectId, int companyId);
        #endregion
        #endregion

        #region Project Members
        Task<IEnumerable<ApplicationUser>> GetProjectMembersAsync(int projectId, int companyId);
        Task<ApplicationUser?> GetProjectManagerAsync(int projectId, int companyId);
        Task AddMemberToProjectAsync(int projectId, string userId, string managerId);
        Task RemoveMemberFromProjectAsync(int projectId, string userId, string managerId);
        Task AssignProjectManagerAsync(int projectId, string userId, string adminId);
        Task RemoveProjectManagerAsync(int projectId, string adminId);
        #endregion
    }
}
