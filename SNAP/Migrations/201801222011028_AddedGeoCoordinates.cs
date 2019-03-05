namespace SNAP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGeoCoordinates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offices", "GeoCoordinates", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offices", "GeoCoordinates");
        }
    }
}
