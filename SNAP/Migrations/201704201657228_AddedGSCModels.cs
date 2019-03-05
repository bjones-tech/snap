namespace SNAP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGSCModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GSCClients",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        InternalEmailAddress = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GSCUsers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GSCClientID = c.Int(nullable: false),
                        GUID = c.String(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Title = c.String(nullable: false),
                        Department = c.String(nullable: false),
                        Manager = c.String(),
                        ManagersEmail = c.String(nullable: false),
                        Active = c.Boolean(nullable: false),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.GSCClients", t => t.GSCClientID, cascadeDelete: true)
                .Index(t => t.GSCClientID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GSCUsers", "GSCClientID", "dbo.GSCClients");
            DropIndex("dbo.GSCUsers", new[] { "GSCClientID" });
            DropTable("dbo.GSCUsers");
            DropTable("dbo.GSCClients");
        }
    }
}
