//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIPSoftSharif.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HamiMadadjouSet
    {
        public int Id { get; set; }
        public int HamiId { get; set; }
        public int MadadjouId { get; set; }
        public string MadadjouFname { get; set; }
        public string MadadjouLname { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
    
        public virtual HamiEditSet HamiEditSet { get; set; }
    }
}
