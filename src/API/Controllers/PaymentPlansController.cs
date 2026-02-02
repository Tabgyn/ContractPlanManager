namespace API.Controllers;

using Application.DTOs.PaymentPlan;
using Application.Interfaces;

using API.Models;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PaymentPlansController : ControllerBase
{
    private readonly IPaymentPlanService _paymentPlanService;
    private readonly ILogger<PaymentPlansController> _logger;

    public PaymentPlansController(
        IPaymentPlanService paymentPlanService,
        ILogger<PaymentPlansController> logger)
    {
        _paymentPlanService = paymentPlanService;
        _logger = logger;
    }

    /// <summary>
    /// Get all payment plans
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentPlanDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentPlanDto>>>> GetAll()
    {
        try
        {
            var plans = await _paymentPlanService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<PaymentPlanDto>>.SuccessResponse(plans));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all payment plans");
            return StatusCode(500, ApiResponse<IEnumerable<PaymentPlanDto>>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Get active payment plans only
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentPlanDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentPlanDto>>>> GetActive()
    {
        try
        {
            var plans = await _paymentPlanService.GetActivePlansAsync();
            return Ok(ApiResponse<IEnumerable<PaymentPlanDto>>.SuccessResponse(plans));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active payment plans");
            return StatusCode(500, ApiResponse<IEnumerable<PaymentPlanDto>>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Get payment plan by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaymentPlanDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PaymentPlanDto>>> GetById(Guid id)
    {
        try
        {
            var plan = await _paymentPlanService.GetByIdAsync(id);
            if (plan == null)
                return NotFound(ApiResponse<PaymentPlanDto>.ErrorResponse("Payment plan not found"));

            return Ok(ApiResponse<PaymentPlanDto>.SuccessResponse(plan));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment plan {PlanId}", id);
            return StatusCode(500, ApiResponse<PaymentPlanDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Create a new payment plan
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentPlanDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PaymentPlanDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PaymentPlanDto>>> Create([FromBody] CreatePaymentPlanDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<PaymentPlanDto>.ErrorResponse("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var plan = await _paymentPlanService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = plan.Id },
                ApiResponse<PaymentPlanDto>.SuccessResponse(plan, "Payment plan created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment plan");
            return StatusCode(500, ApiResponse<PaymentPlanDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Update payment plan pricing and description
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaymentPlanDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<PaymentPlanDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PaymentPlanDto>>> Update(Guid id, [FromBody] UpdatePaymentPlanDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<PaymentPlanDto>.ErrorResponse("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var plan = await _paymentPlanService.UpdateAsync(id, dto);
            return Ok(ApiResponse<PaymentPlanDto>.SuccessResponse(plan, "Payment plan updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Payment plan not found");
            return NotFound(ApiResponse<PaymentPlanDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment plan {PlanId}", id);
            return StatusCode(500, ApiResponse<PaymentPlanDto>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Deactivate a payment plan
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(Guid id)
    {
        try
        {
            var result = await _paymentPlanService.DeactivateAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Payment plan not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Payment plan deactivated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating payment plan {PlanId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
        }
    }

    /// <summary>
    /// Reactivate a payment plan
    /// </summary>
    [HttpPost("{id:guid}/reactivate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Reactivate(Guid id)
    {
        try
        {
            var result = await _paymentPlanService.ReactivateAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Payment plan not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Payment plan reactivated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating payment plan {PlanId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
        }
    }
}