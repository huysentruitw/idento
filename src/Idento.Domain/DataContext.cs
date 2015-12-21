/*
 * Copyright 2016 Wouter Huysentruit
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
using System.Data.Entity;
using Idento.Domain.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Idento.Domain
{
    public class DataContext : IdentityDbContext<User, Role, Guid, UserLogin, UserRole, UserClaim>
    {
        public DataContext()
            : this("Idento")
        {
        }

        public DataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<ExternalLoginProvider> ExternalLoginProviders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Because AspNet.Identity ignores our TableAttribute we will re-map them
            // otherwise we get tablenames like dbo.UserRoles
            modelBuilder
                .MapTableNameFor<Role>()
                .MapTableNameFor<User>()
                .MapTableNameFor<UserClaim>()
                .MapTableNameFor<UserLogin>()
                .MapTableNameFor<UserRole>();
        }
    }
}
