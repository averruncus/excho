//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Excho.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderDetail
    {
        public int Order { get; set; }
        public int Sale { get; set; }
        public Nullable<int> Volume { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}