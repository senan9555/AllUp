﻿using AllUp.Models;
using Microsoft.EntityFrameworkCore;

namespace AllUp.DAL
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
                
        }
        public DbSet<Category> Categories { get; set; }
    }
}
