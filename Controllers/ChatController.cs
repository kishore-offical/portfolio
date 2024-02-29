using ChatWe.Persistance.Context;
using ChatWe.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatWe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public readonly IChatWeContext _context;

        public ChatController(IChatWeContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(string sender, string receiver, string message, int groupId)
        {
            try
            {
                var conversation = new Conversations
                {
                    DateTime = DateTime.Now,
                    Message = message,
                    SenderId = sender,
                    ReceiverId = receiver,
                    GroupId = groupId
                };

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("createGroup")]
        public async Task<IActionResult> CreatePrivateGroup(string senderId, string receiverId)
        {
            try
            {
                // Check if a group already exists for these users
                var existingGroup = _context.Group
                    .FirstOrDefault(g => g.Name == $"{senderId}_{receiverId}" || g.Name == $"{receiverId}_{senderId}");

                if (existingGroup == null)
                {
                    // Create a new group
                    var newGroup = new Group
                    {
                        Name = $"{senderId}_{receiverId}",
                        IsActive = true
                    };

                    _context.Group.Add(newGroup);
                    await _context.SaveChangesAsync();

                    return Ok(newGroup.Id); // Return the ID of the created group
                }

                return Ok(existingGroup.Id); // Return the ID of the existing group
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getMessages")]
        public IActionResult GetGroupMessages(int groupId)
        {
            try
            {
                var messages = _context.Conversations
                    .Where(c => c.GroupId == groupId)
                    .OrderBy(c => c.DateTime)

                    .ToList();
                var data = messages.Select(x => new
                {
                    Sender = x.SenderId == User?.Claims.FirstOrDefault()!.Value ? "You" : _userManager.Users.AsNoTracking().FirstOrDefault(u => u.Id == x.SenderId)?.FirstName,
                    Receiver = x.ReceiverId == User?.Claims.FirstOrDefault()!.Value ? "You" : _userManager.Users.AsNoTracking().FirstOrDefault(u => u.Id == x.ReceiverId)?.FirstName,
                    x.Message,
                    SendOn = x.DateTime.ToString("t")
                });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}