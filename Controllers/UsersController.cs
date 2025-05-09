using AtonTask.DAL.Repositories;
using AtonTask.Models.InputModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    private string _сurrentUserLogin => 
    User.Claims.FirstOrDefault(c => c.Type == "Login")?.Value 
    ?? throw new UnauthorizedAccessException("Login claim not found");

    public UsersController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
    {
        try 
        {
            await _userRepo.CreateUserAsync(dto, _сurrentUserLogin);
            return CreatedAtAction(nameof(CreateUser), dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{login}")]
    [Authorize(Policy = "AdminOrSelf")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UserUpdateDto dto, string login)
    {
        return await this.ExecuteHelper(
            _userRepo.UpdateUserAsync(login, _сurrentUserLogin, dto),
            NoContent()
        );
    }

    [HttpPut("{login}/password/{newPassword}")]
    [Authorize(Policy = "AdminOrSelf")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePassword(string login, string newPassword)
    {
        return await this.ExecuteHelper(
            _userRepo.UpdatePasswordAsync(login, _сurrentUserLogin, newPassword),
            NoContent()
        );
    }

    [HttpPut("{login}/login/{newLogin}")]
    [Authorize(Policy = "AdminOrSelf")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLogin(string login, string newLogin)
    {
        return await this.ExecuteHelper(
            _userRepo.UpdateLoginAsync(login, _сurrentUserLogin, newLogin),
            NoContent()
        );
    }

    [HttpGet("active")]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetActiveUsers()
    {
        var users = await _userRepo.GetActiveUsersAsync();
        return Ok(users);
    }

    [HttpGet("{login}")]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByLogin(string login)
    {
        var user = await _userRepo.GetUserDtoByLoginAsync(login);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("self")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userRepo.GetUserByLoginAsync(_сurrentUserLogin);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("older-than/{age}")]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsersOlderThan(int age)
    {
        var users = await _userRepo.GetUsersOlderThanAsync(age);
        return Ok(users);
    }

    [HttpDelete("{login}/soft")]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUserSoft(string login)
    {
        return await ExecuteHelper(
            _userRepo.SoftDeleteUserAsync(login, _сurrentUserLogin),
            NoContent());
    }

    [HttpDelete("{login}/hard")]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUserHard(string login)
    {
        return await ExecuteHelper(
            _userRepo.HardDeleteUserAsync(login),
            NoContent()); 
    }

    [HttpPut("{login}/restore")]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RestoreUser(string login)
    {
        return await ExecuteHelper(
            _userRepo.RestoreUserAsync(login),
            NoContent());
    }

    private async Task<IActionResult> ExecuteHelper(Task todo, IActionResult complete)
    {
        try
        {
            await todo;
            return complete;
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}