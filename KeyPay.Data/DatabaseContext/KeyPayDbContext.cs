﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeyPay.Data.DatabaseContext
{
    class KeyPayDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;User ID=sa;Password=124000;Initial Catalog=KeyPay;Integrated Security=true;MultipleActiveResultSets=true;");
        }

        public DbSet<Models.User> Users { get; set; }

        public DbSet<Models.BankCard> BankCards { get; set; }

        public DbSet<Models.Photo> Photoes { get; set; }

    }
}