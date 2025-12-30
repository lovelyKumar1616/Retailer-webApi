using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.ViewModel
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int PaymentType { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
