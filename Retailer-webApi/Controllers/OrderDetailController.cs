using BusinessLayer.IRepository;
using EntityModel.Models;
using Microsoft.AspNetCore.Mvc;


namespace Retailer_webApi.Controllers
{
    /// <summary>
  
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderDetailController> _logger;

        public OrderDetailController(IUnitOfWork unitOfWork, ILogger<OrderDetailController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all orderDetails
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetAllOrderDetail()
        {
            var repository = _unitOfWork.Repository<OrderDetail>();
            var OrderDetail = await repository.GetAllAsync();
            return Ok(OrderDetail);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        {
            var repository = _unitOfWork.Repository<OrderDetail>();
            var orderDetail = await repository.GetByIdAsync(id);

            if (orderDetail == null)
            {
                return NotFound($"orderDetail with ID {id} not found.");
            }

            return Ok(orderDetail);
        }

        /// <summary>
        /// Create a new OrderDetail
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderDetail>> CreateOrderDetail([FromBody] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repository = _unitOfWork.Repository<OrderDetail>();
            
            // Check if orderdetails with same name already exists
            var existingOrderDetail = await repository.FirstOrDefaultAsync(p => p.ID == orderDetail.ID);
            if (existingOrderDetail != null)
            {
                return Conflict($" with name '{orderDetail.OrderId}' already exists.");
            }

            await repository.AddAsync(orderDetail);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.ID }, orderDetail);
        }

        /// <summary>
        /// Update an existing OrderDetail
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetail(int id, [FromBody] OrderDetail orderDetail)
        {
            if (id != orderDetail.ID)
            {
                return BadRequest("Order Detail ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var repository = _unitOfWork.Repository<OrderDetail>();
            var existingOrderDetail = await repository.GetByIdAsync(id);

            if (existingOrderDetail == null)
            {
                return NotFound($"existingOrderDetail with ID {id} not found.");
            }

            // Update properties
            existingOrderDetail.OrderId = existingOrderDetail.OrderId;
            existingOrderDetail.Quantity = existingOrderDetail.Quantity;
            existingOrderDetail.ProductId = existingOrderDetail.ProductId;
           
            repository.Update(existingOrderDetail);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a orderDetail (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var repository = _unitOfWork.Repository<OrderDetail>();
            var deleted = await repository.SoftDeleteAsync(id);

            if (!deleted)
            {
                return NotFound($"Order Detail with ID {id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Search orderDetils by OrderID
        /// </summary>
        [HttpGet("OrderDetail/{OrderDetail}")]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetailByID(int OrderID)
        {
            var repository = _unitOfWork.Repository<OrderDetail>();
            var customer = await repository.FindAsync(p => p.OrderId == OrderID);
            return Ok(customer);
        }

        /// <summary>
        /// Example of using transactions with Unit of Work
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateBulkOrderDetail([FromBody] List<OrderDetail> orderDetails)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var repository = _unitOfWork.Repository<OrderDetail>();

                foreach (var orderDetail in orderDetails)
                {
                    await repository.AddAsync(orderDetail);
                }

                await _unitOfWork.CommitTransactionAsync();
                return Ok($"Successfully created {orderDetails.Count} orderdetails.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk orderdetails");
                return StatusCode(500, "An error occurred while creating orderdetails.");
            }
        }
    }
}
