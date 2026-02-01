using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshNoodle.API.Authentication.Controllers;

[ApiController]
[Route("api/payment-types")]
[Authorize(Roles = "Admin")]
public class PaymentTypeController : ControllerBase
{
    private readonly IRepository<PaymentType> _repository;

    public PaymentTypeController(IRepository<PaymentType> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await _repository.GetAllWithInactiveAsync();

        return Ok(types.Select(t => new PaymentTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            IsActive = t.IsActive
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PaymentTypeDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            return BadRequest(new { message = "Name is required" });

        var type = new PaymentType
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true
        };

        await _repository.AddAsync(type);
        return Ok(new PaymentTypeDto
        {
            Id = type.Id,
            Name = type.Name,
            Description = type.Description,
            IsActive = type.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PaymentTypeDto dto)
    {
        var type = await _repository.GetByIdWithInactiveAsync(id);

        if (type == null) return NotFound();

        type.Name = dto.Name;
        type.Description = dto.Description;
        type.IsActive = dto.IsActive;

        await _repository.UpdateAsync(type);
        return Ok(new PaymentTypeDto
        {
            Id = type.Id,
            Name = type.Name,
            Description = type.Description,
            IsActive = type.IsActive
        });
    }
}
