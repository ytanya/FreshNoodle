using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreshNoodle.API.Authentication.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Admin")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repository;

    public CustomersController(ICustomerRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _repository.GetAllWithTypesAndInactiveAsync();

        return Ok(customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            CustomerTypeId = c.CustomerTypeId,
            CustomerTypeName = c.CustomerType.Name,
            PriceTypeId = c.PriceTypeId,
            PriceTypeName = c.PriceType.Name,
            PaymentTypeId = c.PaymentTypeId,
            PaymentTypeName = c.PaymentType.Name,
            SkipDay = c.SkipDay,
            PriorityOrder = c.PriorityOrder,
            IsActive = c.IsActive
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            return BadRequest(new { message = "Name is required" });

        var customer = new Customer
        {
            Name = dto.Name,
            CustomerTypeId = dto.CustomerTypeId,
            PriceTypeId = dto.PriceTypeId,
            PaymentTypeId = dto.PaymentTypeId,
            SkipDay = dto.SkipDay,
            PriorityOrder = dto.PriorityOrder,
            IsActive = true
        };

        await _repository.AddAsync(customer);
        return Ok(new { message = "Customer created successfully" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerDto dto)
    {
        var customer = await _repository.GetByIdWithInactiveAsync(id);

        if (customer == null) return NotFound();

        customer.Name = dto.Name;
        customer.CustomerTypeId = dto.CustomerTypeId;
        customer.PriceTypeId = dto.PriceTypeId;
        customer.PaymentTypeId = dto.PaymentTypeId;
        customer.SkipDay = dto.SkipDay;
        customer.PriorityOrder = dto.PriorityOrder;
        customer.IsActive = dto.IsActive;

        await _repository.UpdateAsync(customer);
        return Ok(new { message = "Customer updated successfully" });
    }
}
