using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using Tasket.Data;
using Tasket.Models;
using Tasket.Services;
using Tasket.Services.Interfaces;

namespace Tasket.Client.Services
{
    public class ProjectDTOService : IProjectDTOService
    {
        private readonly IProjectRepository _repository;
        private readonly ICompanyRepository _companyRepository;
        public ProjectDTOService(IProjectRepository repository, ICompanyRepository companyRepository)
        {
            _repository = repository;
            _companyRepository = companyRepository;
        }

        #region Project CRUDs
        #region Get List of Items
        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId)
        {
            IEnumerable<Project> projects = await _repository.GetAllProjectsAsync(companyId);

            IEnumerable<ProjectDTO> dtos = projects.Select(bp => bp.ToDTO());

            return dtos;
        }
        public async Task<IEnumerable<ProjectDTO>> GetAllAssignedProjectsAsync(int companyId, string userId)
        {
            IEnumerable<Project> projects = await _repository.GetAllAssignedProjectsAsync(companyId, userId);

            IEnumerable<ProjectDTO> dtos = projects.Select(bp => bp.ToDTO());

            return dtos;
        }


        public async Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync(int companyId)
        {
            IEnumerable<Project> archivedProjects = await _repository.GetArchivedProjectsAsync(companyId);

            IEnumerable<ProjectDTO> dtos = archivedProjects.Select(bp => bp.ToDTO());

            return dtos;
        }
        #endregion
        #region Get Item
        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project? project = await _repository.GetProjectByIdAsync(projectId, companyId);

            return project?.ToDTO();
        }
        #endregion
        #region Update DB Item/Items
        public async Task UpdateProjectAsync(ProjectDTO project, int companyId)
        {
            Project? projectToUpdate = await _repository.GetProjectByIdAsync(project.Id, companyId);

            if (projectToUpdate is not null)
            {
                projectToUpdate.Name = project.Name;
                projectToUpdate.Description = project.Description;
                projectToUpdate.Priority = project.Priority;
                projectToUpdate.StartDate = project.StartDate;
                projectToUpdate.EndDate = project.EndDate;

                projectToUpdate.Company = null;

                projectToUpdate.Tickets = [];
                projectToUpdate.Members = [];

                await _repository.UpdateProjectAsync(projectToUpdate, companyId);
            }
        }
        public async Task<ProjectDTO> AddProjectAsync(ProjectDTO project, int companyId)
        {
            Project newProject = new Project()
            {
                Name = project.Name,
                Description = project.Description,
                Created = DateTimeOffset.Now,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Priority = project.Priority,
                Archived = project.Archived,
                CompanyId = companyId,
            };

            return (await _repository.AddProjectAsync(newProject, companyId)).ToDTO();
        }
        public async Task ArchiveProjectAsync(int projectId, int companyId)
        {
            await _repository.ArchiveProjectAsync(projectId, companyId);
        }
        public async Task RestoreProjectAsync(int projectId, int companyId)
        {
            await _repository.RestoreProjectAsync(projectId, companyId);
        }
        #endregion
        #endregion



        #region Project Members
        public async Task<IEnumerable<UserDTO>> GetProjectMembersAsync(int projectId, int companyId)
        {
            IEnumerable<ApplicationUser> members = await _repository.GetProjectMembersAsync(projectId, companyId);

            List<UserDTO> result = [];

            foreach (ApplicationUser member in members)
            {
                UserDTO userDTO = member.ToDTO();
                userDTO.Role = await _companyRepository.GetUserRoleAsync(member.Id, companyId);
                result.Add(userDTO);
            }

            return result;
        }

        public async Task<UserDTO?> GetProjectManagerAsync(int projectId, int companyId)
        {
            ApplicationUser? projectManager = await _repository.GetProjectManagerAsync(projectId, companyId); 

            if (projectManager == null) return null;

            UserDTO userDTO = projectManager.ToDTO(); 
            userDTO.Role = nameof(Roles.ProjectManager);

            return userDTO;
        }

        public async Task AddMemberToProjectAsync(int projectId, string memberId, string managerId)
        {
            await _repository.AddMemberToProjectAsync(projectId, memberId, managerId);
        }

        public async Task RemoveMemberFromProjectAsync(int projectId, string memberId, string managerId)
        {
            await _repository.RemoveMemberFromProjectAsync(projectId, memberId, managerId);
        }

        public async Task AssignProjectManagerAsync(int projectId, string memberId, string adminId)
        {
            await _repository.AssignProjectManagerAsync(projectId, memberId, adminId);
        }

        public async Task RemoveProjectManagerAsync(int projectId, string adminId)
        {
            await _repository.RemoveProjectManagerAsync(projectId, adminId);
        }

        #endregion
    }
}
