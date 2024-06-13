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
    public class TicketsController(ITicketDTOService _ticketService,
                                    IProjectDTOService _projectService,
                                    UserManager<ApplicationUser> _userManager) : ControllerBase
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
        [Authorize]
        public async Task<ActionResult<TicketDTO>> AddTicketAsync([FromBody] TicketDTO ticket)
        {
            if (_companyId is null) return BadRequest();
            try
            {

                ticket.SubmitterUserId = _userId;

                if (User.IsInRole(nameof(Roles.ProjectManager)))
                {
                    UserDTO? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId.Value);
                    if (projectManager is null) return BadRequest();
                    if (projectManager.Id != _userId) return BadRequest();
                }

                if (User.IsInRole(nameof(Roles.Developer)) || User.IsInRole(nameof(Roles.Developer.Submitter)))
                {
                    IEnumerable<UserDTO?> projectMembers = await _projectService.GetProjectMembersAsync(ticket.ProjectId, _companyId.Value);
                    if (!projectMembers.Any()) return BadRequest();
                    if (projectMembers.Any(m => m.Id == _userId))
                    {
                        TicketDTO addedTicket = await _ticketService.AddTicketAsync(ticket, _companyId.Value);
                        return Ok(addedTicket);
                    }
                }

                TicketDTO ticketDTO = await _ticketService.AddTicketAsync(ticket, _companyId.Value);
                return Ok(ticketDTO);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("{ticketId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateTicketAsync([FromRoute] int ticketId, [FromBody] TicketDTO ticket)
        {
            if (_companyId is null) return BadRequest();
            if (ticketId != ticket.Id) return BadRequest();

            try
            {
                //Project manager is assigned to the project that has this ticket and that PM is the user
                if (User.IsInRole(nameof(Roles.ProjectManager)))
                {
                    //get project manager from DB to check
                    UserDTO? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId.Value);

                    //If they arent assigned kick them out of request else continue out of if statement
                    if (projectManager is null) return BadRequest();
                    if (projectManager.Id != _userId) return BadRequest();
                }

                //get ticket so you have all the other members
                TicketDTO? ticketToUpdate = await _ticketService.GetTicketByIdAsync(ticketId, _companyId!.Value);

                // if you are a developer you must have made ticket or be assigned to the ticket
                if (User.IsInRole(nameof(Roles.Developer)))
                {
                    if (ticketToUpdate?.DeveloperUserId != _userId && ticketToUpdate?.SubmitterUserId != _userId) return BadRequest();

                    //not allowed to update developer assigned to ticket
                    if (ticketToUpdate.DeveloperUserId != ticket.DeveloperUserId) return BadRequest();
                }

                // if you are a submitter you must have made the ticket
                if (User.IsInRole(nameof(Roles.Submitter)))
                {
                    if (ticketToUpdate?.SubmitterUserId != _userId) return BadRequest();

                    //not allowed to update developer assigned to ticket
                    if (ticketToUpdate.DeveloperUserId != ticket.DeveloperUserId) return BadRequest();
                }


                // if you make it this far you are a valid appUser to edit the ticket
                await _ticketService.UpdateTicketAsync(ticket, _companyId.Value, _userId);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("archive/{ticketId:int}")]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> ArchiveTicketAsync([FromRoute] int ticketId, [FromBody] int ticketIdFromBody)
        {
            if (ticketId != ticketIdFromBody) return BadRequest();
            if (_companyId is null) return BadRequest();

            try
            {
                if (User.IsInRole(nameof(Roles.ProjectManager)))
                {
                    TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(ticketIdFromBody, _companyId.Value);
                    if (ticket == null) return BadRequest();

                    UserDTO? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId.Value);
                    if (projectManager is null) return BadRequest();
                    if (projectManager.Id != _userId) return BadRequest();
                }

                await _ticketService.ArchiveTicketAsync(ticketId, _companyId.Value);
                return Ok();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("restore/{ticketId:int}")]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> RestoreTicketAsync([FromRoute] int ticketId, [FromBody] int ticketIdFromBody)
        {
            if (ticketId != ticketIdFromBody) return BadRequest();
            if (_companyId is null) return BadRequest();

            try
            {
                if (User.IsInRole(nameof(Roles.ProjectManager)))
                {
                    TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(ticketIdFromBody, _companyId.Value);
                        if (ticket == null) return BadRequest();

                    UserDTO? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId.Value);
                        if (projectManager is null) return BadRequest();
                        if (projectManager.Id != _userId) return BadRequest();
                }

                await _ticketService.RestoreTicketAsync(ticketId, _companyId.Value);
                return Ok();

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
            if (commentId != comment.Id) return BadRequest();
            if (_companyId is null) return BadRequest();

            try
            {
                TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(comment.TicketId, _companyId.Value);
                if (ticket == null) return BadRequest();

                //Project manager is assigned to the project that has this ticket and that PM is the user
                if (User.IsInRole(nameof(Roles.ProjectManager)))
                {
                    //get project manager from DB to check
                    UserDTO? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId.Value);

                    //If they arent assigned kick them out of request else continue out of if statement
                    if (projectManager is null) return BadRequest();
                    if (projectManager.Id != _userId) return BadRequest();
                }

                // if you are a developer you must have made ticket or be assigned to the ticket
                if (User.IsInRole(nameof(Roles.Developer)))
                {
                    if (ticket?.DeveloperUserId != _userId && ticket?.SubmitterUserId != _userId) return BadRequest();
                }

                // if you are a submitter you must have made the ticket
                if (User.IsInRole(nameof(Roles.Submitter)))
                {
                    if (ticket?.SubmitterUserId != _userId) return BadRequest();
                }

                await _ticketService.AddCommentAsync(comment, _companyId.Value);
                return Ok();
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
            if (_companyId is null) return BadRequest();

            try
            {
                TicketCommentDTO? comment = await _ticketService.GetCommentByIdAsync(commentId, _companyId.Value);
                if (comment == null) return BadRequest();

                TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(comment.TicketId, _companyId.Value);
                if (ticket == null) return BadRequest();

                //Project manager is assigned to the project that has this ticket and that PM is the user
                if (User.IsInRole(nameof(Roles.ProjectManager)))
                {
                    //get project manager from DB to check
                    UserDTO? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId.Value);

                    //If they arent assigned kick them out of request else continue out of if statement
                    if (projectManager is null) return BadRequest();
                    if (projectManager.Id != _userId) return BadRequest();
                }

                // if you are a developer you must have made ticket or be assigned to the ticket
                if (User.IsInRole(nameof(Roles.Developer)))
                {
                    if (ticket?.DeveloperUserId != _userId && ticket?.SubmitterUserId != _userId) return BadRequest();
                }

                // if you are a submitter you must have made the ticket
                if (User.IsInRole(nameof(Roles.Submitter)))
                {
                    if (ticket?.SubmitterUserId != _userId) return BadRequest();
                }


                await _ticketService.DeleteCommentAsync(commentId, _companyId.Value);
                return Ok();
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
            if (commentId != comment.Id) return BadRequest();
            if (_companyId is null) return BadRequest();
            try
            {
                TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(comment.TicketId, _companyId.Value);
                if (ticket == null) return BadRequest();

                //Project manager is assigned to the project that has this ticket and that PM is the user
                if (User.IsInRole(nameof(Roles.ProjectManager)))
                {
                    //get project manager from DB to check
                    UserDTO? projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId.Value);

                    //If they arent assigned kick them out of request else continue out of if statement
                    if (projectManager is null) return BadRequest();
                    if (projectManager.Id != _userId) return BadRequest();
                }

                // if you are a developer you must have made ticket or be assigned to the ticket
                if (User.IsInRole(nameof(Roles.Developer)))
                {
                    if (ticket?.DeveloperUserId != _userId && ticket?.SubmitterUserId != _userId) return BadRequest();
                }

                // if you are a submitter you must have made the ticket
                if (User.IsInRole(nameof(Roles.Submitter)))
                {
                    if (ticket?.SubmitterUserId != _userId) return BadRequest();
                }
                await _ticketService.UpdateCommentAsync(comment, _companyId.Value, _userId);
                return Ok();

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
        [Authorize]
        public async Task<ActionResult<TicketAttachmentDTO>> PostTicketAttachment(int ticketId,
                                                                                    [FromForm] TicketAttachmentDTO attachment,
                                                                                    [FromForm] IFormFile? file)
        {
            if (attachment.TicketId != ticketId || file is null)
            {
                return BadRequest();
            }
            try
            {
                TicketDTO? ticketdto = await _ticketService.GetTicketByIdAsync(ticketId, _companyId!.Value);
                if (ticketdto == null) return BadRequest();

                if (User.IsInRole(nameof(Roles.ProjectManager)))
                {
                    UserDTO? projectManager = await _projectService.GetProjectManagerAsync(ticketdto.ProjectId, _companyId.Value);
                    if (projectManager is null) return BadRequest();
                    if (projectManager.Id != _userId) return BadRequest();
                }

                if (User.IsInRole(nameof(Roles.Developer)) || User.IsInRole(nameof(Roles.Developer.Submitter)))
                {
                    IEnumerable<UserDTO?> projectMembers = await _projectService.GetProjectMembersAsync(ticketdto.ProjectId, _companyId.Value);
                    if (!projectMembers.Any()) return BadRequest();
                    if (!projectMembers.Any(m => m?.Id == _userId)) return BadRequest();
                }
            }
            catch (Exception)
            {

                throw;
            }

            var user = await _userManager.GetUserAsync(User);
            var ticket = await _ticketService.GetTicketByIdAsync(ticketId, user!.CompanyId);

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
        [Authorize]
        public async Task<IActionResult> DeleteTicketAttachment(int attachmentId)
        {
            if (_companyId is null) return BadRequest();

            var user = await _userManager.GetUserAsync(User);

            try
            {
                TicketAttachmentDTO? attachment = await _ticketService.GetTicketAttachmentByIdAsync(attachmentId, _companyId.Value);
                    if (attachment is null) return BadRequest();

                    if (_userId != attachment.UserId) return BadRequest();
                await _ticketService.DeleteTicketAttachment(attachmentId, user!.CompanyId);

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return NoContent();
        }
    }
}
