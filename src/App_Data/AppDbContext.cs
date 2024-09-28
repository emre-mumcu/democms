using Microsoft.EntityFrameworkCore;
using src.App_Data.Entities;
using src.App_Data.LookUps;
using src.App_Lib.Configuration;
using System.Diagnostics;

//dotnet tool install --global dotnet-ef
//dotnet tool update --global dotnet-ef
//dotnet ef migrations add Migration1 -o App_Data\Migrations
//dotnet ef database update

// dotnet add package Microsoft.EntityFrameworkCore.Sqlite
// dotnet add package Microsoft.EntityFrameworkCore.Design

//dotnet add package Microsoft.EntityFrameworkCore.Relational

//dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

namespace src.App_Data
{
    public class AppDbContext : DbContext
    {

        //private readonly IServiceProvider? _serviceProvider;

        public AppDbContext() { }

        //public AppDbContext(DbContextOptions options) : base(options) { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

/*         public AppDbContext(DbContextOptions<AppDbContext> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;
        }  */

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           //if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

			//Debugger.Launch();

			base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(connectionString: App.Instance._DataConfiguration.GetSection("Database:ConnectionString").Value);
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

            base.OnModelCreating(modelBuilder);

            // Only for sql server
            // modelBuilder.UseCollation("Turkish_CI_AS");

            modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

            // In order to seed the data by this way, migrations (add migration and update database) must be run
            modelBuilder.SeedData();
        }

        // LookUps
        public virtual DbSet<Gender> Genders => Set<Gender>();
        public virtual DbSet<City> Cities => Set<City>();
        public virtual DbSet<State> States => Set<State>();


        // Entities
        public virtual DbSet<ActionLog> ActionLogs => Set<ActionLog>();
        public virtual DbSet<ExceptionLog> ExceptionLogs => Set<ExceptionLog>();
        
        public virtual DbSet<UserRoleRight> UserRoleRights => Set<UserRoleRight>();
    }
}

/*
using var ctx = new BlogContext();

var services = new ServiceCollection()
    .AddEntityFrameworkDesignTimeServices()
    .AddDbContextDesignTimeServices(ctx);

var npgsqlDesignTimeServices = new NpgsqlDesignTimeServices();
npgsqlDesignTimeServices.ConfigureDesignTimeServices(services);

var serviceProvider = services.BuildServiceProvider();
var scaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();
var migration = scaffolder.ScaffoldMigration(
    "MyMigration",
    "MyApp.Data");

Console.WriteLine(migration.MigrationCode);

*/

/*
https://stackoverflow.com/questions/66383701/passing-parameter-to-dbcontext-with-di

You cannot pass dependency to DbContext in this way.

First, create a class:

class MyClass 
{
    public string MyProperty { get; set; }
}
Then pass MyClass as a dependency to DbContext:

MyDbContext(DbContextOptions<MyDbContext> options, MyClass myParam)
    : base(options)
{           
    //* Intentionally empty            
}
Then register MyClass as a singleton to ServiceCollection:

services.AddSingletone(new MyClass { MyProperty = "xx" });
services.AddDbContext<MyDbContext>(options =>options.UseSqlServer(con));
*/