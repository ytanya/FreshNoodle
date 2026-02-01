using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshNoodle.API.Authentication.Controllers;

[ApiController]
[Route("api/price-types")]
[Authorize(Roles = "Admin")]
public class PriceTypeController : ControllerBase
{
    private readonly IRepository<PriceType> _repository;

    public PriceTypeController(IRepository<PriceType> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await _repository.GetAllWithInactiveAsync();

        return Ok(types.Select(t => new PriceTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            DefaultPricePerKg = t.DefaultPricePerKg,
            IsActive = t.IsActive
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PriceTypeDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            return BadRequest(new { message = "Name is required" });

        var type = new PriceType
        {
            Name = dto.Name,
            DefaultPricePerKg = dto.DefaultPricePerKg,
            IsActive = true
        };

        await _repository.AddAsync(type);
        return Ok(new PriceTypeDto
        {
            Id = type.Id,
            Name = type.Name,
            DefaultPricePerKg = type.DefaultPricePerKg,
            IsActive = type.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PriceTypeDto dto)
    {
        var type = await _repository.GetByIdWithInactiveAsync(id);

        if (type == null) return NotFound();

        type.Name = dto.Name;
        type.DefaultPricePerKg = dto.DefaultPricePerKg;
        type.IsActive = dto.IsActive;

        await _repository.UpdateAsync(type);
        return Ok(new PriceTypeDto
        {
            Id = type.Id,
            Name = type.Name,
            DefaultPricePerKg = type.DefaultPricePerKg,
            IsActive = type.IsActive
        });
    }
}
