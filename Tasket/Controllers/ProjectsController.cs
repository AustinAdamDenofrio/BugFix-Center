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

        //Figure out Authorization
        private int? _companyId => User.FindFirst("CompanyId") != null ? int.Parse(User.FindFirst("CompanyId")!.Value) : null;





        #region Get list of items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetAllProjectsAsync()
        {
            try
            {
                IEnumerable<ProjectDTO?> project = await _projectService.GetAllProjectsAsync( _companyId!.Value);

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
    }
    #endregion
}

