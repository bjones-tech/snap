﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PowerBI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AzureBIReportingContext : DbContext
    {
        public AzureBIReportingContext()
            : base("name=AzureBIReportingContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Office> Offices { get; set; }
        public virtual DbSet<LenelAttendance> LenelAttendance { get; set; }
        public virtual DbSet<NetworkActivity> NetworkActivity { get; set; }
        public virtual DbSet<SCCMWindowsServer> SCCMWindowsServers { get; set; }
        public virtual DbSet<SCCMWindowsWorkstation> SCCMWindowsWorkstations { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}