//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RESTService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Products
    {
        public int Id { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int Quantity { get; set; }
        public string OriginCountry { get; set; }
    }
}
