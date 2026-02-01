using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshNoodle.API.Authentication.Controllers;

[ApiController]
[Route("api/customer-types")]
[Authorize(Roles = "Admin")]
public class CustomerTypeController : ControllerBase
{
    private readonly IRepository<CustomerType> _repository;

    public CustomerTypeController(IRepository<CustomerType> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await _repository.GetAllWithInactiveAsync();

        return Ok(types.Select(t => new CustomerTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            IsActive = t.IsActive
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerTypeDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            return BadRequest(new { message = "Name is required" });

        var type = new CustomerType
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true
        };

        await _repository.AddAsync(type);
        return Ok(new CustomerTypeDto
        {
            Id = type.Id,
            Name = type.Name,
            Description = type.Description,
            IsActive = type.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerTypeDto dto)
    {
        var type = await _repository.GetByIdWithInactiveAsync(id);

        if (type == null) return NotFound();

        type.Name = dto.Name;
        type.Description = dto.Description;
        type.IsActive = dto.IsActive;

        await _repository.UpdateAsync(type);
        return Ok(new CustomerTypeDto
        {
            Id = type.Id,
            Name = type.Name,
            Description = type.Description,
            IsActive = type.IsActive
        });
    }
}
