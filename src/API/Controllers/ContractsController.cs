namespace API.Controllers;

using Application.DTOs.Contract;
using Application.Interfaces;

using API.Models;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;
    private readonly ILogger<ContractsController> _logger;

    public ContractsController(
        IContractService contractService,
        ILogger<ContractsController> logger)
    {
        _contractService = contractService;
        _logger = logger;
    }

    /// <summary>
    /// Get all contracts
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContractDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContractDto>>>> GetAll()
    {
        try
        {
            var contracts = await _contractService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ContractDto>>.SuccessResponse(contracts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all contracts");
            return StatusCode(500, ApiResponse<IEnumerable<ContractDto>>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Get active contracts only
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContractDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContractDto>>>> GetActive()
    {
        try
        {
            var contracts = await _contractService.GetActiveContractsAsync();
            return Ok(ApiResponse<IEnumerable<ContractDto>>.SuccessResponse(contracts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active contracts");
            return StatusCode(500, ApiResponse<IEnumerable<ContractDto>>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Get contract by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ContractDto>>> GetById(Guid id)
    {
        try
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null)
                return NotFound(ApiResponse<ContractDto>.ErrorResponse("Contract not found"));

            return Ok(ApiResponse<ContractDto>.SuccessResponse(contract));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract {ContractId}", id);
            return StatusCode(500, ApiResponse<ContractDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Get contract by contract number
    /// </summary>
    [HttpGet("by-number/{contractNumber}")]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ContractDto>>> GetByNumber(string contractNumber)
    {
        try
        {
            var contract = await _contractService.GetByContractNumberAsync(contractNumber);
            if (contract == null)
                return NotFound(ApiResponse<ContractDto>.ErrorResponse("Contract not found"));

            return Ok(ApiResponse<ContractDto>.SuccessResponse(contract));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract {ContractNumber}", contractNumber);
            return StatusCode(500, ApiResponse<ContractDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Create a new contract
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ContractDto>>> Create([FromBody] CreateContractDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ContractDto>.ErrorResponse("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var contract = await _contractService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = contract.Id },
                ApiResponse<ContractDto>.SuccessResponse(contract, "Contract created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while creating contract");
            return BadRequest(ApiResponse<ContractDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contract");
            return StatusCode(500, ApiResponse<ContractDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Update contract information
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ContractDto>>> Update(Guid id, [FromBody] UpdateContractDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ContractDto>.ErrorResponse("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var contract = await _contractService.UpdateAsync(id, dto);
            return Ok(ApiResponse<ContractDto>.SuccessResponse(contract, "Contract updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Contract not found or business rule violation");
            return NotFound(ApiResponse<ContractDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contract {ContractId}", id);
            return StatusCode(500, ApiResponse<ContractDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Terminate a contract
    /// </summary>
    [HttpPost("{id:guid}/terminate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Terminate(Guid id, [FromBody] DateTime endDate)
    {
        try
        {
            var result = await _contractService.TerminateAsync(id, endDate);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Contract not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Contract terminated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while terminating contract");
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating contract {ContractId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Suspend a contract
    /// </summary>
    [HttpPost("{id:guid}/suspend")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Suspend(Guid id)
    {
        try
        {
            var result = await _contractService.SuspendAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Contract not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Contract suspended successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while suspending contract");
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suspending contract {ContractId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Reactivate a suspended contract
    /// </summary>
    [HttpPost("{id:guid}/reactivate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Reactivate(Guid id)
    {
        try
        {
            var result = await _contractService.ReactivateAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Contract not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Contract reactivated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while reactivating contract");
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating contract {ContractId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
        }
    }
}