/*
 * Copyright 2016-2017 Wouter Huysentruit
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Idento.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Idento.Domain
{
    public class DataContext : IdentityDbContext<User, Role, Guid>
    {
        public DataContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Because AspNet.Identity ignores our TableAttribute we will re-map them
            // otherwise we get table names like dbo.UserRoles
            builder
                .MapTableNameFor<Role>()
                .MapTableNameFor<User>();

            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "Security");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "Security");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "Security");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "Security");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "Security");
        }
    }
}
