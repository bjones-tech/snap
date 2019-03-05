using SNAP.Models.Persistent;
using System.Data.Entity;

namespace SNAP.DAL
{
    public class SNAPContext : DbContext
    {
        public SNAPContext() : base("SNAPContext")
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<NewHire> NewHires { get; set; }
        public DbSet<LastDay> LastDays { get; set; }
        public DbSet<Contingent> Contingents { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidateInterview> CandidateInterviews { get; set; }
        public DbSet<GSCClient> GSCClients { get; set; }
        public DbSet<GSCUser> GSCUsers { get; set; }
    }
}