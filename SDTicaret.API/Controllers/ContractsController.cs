using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContractDto>>>> GetAll()
    {
        try
        {
            var contracts = await _contractService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ContractDto>>.SuccessResult(contracts));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ContractDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ContractDto>>> GetById(int id)
    {
        try
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null)
                return NotFound(ApiResponse<ContractDto>.ErrorResult("Sözleşme bulunamadı"));

            return Ok(ApiResponse<ContractDto>.SuccessResult(contract));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ContractDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContractDto>>> Create(ContractDto dto)
    {
        try
        {
            var contract = await _contractService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, 
                ApiResponse<ContractDto>.SuccessResult(contract, "Sözleşme başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ContractDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ContractDto>>> Update(int id, ContractDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<ContractDto>.ErrorResult("ID uyumsuzluğu"));

            var contract = await _contractService.UpdateAsync(dto);
            return Ok(ApiResponse<ContractDto>.SuccessResult(contract, "Sözleşme başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ContractDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _contractService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Sözleşme bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Sözleşme başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 