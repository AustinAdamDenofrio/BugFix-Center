using System.ComponentModel.Design;
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
        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId)
        {
            IEnumerable<ProjectDTO> projects = await _httpClient.GetFromJsonAsync<IEnumerable<ProjectDTO>>("api/projects") ?? [];
            return projects;
        }
        public async Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync(int companyId)
        {
            IEnumerable<ProjectDTO> projects = await _httpClient.GetFromJsonAsync<IEnumerable<ProjectDTO>>("api/projects/archive") ?? [];
            return projects;
        }
        #endregion


        #region Get Item
        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, int companyId)
        {
            ProjectDTO? project = await _httpClient.GetFromJsonAsync<ProjectDTO>($"api/projects/{projectId}");
            return project;
        }
        #endregion



        #region Update DB item or items
        public async Task<ProjectDTO> AddProjectAsync(ProjectDTO newProject, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/projects", newProject);
            response.EnsureSuccessStatusCode();

            ProjectDTO? projectDTO = await response.Content.ReadFromJsonAsync<ProjectDTO>();
            return projectDTO!;
        }
        public async Task ArchiveProjectAsync(int projectId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/archive/{projectId}", projectId);
            response.EnsureSuccessStatusCode();
        }
        public async Task RestoreProjectAsync(int projectId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/restore/{projectId}", projectId);
            response.EnsureSuccessStatusCode();
        }
        public async Task UpdateProjectAsync(ProjectDTO updatedProject, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/update/{updatedProject.Id}", updatedProject);
            response.EnsureSuccessStatusCode();
        }
        #endregion






    }
}
