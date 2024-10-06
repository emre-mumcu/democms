using Microsoft.EntityFrameworkCore;
using src.App_Data.Entities;
using src.App_Data.LookUps;
using src.App_Lib.Configuration;

namespace src.App_Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext() { }

		//public AppDbContext(DbContextOptions options) : base(options) { }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();			

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
			// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

			modelBuilder.SeedData(); // Data seeding along with migrations
		}

		// LookUps		
		public virtual DbSet<City> Cities => Set<City>();
		public virtual DbSet<State> States => Set<State>();
		public virtual DbSet<Gender> Genders => Set<Gender>();

		// Entities
		public virtual DbSet<ActionLog> ActionLogs => Set<ActionLog>();
		public virtual DbSet<ExceptionLog> ExceptionLogs => Set<ExceptionLog>();
		public virtual DbSet<DynamicRole> RoleMatrixes => Set<DynamicRole>();
	}
}