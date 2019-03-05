namespace SNAP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedNewHire : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NewHires", "O365License", c => c.String());
            AddColumn("dbo.NewHires", "OracleRequired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewHires", "OracleRequired");
            DropColumn("dbo.NewHires", "O365License");
        }
    }
}
