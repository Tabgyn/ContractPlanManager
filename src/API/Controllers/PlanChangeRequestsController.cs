namespace API.Controllers;

using Application.DTOs.PlanChangeRequest;
using Application.Interfaces;

using API.Models;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PlanChangeRequestsController : ControllerBase
{
    private readonly IPlanChangeRequestService _changeRequestService;
    private readonly ILogger<PlanChangeRequestsController> _logger;

    public PlanChangeRequestsController(
        IPlanChangeRequestService changeRequestService,
        ILogger<PlanChangeRequestsController> logger)
    {
        _changeRequestService = changeRequestService;
        _logger = logger;
    }

    /// <summary>
    /// Get all pending plan change requests
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PlanChangeRequestDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PlanChangeRequestDto>>>> GetPending()
    {
        try
        {
            var requests = await _changeRequestService.GetPendingRequestsAsync();
            return Ok(ApiResponse<IEnumerable<PlanChangeRequestDto>>.SuccessResponse(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending change requests");
            return StatusCode(500, ApiResponse<IEnumerable<PlanChangeRequestDto>>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Get plan change request by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PlanChangeRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PlanChangeRequestDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PlanChangeRequestDto>>> GetById(Guid id)
    {
        try
        {
            var request = await _changeRequestService.GetByIdAsync(id);
            if (request == null)
                return NotFound(ApiResponse<PlanChangeRequestDto>.ErrorResponse("Change request not found"));

            return Ok(ApiResponse<PlanChangeRequestDto>.SuccessResponse(request));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting change request {RequestId}", id);
            return StatusCode(500, ApiResponse<PlanChangeRequestDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Get all plan change requests for a contract
    /// </summary>
    [HttpGet("contract/{contractId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PlanChangeRequestDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PlanChangeRequestDto>>>> GetByContractId(Guid contractId)
    {
        try
        {
            var requests = await _changeRequestService.GetByContractIdAsync(contractId);
            return Ok(ApiResponse<IEnumerable<PlanChangeRequestDto>>.SuccessResponse(requests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting change requests for contract {ContractId}", contractId);
            return StatusCode(500, ApiResponse<IEnumerable<PlanChangeRequestDto>>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Create a new plan change request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PlanChangeRequestDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PlanChangeRequestDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PlanChangeRequestDto>>> Create([FromBody] CreatePlanChangeRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<PlanChangeRequestDto>.ErrorResponse("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var request = await _changeRequestService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = request.Id },
                ApiResponse<PlanChangeRequestDto>.SuccessResponse(request, "Plan change request created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while creating change request");
            return BadRequest(ApiResponse<PlanChangeRequestDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plan change request");
            return StatusCode(500, ApiResponse<PlanChangeRequestDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Process (approve or reject) a plan change request
    /// </summary>
    [HttpPost("{id:guid}/process")]
    [ProducesResponseType(typeof(ApiResponse<PlanChangeRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PlanChangeRequestDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<PlanChangeRequestDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PlanChangeRequestDto>>> Process(Guid id, [FromBody] ProcessPlanChangeRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<PlanChangeRequestDto>.ErrorResponse("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var request = await _changeRequestService.ProcessAsync(id, dto);
            var message = dto.Approved ? "Plan change request approved" : "Plan change request rejected";
            return Ok(ApiResponse<PlanChangeRequestDto>.SuccessResponse(request, message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while processing change request");
            return BadRequest(ApiResponse<PlanChangeRequestDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing change request {RequestId}", id);
            return StatusCode(500, ApiResponse<PlanChangeRequestDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Cancel a pending plan change request
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(Guid id)
    {
        try
        {
            var result = await _changeRequestService.CancelAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Change request not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Plan change request cancelled successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while cancelling change request");
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling change request {RequestId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
        }
    }
}