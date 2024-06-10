using Tasket.Client.Models;

namespace Tasket.Client.Services.Interfaces
{
    public interface IProjectDTOService
    {
        #region Project Crud
        #region Get list
        Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId);
        Task<IEnumerable<ProjectDTO>> GetAllAssignedProjectsAsync(int companyId, string userId);
        Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync(int companyId);
        #endregion
        #region Get One Item
        Task<ProjectDTO?> GetProjectByIdAsync(int projectId, int companyId);
        #endregion
        #region Update DB item/items
        Task<ProjectDTO> AddProjectAsync(ProjectDTO project, int companyId);
        Task UpdateProjectAsync(ProjectDTO project, int companyId);
        Task ArchiveProjectAsync(int projectId, int companyId);
        Task RestoreProjectAsync(int projectId, int companyId);
        #endregion
        #endregion








        #region Project Members
        Task<IEnumerable<UserDTO>> GetProjectMembersAsync(int projectId, int companyId);
        Task<UserDTO?> GetProjectManagerAsync(int projectId, int companyId);
        Task AddMemberToProjectAsync(int projectId, string memberId, string managerId);
        Task RemoveMemberFromProjectAsync(int projectId, string memberId, string managerId);
        Task AssignProjectManagerAsync(int projectId, string memberId, string adminId);
        Task RemoveProjectManagerAsync(int projectId, string adminId);
        #endregion
    }
}
