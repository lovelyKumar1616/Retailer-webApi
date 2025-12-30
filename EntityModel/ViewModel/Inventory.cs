using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.ViewModel
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int QuantityAvailable { get; set; }

    }

}
