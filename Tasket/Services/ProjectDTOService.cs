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
        public ProjectDTOService(IProjectRepository repository)
        {
            _repository = repository;
        }
    

        #region Get List of Items
        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId)
        {
            IEnumerable<Project> projects = await _repository.GetAllProjectsAsync(companyId);

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
                projectToUpdate.Archived = project.Archived;
                projectToUpdate.Priority = project.Priority;

                //if (project.Members is not null)
                //{
                    //projectToUpdate.Members.Clear();

                    //foreach (UserDTO memberDTO in project.Members)
                    //{
                    //    //Get member from claims principle by matching some ids to other ids
                    //    projectToUpdate.Members.Add();
                    //}
                //}


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







    }
}
