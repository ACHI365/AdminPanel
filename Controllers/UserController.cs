using AdminPanel.Model.Dto;
using AdminPanel.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("user-management")]
        public async Task<IActionResult> GetUserManagement()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("block-user")]
        public async Task<IActionResult> BlockUser([FromBody] List<int> userIds)
        {
            var result = await _userService.BlockUserAsync(userIds);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("unblock-user")]
        public async Task<IActionResult> UnblockUser([FromBody] List<int> userIds)
        {
            var result = await _userService.UnblockUserAsync(userIds);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] List<int> userIds)
        {
            var result = await _userService.DeleteUserAsync(userIds);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
    }
}