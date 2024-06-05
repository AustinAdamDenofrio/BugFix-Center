using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using Tasket.Data;

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
    }
}
