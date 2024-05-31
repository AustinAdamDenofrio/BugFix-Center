using System.Net.Http.Json;
using System.Xml.Linq;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;

namespace Tasket.Client.Services
{
    public class WASMProjectDTOService : IProjectDTOService
    {
        private readonly HttpClient _httpClient;

        public WASMProjectDTOService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        #region Get List of Items
        #endregion


        #region Get Item
        #endregion



        #region Update DB item or items
        #endregion


        public async Task<ProjectDTO> AddProjectAsync(ProjectDTO newProject, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/projects", newProject);
            response.EnsureSuccessStatusCode();

            ProjectDTO? projectDTO = await response.Content.ReadFromJsonAsync<ProjectDTO>();
            return projectDTO!;
        }

        public Task ArchiveProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDTO?> GetProjectByIdAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task RestoreProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProjectAsync(ProjectDTO project, int companyId)
        {
            throw new NotImplementedException();
        }
    }
}
