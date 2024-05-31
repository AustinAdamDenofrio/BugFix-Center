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


        #region
        #endregion


        #region
        #endregion


        #region Change DB item or items
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> AddProject([FromBody] ProjectDTO projectDTO)
        {
            try
            {
                ProjectDTO contact = await _projectService.AddProjectAsync(projectDTO, _companyId!.Value);
                return Ok(contact);
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
