using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using src.App_Lib.Configuration;
using System.Diagnostics;

namespace src.App_Data
{
    /// <summary>
    /// IDesignTimeDbContextFactory<TContext> is an interface in Entity Framework Core (EF Core) that helps create instances 
    /// of your DbContext at --design time--, such as during migrations or when using tools like ef migrations add. 
    /// This is particularly useful when your DbContext requires parameters that aren’t easily provided at design time, such as 
    /// connection strings from configuration files or services that are only available at runtime.
    /// 
    /// If a class implementing Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<TContext> interface is found 
    /// in either the same project as the derived DbContext or in the application's startup project, 
    /// the tools bypass the other ways of creating the DbContext and use the design-time factory instead.
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        // Both IDesignTimeDbContextFactory<TContext>.CreateDbContext and Program.CreateHostBuilder accept command line arguments.
        public AppDbContext CreateDbContext(string[] args)
        {
			//Debugger.Launch();

			// var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("data.json", optional: false).Build();

			var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var cs = App.Instance._DataConfiguration.GetSection("Database:ConnectionString").Value;

            optionsBuilder.UseSqlite(cs);

            //return new AppDbContext(optionsBuilder.Options);
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}