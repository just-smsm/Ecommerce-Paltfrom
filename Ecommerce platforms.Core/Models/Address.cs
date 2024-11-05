using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_platforms.Core.Models
{
    public class Address
    {
        public Address()
        {

        }

        public Address(string name, string phone, string city, string detail)
        {
            Name = name;
            Phone = phone;
            City = city;
            Detail = detail;
        }
       

        public string Name { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Detail { get; set; }

    }
}
