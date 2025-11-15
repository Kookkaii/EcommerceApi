using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceApi.Dtos.Order
{
     public class CheckoutRequest
    {
        public Guid CartId { get; set; }
    }
}