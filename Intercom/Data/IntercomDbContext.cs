using Intercom.Models;
using Microsoft.EntityFrameworkCore;

namespace Intercom.Data
{
    public class IntercomDbContext : DbContext
    {
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Key> Keys { get; set; }

        public IntercomDbContext() 
            : base() { }

        public IntercomDbContext(DbContextOptions<IntercomDbContext> options) 
            : base(options) { }
    }
}
