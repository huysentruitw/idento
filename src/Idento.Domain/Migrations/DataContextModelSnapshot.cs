using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Idento.Domain;

namespace Idento.Domain.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Idento.Domain.Models.Application", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessTokenLifetimeInMinutes");

                    b.Property<bool>("AllowAllScopes");

                    b.Property<string>("AllowedCorsOrigins");

                    b.Property<string>("AllowedExternalLoginProviders");

                    b.Property<string>("AllowedScopes");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("ClientSecret")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("Enabled");

                    b.Property<string>("FlowName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("RedirectUris")
                        .IsRequired();

                    b.Property<bool>("RequireConsent");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "Applications");
                });

            modelBuilder.Entity("Idento.Domain.Models.Certificate", b =>
                {
                    b.Property<string>("Id")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<byte[]>("Data")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "Certificates");
                });

            modelBuilder.Entity("Idento.Domain.Models.ExternalLoginProvider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("OAuth2ClientId")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("OAuth2ClientSecret")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("ProviderName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("WsFederationMetadataAddress")
                        .HasAnnotation("MaxLength", 1024);

                    b.Property<string>("WsFederationRealm")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "ExternalLoginProviders");
                });

            modelBuilder.Entity("Idento.Domain.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "Roles");
                });

            modelBuilder.Entity("Idento.Domain.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("LastName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "Users");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:Schema", "Security");

                    b.HasAnnotation("Relational:TableName", "UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Idento.Domain.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Idento.Domain.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Idento.Domain.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Idento.Domain.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("Idento.Domain.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
