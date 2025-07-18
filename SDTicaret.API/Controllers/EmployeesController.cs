using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetAll()
    {
        try
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResult(employees));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<EmployeeDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetById(int id)
    {
        try
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound(ApiResponse<EmployeeDto>.ErrorResult("Çalışan bulunamadı"));

            return Ok(ApiResponse<EmployeeDto>.SuccessResult(employee));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<EmployeeDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create(EmployeeDto dto)
    {
        try
        {
            var employee = await _employeeService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, 
                ApiResponse<EmployeeDto>.SuccessResult(employee, "Çalışan başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<EmployeeDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Update(int id, EmployeeDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<EmployeeDto>.ErrorResult("ID uyumsuzluğu"));

            var employee = await _employeeService.UpdateAsync(dto);
            return Ok(ApiResponse<EmployeeDto>.SuccessResult(employee, "Çalışan başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<EmployeeDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _employeeService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Çalışan bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Çalışan başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 