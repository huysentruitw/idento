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

### Stories

- -As a developer, I want to be able to create a tenant-
- -As a developer, I want to see a list of all tenants-
- -As a developer, I want to be able to update a tenant-
- -As a developer, I want to be able to delete a tenant-
- -As a developer, I want a validation error when creating/updating a tenant when the tenant name is already in use-

- -As a developer, I want to define the certificate model-
- -As a developer, I want to have a certificate store-
- As a developer, I want to see a list of tenant specific certificates
- As a developer, I want to be able to upload a tenant specific certificate
- As a developer, I want to be able to update the name of a tenant specific certificate
- As a developer, I want to be able to replace a tenant specific certificate by uploading a new certificate
- As a developer, I want to be able to delete a tenant specific certificate
- As a developer, I want a validation error when creating/updating a tenant specific certificate when the certificate name is already in use

- As a developer, I want to define the application model
- As a developer, I want to have a application store
- As a developer, I want to be able to list all tenant specific applications
- As a developer, I want to be able to create a tenant specific application
- As a developer, I want to be able to update a tenant specific application
- As a developer, I want to be able to delete a tenant specific application
- As a developer, I want a validation error when creating/updating a tenant specific application when the application name is already in use

- -As a developer, I want to define the user model-
- As a developer, I want to be able to create a tenant specific user
- As a developer, I want to see a list of all users under a certain tenant
- As a developer, I want to be able to update a tenant specific user
- As a developer, I want to be able to delete a tenant specific user
- As a developer, I want a validation error when creating/updating a tenant specific user when the users e-mail address is already in use

- TBD: Roles (per tenant)

- TBD: OpenIdConnect middleware

- TBD: Login page

- TBD: Consent page

- TBD: Forgot password

- TBD: Self registration

- TBD: External login providers (per tenant)

- TBD: Two-factor authentication to access Idento management pages

- TBD: As a user, I want to be able to register an account using an external identity provider

- TBD: As a user, I want to be able to link/unlink external identity providers

- TBD: Multi-tenant WS-Federation
