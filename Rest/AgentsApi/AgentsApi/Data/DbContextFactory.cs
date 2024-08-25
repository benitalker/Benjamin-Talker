namespace AgentsApi.Data
{
	public static class DbContextFactory
	{
		public static ApplicationDbContext CreateDbContext(IServiceProvider serviceProvider)
		{
			var scope = serviceProvider.CreateScope();
			return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		}
	}
}
