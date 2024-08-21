using Microsoft.EntityFrameworkCore;

namespace AgentsApi.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
	{

	}
}
