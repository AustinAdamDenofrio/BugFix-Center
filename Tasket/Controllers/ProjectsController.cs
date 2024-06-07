using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using Tasket.Helper.Extensions;

namespace Tasket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController(IProjectDTOService _projectService) : ControllerBase
    {

        private int? _companyId => User.FindFirst("CompanyId") != null ? int.Parse(User.FindFirst("CompanyId")!.Value) : null;

        private string UserId => User.GetUserId()!;



        #region Project CRUDs
        #region Get list of items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetAllProjectsAsync()
        {
            try
            {
                IEnumerable<ProjectDTO?> project = await _projectService.GetAllProjectsAsync(_companyId!.Value);

                if (project == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(project);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }


        [HttpGet("archive")]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetArchivedProjectsAsync()
        {
            try
            {
                IEnumerable<ProjectDTO?> project = await _projectService.GetArchivedProjectsAsync(_companyId!.Value);

                if (project == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(project);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }
        #endregion
        #region get item
        [HttpGet("{projectId:int}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectByIdAsync([FromRoute] int projectId)
        {
            try
            {
                ProjectDTO? project = await _projectService.GetProjectByIdAsync(projectId, _companyId!.Value);

                if (project == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(project);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }
        #endregion
        #region Change DB item or items
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> AddProject([FromBody] ProjectDTO projectDTO)
        {
            try
            {
                ProjectDTO project = await _projectService.AddProjectAsync(projectDTO, _companyId!.Value);
                return Ok(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("update/{projectId:int}")]
        public async Task<IActionResult> UpdateProjectAsync([FromRoute] int projectId, [FromBody] ProjectDTO projectDTO)
        {
            if (projectId == projectDTO.Id)
            {
                try
                {
                    await _projectService.UpdateProjectAsync(projectDTO, _companyId!.Value);
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return NotFound();
                }
            }
            return BadRequest();
        }

        [HttpPut("archive/{projectId:int}")]
        public async Task<IActionResult> ArchiveProjectAsync([FromRoute] int projectId, [FromBody] int projectIdFromBody)
        //this will change to check user role is admin or project manager,not projectDTO
        {

            try
            {
                await _projectService.ArchiveProjectAsync(projectId, _companyId!.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return NotFound();
            }
        }

        [HttpPut("restore/{projectId:int}")]
        public async Task<IActionResult> RestoreProjectAsync([FromRoute] int projectId, [FromBody] int projectIdFromBody)
        //this will change to check user role is admin or project manager,not projectDTO
        {

            try
            {
                await _projectService.RestoreProjectAsync(projectId, _companyId!.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return NotFound();
            }
        }
        #endregion
        #endregion


        #region Project Members

        [HttpGet("{projectId:int}/members")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetProjectMembersAsync([FromRoute] int projectId)
        {

            try
            {
                IEnumerable<UserDTO?> projectMembers = await _projectService.GetProjectMembersAsync(projectId, _companyId!.Value);

                if (projectMembers == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(projectMembers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpGet("{projectId:int}/members/project-manager")]
        public async Task<ActionResult<UserDTO?>> GetProjectManagerAsync([FromRoute] int projectId)
        {

            try
            {
                UserDTO? projectManager = await _projectService.GetProjectManagerAsync(projectId, _companyId!.Value);

                if (projectManager == null) return BadRequest();

                return Ok(projectManager);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("{projectId}/members/remove-manager")]
        public async Task<IActionResult> RemoveProjectManagerAsync([FromRoute] int projectId, [FromBody] int projectIdFromBody)
        {
            try
            {
                if (projectId != projectIdFromBody) return BadRequest();

                await _projectService.RemoveProjectManagerAsync(projectId, UserId);

                return Ok();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return Problem();
            }
        }

        [HttpPut("{projectId}/members/add")]
        public async Task<IActionResult> AddMemberToProjectAsync([FromRoute] int projectId, [FromBody] string memberId)
        {
            try
            {
                await _projectService.AddMemberToProjectAsync(projectId, memberId, UserId);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return Problem();
            }

        }

        [HttpPut("{projectId}/members/remove")]
        public async Task<IActionResult> RemoveMemberFromProjectAsync([FromRoute] int projectId, [FromBody] string memberId)
        {
            try
            {
                await _projectService.RemoveMemberFromProjectAsync(projectId, memberId, UserId);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return Problem();
            }
        }

        [HttpPut("{projectId}/members/assign-manager")]
        public async Task<IActionResult> AssignProjectManagerAsync([FromRoute] int projectId, [FromBody] string memberId)
        {
            try
            {
                await _projectService.AssignProjectManagerAsync(projectId, memberId, UserId);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return Problem();
            }
        }

        #endregion



    }
}

