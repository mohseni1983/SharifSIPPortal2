﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MadadkarOnlineEntities : DbContext
    {
        public MadadkarOnlineEntities()
            : base("name=MadadkarOnlineEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CalendarEvent> CalendarEvent { get; set; }
        public virtual DbSet<CallStatus> CallStatus { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PhoneChange> PhoneChange { get; set; }
        public virtual DbSet<SIPExtensions> SIPExtensions { get; set; }
        public virtual DbSet<PayRequests> PayRequests { get; set; }
        public virtual DbSet<PaymentDetails> PaymentDetails { get; set; }
        public virtual DbSet<JobSchedule> JobSchedule { get; set; }
        public virtual DbSet<JobShift> JobShift { get; set; }
        public virtual DbSet<ShiftPersons> ShiftPersons { get; set; }
        public virtual DbSet<HamiEditSet> HamiEditSet { get; set; }
        public virtual DbSet<HamiMadadjouSet> HamiMadadjouSet { get; set; }
    }
}
