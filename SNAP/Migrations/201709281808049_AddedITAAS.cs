namespace SNAP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedITAAS : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LastDays", "ITaaS", c => c.Boolean(nullable: false));
            AddColumn("dbo.NewHires", "ITaaS", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewHires", "ITaaS");
            DropColumn("dbo.LastDays", "ITaaS");
        }
    }
}
