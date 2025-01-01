using Microsoft.AspNetCore.Mvc;

namespace SimpleApiWithCopilot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            try
            {
                // Validation to process only valid user data (Annotations added in User.cs)
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newUser = _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                // Validation to process only valid user data (Annotations added in User.cs)
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (_userService.GetUserByIdAsync(id) == null)
                {
                    return NotFound();
                }

                var updatedUser = _userService.UpdateUserAsync(id, user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                if (_userService.GetUserByIdAsync(id) == null)
                {
                    return NotFound();
                }

                _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}
