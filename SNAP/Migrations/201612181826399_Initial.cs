namespace SNAP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CandidateInterviews",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    CandidateID = c.Int(nullable: false),
                    Interviewer = c.String(),
                    InterviewersName = c.String(),
                    InterviewersEmail = c.String(),
                    InterviewDate = c.DateTime(),
                    InterviewType = c.String(),
                    Organizer = c.String(),
                    OrganizersName = c.String(),
                    OrganizersEmail = c.String(),
                    GeneralAppraisal = c.String(),
                    TechKnowledge = c.String(),
                    ProblemSolving = c.String(),
                    Teamwork = c.String(),
                    Communication = c.String(),
                    CulturalFit = c.String(),
                    Leadership = c.String(),
                    OverallStrengths = c.String(),
                    OverallConcerns = c.String(),
                    OverallEvaluation = c.String(),
                    OverallRating = c.String(),
                    Recommendation = c.String(),
                    AppointmentID = c.String(),
                    Notes = c.String(),
                    Complete = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Candidates", t => t.CandidateID, cascadeDelete: true)
                .Index(t => t.CandidateID);

            CreateTable(
                "dbo.Candidates",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    FirstName = c.String(nullable: false),
                    LastName = c.String(nullable: false),
                    Title = c.String(nullable: false),
                    Number = c.String(nullable: false),
                    Manager = c.String(),
                    ManagersName = c.String(),
                    ManagersEmail = c.String(nullable: false),
                    Recruiter = c.String(),
                    RecruitersName = c.String(),
                    RecruitersEmail = c.String(),
                    RequestedOn = c.DateTime(nullable: false),
                    ResumeFilePath = c.String(),
                    ResumeFileName = c.String(),
                    Notes = c.String(),
                    Complete = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.ChangeLogs",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Timestamp = c.DateTime(nullable: false),
                    Event = c.String(nullable: false),
                    Description = c.String(nullable: false),
                    SubmittedBy = c.String(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Contingents",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    GUID = c.String(),
                    EmailAddress = c.String(),
                    Name = c.String(),
                    FirstName = c.String(),
                    LastName = c.String(),
                    Title = c.String(),
                    Department = c.String(),
                    ManagersName = c.String(),
                    ManagersEmail = c.String(),
                    Country = c.String(),
                    Office = c.String(),
                    State = c.String(),
                    AccountCreated = c.DateTime(),
                    AccountExpirationDate = c.DateTime(),
                    CodeOfEthics = c.Boolean(nullable: false),
                    InfoSecurity = c.Boolean(nullable: false),
                    PCBusinessPartner = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Countries",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    ADName = c.String(nullable: false),
                    ISO2 = c.String(nullable: false),
                    ISO3 = c.String(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Offices",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    CountryID = c.Int(nullable: false),
                    Name = c.String(nullable: false),
                    StreetAddress = c.String(nullable: false),
                    City = c.String(nullable: false),
                    State = c.String(nullable: false),
                    PostalCode = c.String(nullable: false),
                    ADPath = c.String(nullable: false),
                    ADGroupPrefix = c.String(nullable: false),
                    Networks = c.String(),
                    LenelPanelID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
                .Index(t => t.CountryID);

            CreateTable(
                "dbo.LastDays",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    EmailAddress = c.String(nullable: false),
                    Name = c.String(nullable: false),
                    WorkerID = c.String(),
                    WorkerType = c.String(nullable: false),
                    IsContingent = c.Boolean(nullable: false),
                    EndDate = c.DateTime(nullable: false),
                    StartDate = c.DateTime(),
                    Code = c.String(),
                    Title = c.String(nullable: false),
                    Department = c.String(nullable: false),
                    Manager = c.String(),
                    ManagersEmail = c.String(nullable: false),
                    Office = c.String(nullable: false),
                    Country = c.String(),
                    Immediate = c.Boolean(nullable: false),
                    PublicNotes = c.String(),
                    PrivateNotes = c.String(),
                    Requester = c.String(),
                    RequestersEmail = c.String(),
                    RequestedOn = c.DateTime(nullable: false),
                    GUID = c.String(nullable: false),
                    Decommissioned = c.Boolean(nullable: false),
                    ServiceRequest = c.Boolean(nullable: false),
                    Complete = c.Boolean(nullable: false),
                    ErrorLog = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NewHires",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    EmailAddress = c.String(),
                    FirstName = c.String(nullable: false),
                    LastName = c.String(nullable: false),
                    WorkerID = c.String(),
                    WorkerType = c.String(nullable: false),
                    HireType = c.String(nullable: false),
                    IsContingent = c.Boolean(nullable: false),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(),
                    Title = c.String(nullable: false),
                    Department = c.String(nullable: false),
                    Manager = c.String(),
                    ManagersEmail = c.String(nullable: false),
                    Country = c.String(nullable: false),
                    Office = c.String(nullable: false),
                    State = c.String(),
                    ComputerRequired = c.Boolean(nullable: false),
                    PhoneRequired = c.Boolean(nullable: false),
                    Rehire = c.Boolean(nullable: false),
                    PublicNotes = c.String(),
                    PrivateNotes = c.String(),
                    Requester = c.String(),
                    RequestersEmail = c.String(),
                    RequestedOn = c.DateTime(nullable: false),
                    ServiceDate = c.DateTime(nullable: false),
                    GUID = c.String(),
                    ServiceRequest = c.Boolean(nullable: false),
                    AccountDetails = c.Boolean(nullable: false),
                    Complete = c.Boolean(nullable: false),
                    ErrorLog = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Shipments",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    RecipientsName = c.String(),
                    RecipientsEmail = c.String(nullable: false),
                    ManagersName = c.String(),
                    ManagersEmail = c.String(),
                    ShippersName = c.String(),
                    ShippersEmail = c.String(),
                    ShippedOn = c.DateTime(nullable: false),
                    Item = c.String(nullable: false),
                    TrackingNumber = c.String(nullable: false),
                    TrackingURL = c.String(),
                    Notes = c.String(),
                })
                .PrimaryKey(t => t.ID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Offices", "CountryID", "dbo.Countries");
            DropForeignKey("dbo.CandidateInterviews", "CandidateID", "dbo.Candidates");
            DropIndex("dbo.Offices", new[] { "CountryID" });
            DropIndex("dbo.CandidateInterviews", new[] { "CandidateID" });
            DropTable("dbo.Shipments");
            DropTable("dbo.NewHires");
            DropTable("dbo.LastDays");
            DropTable("dbo.Offices");
            DropTable("dbo.Countries");
            DropTable("dbo.Contingents");
            DropTable("dbo.ChangeLogs");
            DropTable("dbo.Candidates");
            DropTable("dbo.CandidateInterviews");
        }
    }
}
