using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using NuGet.Protocol.Core.Types;
using System.Net.Sockets;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using Tasket.Data;
using Tasket.Helper;
using Tasket.Models;

namespace Tasket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController(ITicketDTOService _ticketService, UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        private string _userId => _userManager.GetUserId(User)!;
        private int? _companyId => User.FindFirst("CompanyId") != null ? int.Parse(User.FindFirst("CompanyId")!.Value) : null;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAllTicketsAsync()
        {
            try
            {
                IEnumerable<TicketDTO> tickets = await _ticketService.GetAllTicketsAsync(_companyId!.Value);

                if (tickets == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(tickets);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("assignments")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetUserTicketsAsync()
        {
            try
            {
                IEnumerable<TicketDTO> tickets = await _ticketService.GetUserTicketsAsync(_companyId!.Value, _userId);

                if (tickets == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(tickets);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{ticketId:int}")]
        public async Task<ActionResult<TicketDTO?>> GetTicketByIdAsync([FromRoute] int ticketId)
        {
            try
            {
                TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(ticketId, _companyId!.Value);

                if (ticket == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(ticket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<TicketDTO>> AddTicketAsync([FromBody] TicketDTO ticket)
        {
            try
            {
                if (_companyId is not null)
                {
                    TicketDTO ticketDTO = await _ticketService.AddTicketAsync(ticket, _companyId.Value);
                    return Ok(ticketDTO);
                }
                else 
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("{ticketId:int}")]
        public async Task<IActionResult> UpdateTicketAsync([FromRoute] int ticketId, [FromBody] TicketDTO ticket)
        {
            try
            {
                if (_companyId is not null && ticketId == ticket.Id)
                {
                    await _ticketService.UpdateTicketAsync(ticket, _companyId.Value, _userId);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("archive/{ticketId:int}")]
        public async Task<IActionResult> ArchiveTicketAsync([FromRoute] int ticketId, [FromBody]  int ticketIdFromBody)
        {
            try
            {
                if (_companyId is not null && ticketId == ticketIdFromBody)
                {
                    await _ticketService.ArchiveTicketAsync(ticketId, _companyId.Value);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("restore/{ticketId:int}")]
        public async Task<IActionResult> RestoreTicketAsync([FromRoute] int ticketId, [FromBody] int ticketIdFromBody)
        {
            try
            {
                if (_companyId is not null)
                {
                    await _ticketService.RestoreTicketAsync(ticketId, _companyId.Value);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }





        [HttpPost("comments/{commentId:int}")]
        public async Task<IActionResult> AddCommentAsync([FromRoute] int commentId, [FromBody] TicketCommentDTO comment)
        {
            try
            {
                if (comment.Id == commentId && _companyId is not null)
                {
                    await _ticketService.AddCommentAsync(comment, _companyId.Value);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }


        [HttpDelete("comments/{commentId:int}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int commentId)
        {
            try
            {
                if (_companyId is not null)
                {
                    await _ticketService.DeleteCommentAsync(commentId, _companyId.Value);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }


        [HttpPut("comments/{commentId:int}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] int commentId, [FromBody] TicketCommentDTO comment)
        {
            try
            {
                if (_companyId is not null && commentId == comment.Id)
                {
                    await _ticketService.UpdateCommentAsync(comment, _companyId.Value, _userId);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }


        [HttpGet("comments/{commentId:int}")]
        public async Task<ActionResult<TicketCommentDTO?>> GetCommentByIdAsync([FromRoute] int ticketCommentId)
        {
            try
            {
                TicketCommentDTO? comments = await _ticketService.GetCommentByIdAsync(ticketCommentId, _companyId!.Value);

                if (comments == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(comments);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }


        [HttpGet("{ticketId:int}/comments")]
        public async Task<ActionResult<IEnumerable<TicketCommentDTO>>> GetTicketCommentsAsync([FromRoute] int ticketId)
        {
            try
            {
                IEnumerable<TicketCommentDTO> comments = await _ticketService.GetTicketCommentsAsync(ticketId, _companyId!.Value);

                if (comments == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(comments);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }







        // POST: api/Tickets/5/attachments
        // NOTE: the parameters are decorated with [FromForm] because they will be sent
        // encoded as multipart/form-data and NOT the typical JSON
        [HttpPost("{id}/attachments")]
        public async Task<ActionResult<TicketAttachmentDTO>> PostTicketAttachment(int id,
                                                                                    [FromForm] TicketAttachmentDTO attachment,
                                                                                    [FromForm] IFormFile? file)
        {
            if (attachment.TicketId != id || file is null)
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User);
            var ticket = await _ticketService.GetTicketByIdAsync(id, user!.CompanyId);

            if (ticket is null)
            {
                return NotFound();
            }

            attachment.UserId = user!.Id;
            attachment.Created = DateTimeOffset.Now;

            if (string.IsNullOrWhiteSpace(attachment.FileName))
            {
                attachment.FileName = file.FileName;
            }

            // ImageHelper was renamed to UploadHelper!
            FileUpload upload = await UploadHelper.GetFileUploadAsync(file);

            try
            {
                var newAttachment = await _ticketService.AddTicketAttachment(attachment, upload.Data!, upload.Type!, user!.CompanyId);
                return Ok(newAttachment);
            }
            catch
            {
                return Problem();
            }
        }

        // DELETE: api/Tickets/attachments/1
        [HttpDelete("attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteTicketAttachment(int attachmentId)
        {
            var user = await _userManager.GetUserAsync(User);

            await _ticketService.DeleteTicketAttachment(attachmentId, user!.CompanyId);

            return NoContent();
        }
    }
}
