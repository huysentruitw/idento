# Idento

Idento wants to be a complete, manageable OpenID Connect server for adding Single Sign On (SSO) functionality to your projects. It is based on the AspNet.Security.OpenIdConnect.Server and ASP.NET Identity but adds a user-interface for managing tenants, certificates, apps, external login providers (like Facebook, LinkedIn, Outlook, Google, etc), users, roles and claims.

Idento will also include user self-service where users can manage their credentials, applications and link/unlink their external logins.

Idento supports multi-tenancy.

## Development

### Create database

- Open Package Manager Console
- Select src\Idento.Domain as Default project
- PM> Update-Database

### Rebuild node-sass for node environment

- `nvm install node.5.12.0`
- `nvm use 5.12.0`
- `npm install`
- `npm rebuild node-sass --force`