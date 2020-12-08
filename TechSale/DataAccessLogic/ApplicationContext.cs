using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplicationTechSale.Models;

namespace DataAccessLogic
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<AuctionLot> AuctionLots { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<SavedList> SavedLists { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base (options)
        {
            Database.EnsureCreated();
        }
    }
}
