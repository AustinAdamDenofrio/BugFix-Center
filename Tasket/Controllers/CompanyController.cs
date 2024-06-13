using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using Tasket.Data;
using Tasket.Helper.Extensions;

namespace Tasket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController(ICompanyDTOService _companyService, UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        private int CompanyId => int.Parse(User.FindFirst("CompanyId")!.Value);
        private string UserId => _userManager.GetUserId(User)!;


        [HttpGet]
        public async Task<ActionResult<CompanyDTO>> GetCompany()
        {
            CompanyDTO? company = await _companyService.GetCompanyByIdAsync(CompanyId);

            if (company == null) return NotFound();

            return Ok(company);
        }


        [HttpGet("members")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetCompanyMembersAsync()
        {
            IEnumerable<UserDTO> members = await _companyService.GetCompanyMembersAsync(CompanyId);

            if (members == null) return NotFound();

            return Ok(members);
        }



        [HttpGet("{userId}/role")]
        public async Task<ActionResult<string>> GetUserRoleAsync([FromRoute] string userId)
        {
            string currentRole = await _companyService.GetUserRoleAsync(userId, CompanyId);

            if (currentRole == "Unknown") return NotFound();

            return Ok(currentRole);
        }



        [HttpGet("{roleName}/members")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersInRoleAsync([FromRoute] string roleName)
        {
            IEnumerable<UserDTO> usersRoles = await _companyService.GetUsersInRoleAsync(roleName, CompanyId);

            if (usersRoles == null) return NotFound();

            return Ok(usersRoles);
        }


        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCompanyAsync([FromBody] CompanyDTO companyDTO)
        {
            if (companyDTO.Id != CompanyId) return BadRequest();

            if (User.IsInRole((nameof(Roles.Admin))))
            {
                await _companyService.UpdateCompanyAsync(companyDTO, UserId);

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("update/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRoleAsync([FromBody] UserDTO userDTO)
        {
            if (userDTO.Id == UserId) return BadRequest();

            if (User.IsInRole((nameof(Roles.Admin))))
            {
                await _companyService.UpdateUserRoleAsync(userDTO, UserId);

                return Ok();
            }

            return BadRequest();
        }

    }
}
