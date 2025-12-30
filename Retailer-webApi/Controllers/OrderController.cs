using BusinessLayer.IRepository;
using EntityModel.Models;
using Microsoft.AspNetCore.Mvc;


namespace Retailer_webApi.Controllers
{
    /// <summary>
  
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IUnitOfWork unitOfWork, ILogger<OrderController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            var repository = _unitOfWork.Repository<Order>();
            var orders = await repository.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var repository = _unitOfWork.Repository<Order>();
            var Order = await repository.GetByIdAsync(id);

            if (Order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok(Order);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Order>();
            
            // Check if order with same name already exists
            var existingOrder = await repository.FirstOrDefaultAsync(p => p.ID == order.ID);
            if (existingOrder != null)
            {
                return Conflict($" with ID '{order.ID}' already exists.");
            }

            ////existingOrder.CustomerId = order.CustomerId;
            //existingOrder.EmployeeId = order.EmployeeId;
            //existingOrder.TotalAmount = order.TotalAmount;
            //existingOrder.Date = order.Date;


            await repository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.ID }, order);
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.ID)
            {
                return BadRequest("order ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<Order>();
            var existingOrder = await repository.GetByIdAsync(id);

            if (existingOrder == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            // Update properties
            existingOrder.CustomerId = order.CustomerId;
            existingOrder.EmployeeId = order.EmployeeId;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.Date = order.Date;

            repository.Update(existingOrder);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a Order (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var repository = _unitOfWork.Repository<Order>();
            var deleted = await repository.SoftDeleteAsync(id);

            if (!deleted)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

       

        /// <summary>
        /// Example of using transactions with Unit of Work
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateBulkOrder([FromBody] List<Order> orders)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var repository = _unitOfWork.Repository<Order>();

                foreach (var order in orders)
                {
                    await repository.AddAsync(order);
                }

                await _unitOfWork.CommitTransactionAsync();
                return Ok($"Successfully created {orders.Count} Order.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk Order");
                return StatusCode(500, "An error occurred while creating Order.");
            }
        }
    }
}
