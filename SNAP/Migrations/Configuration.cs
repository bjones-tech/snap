namespace SNAP.Migrations
{
    using Models.Persistent;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<SNAP.DAL.SNAPContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SNAP.DAL.SNAPContext context)
        {
            context.Countries.AddOrUpdate(c => c.Name, new Country { Name = "United States of America", ADName = "United States", ISO3 = "USA", ISO2 = "US" });
            context.Countries.AddOrUpdate(c => c.Name, new Country { Name = "Canada", ADName = "Canada", ISO3 = "CAN", ISO2 = "CA" });
            context.Countries.AddOrUpdate(c => c.Name, new Country { Name = "Mexico", ADName = "Mexico", ISO3 = "MEX", ISO2 = "MX" });
            context.Countries.AddOrUpdate(c => c.Name, new Country { Name = "Brazil", ADName = "Brazil", ISO3 = "BRA", ISO2 = "BR" });
            context.Countries.AddOrUpdate(c => c.Name, new Country { Name = "Chile", ADName = "Chile", ISO3 = "CHL", ISO2 = "CL" });

            context.SaveChanges();
        }
    }
}
