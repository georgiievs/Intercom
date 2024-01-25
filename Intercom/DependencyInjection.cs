using Intercom.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Intercom
{
    public static class DependencyInjection
    { 
        public static IServiceCollection AddSingletonDatabase(this IServiceCollection services)
        {
            services.AddDbContext<IntercomDbContext>(options =>
            {
                options.UseNpgsql("Host=188.134.68.214;Port=5432;Database=cooking_db;Username=ReadWrite;Password=ReadWrite");
            }, ServiceLifetime.Singleton);

            return services;
        }
    }
}