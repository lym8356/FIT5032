//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FIT5032_Project.data.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CartItem
    {
        public int cartId { get; set; }
        public string productId { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<double> unitCost { get; set; }
        public Nullable<double> subTotal { get; set; }
    }
}
