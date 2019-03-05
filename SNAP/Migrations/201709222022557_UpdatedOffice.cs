namespace SNAP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedOffice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offices", "Type", c => c.String());
            AddColumn("dbo.Offices", "Landlord", c => c.String());
            AddColumn("dbo.Offices", "SecDeposit", c => c.Single(nullable: false));
            AddColumn("dbo.Offices", "Seating", c => c.Int(nullable: false));
            AddColumn("dbo.Offices", "LeaseStartDate", c => c.DateTime());
            AddColumn("dbo.Offices", "LeaseEndDate", c => c.DateTime());
            AddColumn("dbo.Offices", "MonthlyRent", c => c.Single(nullable: false));
            AddColumn("dbo.Offices", "SquareFootage", c => c.Single(nullable: false));
            AddColumn("dbo.Offices", "RentPerSqFoot", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offices", "RentPerSqFoot");
            DropColumn("dbo.Offices", "SquareFootage");
            DropColumn("dbo.Offices", "MonthlyRent");
            DropColumn("dbo.Offices", "LeaseEndDate");
            DropColumn("dbo.Offices", "LeaseStartDate");
            DropColumn("dbo.Offices", "Seating");
            DropColumn("dbo.Offices", "SecDeposit");
            DropColumn("dbo.Offices", "Landlord");
            DropColumn("dbo.Offices", "Type");
        }
    }
}
