using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Tasket.Data;
using Tasket.Models;

namespace Tasket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //DI
    public class UploadsController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        [OutputCache(VaryByRouteValueNames = ["id"], Duration = 60 * 60)]
        public async Task<IActionResult> GetImage(Guid id)
        {

            FileUpload? image = await context.Files.FirstOrDefaultAsync(i => i.Id == id);
            //Turnery Statement
            return image == null ? NotFound() : File(image.Data!, image.Type!);

        }
    }
}
