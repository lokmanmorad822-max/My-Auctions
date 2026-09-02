using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Phone = u.Phone,
            CreatedAt = u.CreatedAt
        }).ToList();

        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null) return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto createDto, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = createDto.Name,
            Email = createDto.Email,
            Phone = createDto.Phone,
            PasswordHash = createDto.PasswordHash
        };

        var created = await _userService.CreateAsync(user, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new UserDto
        {
            Id = created.Id,
            Name = created.Name,
            Email = created.Email,
            Phone = created.Phone,
            CreatedAt = created.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, UpdateUserDto updateDto, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null) return NotFound();

        user.Name = updateDto.Name;
        user.Email = updateDto.Email;
        user.Phone = updateDto.Phone;

        var updated = await _userService.UpdateAsync(user, cancellationToken);

        return Ok(new UserDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Email = updated.Email,
            Phone = updated.Phone,
            CreatedAt = updated.CreatedAt
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null) return NotFound();

        await _userService.DeleteAsync(user, cancellationToken);
        return NoContent();
    }
}

