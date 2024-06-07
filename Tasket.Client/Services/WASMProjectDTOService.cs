﻿using System.ComponentModel.Design;
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

        #region Project CRUDs
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
        #endregion




        #region Project Members

        public async Task<IEnumerable<UserDTO>> GetProjectMembersAsync(int projectId, int companyId)
        {
            IEnumerable<UserDTO> projectMembers = await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>($"api/projects/{projectId}/members") ?? [];
            return projectMembers;
        }

        public async Task<UserDTO?> GetProjectManagerAsync(int projectId, int companyId)
        {
            UserDTO? projectManager = await _httpClient.GetFromJsonAsync<UserDTO>($"api/projects/{projectId}/members/project-manager");
            return projectManager;
        }


        public async Task AddMemberToProjectAsync(int projectId, string memberId, string managerId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/members/add", memberId);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveMemberFromProjectAsync(int projectId, string memberId, string managerId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/members/remove", memberId);
            response.EnsureSuccessStatusCode();
        }

        public async Task AssignProjectManagerAsync(int projectId, string memberId, string adminId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/members/assign-manager", memberId);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveProjectManagerAsync(int projectId, string adminId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/members/remove-manager", projectId);
            response.EnsureSuccessStatusCode();
        }

        #endregion





    }
}
