//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Office
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string CountryISO2 { get; set; }
        public string CountryISO3 { get; set; }
        public int LenelPanelID { get; set; }
        public string Type { get; set; }
        public string Landlord { get; set; }
        public Nullable<double> SecDeposit { get; set; }
        public Nullable<int> Seating { get; set; }
        public Nullable<System.DateTime> LeaseStartDate { get; set; }
        public Nullable<System.DateTime> LeaseEndDate { get; set; }
        public Nullable<double> MonthlyRent { get; set; }
        public Nullable<double> SquareFootage { get; set; }
        public Nullable<double> RentPerSqFoot { get; set; }
        public string Region { get; set; }
    }
}
