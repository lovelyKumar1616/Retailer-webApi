using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.ViewModel
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public long TotalAmouunt { get; set; }
    }
}
