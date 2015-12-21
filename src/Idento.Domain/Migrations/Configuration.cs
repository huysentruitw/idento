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

using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Idento.Domain.AspNetIdentity;
using Idento.Domain.Models;

namespace Idento.Domain.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;

#if DEBUG
            this.AutomaticMigrationDataLossAllowed = true;
#endif
        }

        protected override void Seed(DataContext context)
        {
            // Uncomment this if you want to attach a debugger during "Update-Database"
            //if (!System.Diagnostics.Debugger.IsAttached)
            //    System.Diagnostics.Debugger.Launch();

            this.SeedAsync(context).Wait();
        }

        private async Task SeedAsync(DataContext context)
        {
            // TODO should be user customizable
            using (var store = new UserStore(context))
            using (var role = new RoleStore(context))
            using (var manager = new UserManager(store))
            {
                var rootRole = new Role
                {
                    Name = "root"
                };
                context.Roles.AddOrUpdate(x => x.Name, rootRole);

                var adminUser = new User
                {
                    UserName = "admin",
                    PasswordHash = manager.PasswordHasher.HashPassword("idento")
                };
                context.Users.AddOrUpdate(x => x.UserName, adminUser);

                await context.SaveChangesAsync();

                await manager.AddToRoleAsync(adminUser.Id, rootRole.Name);
            }

            context.Applications.AddOrUpdate(x => x.ClientId, new Application
            {
                ClientId = "Idento.Client",
                DisplayName = "Idento.Client",
                Enabled = true,
                Uri = "https://localhost:44344/",
                RedirectUri = "https://localhost:44344/",
                Flow = OAuth2Flow.Implicit,
                TokenLifetimeInMinutes = 60,
                RequireConsent = false
            });
        }
    }
}
