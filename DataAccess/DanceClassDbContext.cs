﻿using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataAccess
{
    public class DanceClassDbContext : IdentityDbContext<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>
    {
        public DanceClassDbContext()
            : base("DefaultConnection")
        {
            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;
        }

        public static DanceClassDbContext Create()
        {
            return new DanceClassDbContext();
        }

        public override Task<int> SaveChangesAsync()
        {
            AuditEntity();
            return base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            AuditEntity();
            return base.SaveChanges();
        }

        private void AuditEntity()
        {
            var modifieds = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);
            var currentUser = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
            {
                currentUser = HttpContext.Current.User.Identity.Name;
            }

            foreach (DbEntityEntry item in modifieds)
            {
                var changedOrAddedItem = item.Entity as IAuditable;
                if (changedOrAddedItem != null)
                {
                    if (item.State == EntityState.Added)
                    {
                        changedOrAddedItem.CreatedDate = DateTime.Now;

                        // When using user manager Http context is null so not update the current user with string empty
                        if (!string.IsNullOrEmpty(currentUser))
                        {
                            changedOrAddedItem.CreatedBy = currentUser;
                        }
                    }
                    changedOrAddedItem.UpdatedDate = DateTime.Now;

                    // When using user manager Http context is null so not update the current user with string empty
                    if (!string.IsNullOrEmpty(currentUser))
                    {
                        changedOrAddedItem.UpdatedBy = currentUser;
                    }
                }
            }
        }

        public DbSet<Class> Classes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleDetail> ScheduleDetails { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<DefaultPackage> DefaultPackages { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Branch> Branches { get; set; }
    }
}
