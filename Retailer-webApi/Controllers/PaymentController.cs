using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Retailer_webApi.Controllers
{
    using BusinessLayer.IRepository;
    using EntityModel.Models;
    using Microsoft.AspNetCore.Mvc;


    namespace Retailer_webApi.Controllers
    {
        /// <summary>

        /// </summary>
        [ApiController]
        [Route("api/[controller]")]
        public class PaymentController : ControllerBase
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<PaymentController> _logger;

            public PaymentController(IUnitOfWork unitOfWork, ILogger<PaymentController> logger)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
            }

            /// <summary>
            /// Get all payments
            /// </summary>
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayment()
            {
                var repository = _unitOfWork.Repository<Payment>();
                var employees = await repository.GetAllAsync();
                return Ok(employees);
            }

            /// <summary>
            /// Get payment by ID
            /// </summary>
            [HttpGet("{id}")]
            public async Task<ActionResult<Payment>> GetPayment(int id)
            {
                var repository = _unitOfWork.Repository<Payment>();
                var Employee = await repository.GetByIdAsync(id);

                if (Employee == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }

                return Ok(Employee);
            }

            /// <summary>
            /// Create a new payment
            /// </summary>
            [HttpPost]
            public async Task<ActionResult<Payment>> CreatePayment([FromBody] Payment payment)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repository = _unitOfWork.Repository<Payment>();

                // Check if Payment with same name already exists
                var existingCustomer = await repository.FirstOrDefaultAsync(p => p.ID == payment.ID);
                if (existingCustomer != null)
                {
                    return Conflict($" with name '{payment.ID}' already exists.");
                }

                await repository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPayment), new { id = payment.ID }, payment);
            }

            /// <summary>
            /// Update an existing Payment
            /// </summary>
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment payment)
            {
                if (id != payment.ID)
                {
                    return BadRequest("payment ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repository = _unitOfWork.Repository<Payment>();
                var existingPayment = await repository.GetByIdAsync(id);

                if (existingPayment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }

                // Update properties
                existingPayment.ID = payment.ID;
               
                existingPayment.PaymentType = payment.PaymentType;
                existingPayment.OrderId = payment.OrderId;
                existingPayment.Date = payment.Date;
                existingPayment.Amount = payment.Amount;
                repository.Update(existingPayment);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }

            /// <summary>
            /// Delete a Payment (soft delete)
            /// </summary>
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeletePayment(int id)
            {
                var repository = _unitOfWork.Repository<Employee>();
                var deleted = await repository.SoftDeleteAsync(id);

                if (!deleted)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }

                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }

            

            /// <summary>
            /// Example of using transactions with Unit of Work
            /// </summary>
            [HttpPost("bulk")]
            public async Task<ActionResult> CreateBulkEmployees([FromBody] List<Employee> employees)
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    var repository = _unitOfWork.Repository<Employee>();

                    foreach (var employee in employees)
                    {
                        await repository.AddAsync(employee);
                    }

                    await _unitOfWork.CommitTransactionAsync();
                    return Ok($"Successfully created {employees.Count} Employee.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating bulk Employee");
                    return StatusCode(500, "An error occurred while creating Employee.");
                }
            }
        }
    }

}
