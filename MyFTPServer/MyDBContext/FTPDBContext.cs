using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFTPServer.MyDBContext
{
    class FTPDBContext : DbContext
    {
        public FTPDBContext(DbContextOptions options) : base(options)
        {
            // Init
        }

        public DbSet<User> User { get; set; }

        public DbSet<Dictionary> Dictionary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
        }

        public void Seed()
        {
            AddTestUser();
            AddSimplyConfigrution();
            this.SaveChanges();
        }

        private void AddTestUser()
        {
            User user = User.FirstOrDefault(c => c.LoginName == "test");
            if (user == null)
            {
                this.Entry<User>(new User() {
                    LoginName = "test",
                    Password = "123",
                    CanCopyFiles = true,
                    CanDeleteFiles = true,
                    CanDeleteFolders = true,
                    CanRenameFiles = true,
                    CanRenameFolders = true,
                    CanStoreFiles = true,
                    CanStoreFolder = true,
                    CanViewHiddenFiles = true,
                    CanViewHiddenFolders = true,
                    WorkdDirectory = @"\"
                }).State = EntityState.Added;
            }
        }

        private void AddSimplyConfigrution()
        {
            Dictionary port = Dictionary.FirstOrDefault(c => c.Key == "Port");
            if (port == null)
            {
                this.Entry<Dictionary>(new Dictionary() { Key = "Port", Value = "21" }).State = EntityState.Added;
            }

            Dictionary MinPOVE = Dictionary.FirstOrDefault(c => c.Key == "MinPOVE");
            if (MinPOVE == null)
            {
                this.Entry<Dictionary>(new Dictionary() { Key = "MinPOVE", Value = "7000" }).State = EntityState.Added;
            }

            Dictionary MaxPove = Dictionary.FirstOrDefault(c => c.Key == "MaxPove");
            if (MaxPove == null)
            {
                this.Entry<Dictionary>(new Dictionary() { Key = "MaxPove", Value = "8000" }).State = EntityState.Added;
            }
        }
    }
}
