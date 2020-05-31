using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoopbackAuthenticationIntegrationPoC.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LoopbackAuthenticationIntegrationPoC.Data
{
    public class LoopbackDbContext : DbContext
    {
        public LoopbackDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.AccessTokens)
                .WithOne(at => at.User)
                .HasForeignKey(at => at.UserId);

            modelBuilder.Entity<AccessToken>()
                .Property(at => at.Scopes)
                .HasConversion(stringArray => JsonConvert.SerializeObject(stringArray),
                    jsonArray => JsonConvert.DeserializeObject<string[]>(jsonArray));
            
            // Note: this is only needed for SQLite as it does not have a datetime type
            modelBuilder.Entity<AccessToken>()
                .Property(at => at.Created)
                .HasConversion(dateTime => (double)new DateTimeOffset(dateTime).ToUnixTimeMilliseconds(),
                    unixDateTimeInMillis => DateTimeOffset.FromUnixTimeMilliseconds((long)unixDateTimeInMillis).DateTime);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<Acl> Acls { get; set; }
    }
}
