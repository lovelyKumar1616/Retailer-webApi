using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityModel.Model
{
    public abstract class BaseEntity
    {
       
        public int Id { get; set; }
       
        public DateTime? UpdatedAt { get; set; }
       
        public DateTime? CreatedAt { get; set; }
       
        public bool? IsDeleted { get; set; }
    }
}
