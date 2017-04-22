# Idento

Idento wants to be a complete, manageable OpenID Connect server for adding Single Sign On (SSO) functionality to your projects. It is based on the AspNet.Security.OpenIdConnect.Server and ASP.NET Identity but adds a user-interface for managing certificates, apps, external login providers (like Facebook, LinkedIn, Outlook, Google, etc), users, roles and claims.

Idento will also include user self-service where users can manage their credentials, applications and link/unlink their external logins.

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

- -As a developer, I want to define the application model-
- -As a developer, I want to have a application store-
- -As a developer, I want to be able to list all applications-
- -As a developer, I want to be able to create an application-
- -As a developer, I want to be able to update an application-
- -As a developer, I want to be able to delete an application-
- -As a developer, I want a validation error when creating/updating an application when the application name is already in use-

- -As a developer, I want to define the certificate model-
- -As a developer, I want to have a certificate store-
- -As a developer, I want to see a list of certificates-
- -As a developer, I want to be able to upload a file-
- -As a developer, I want to verify that the uploaded file is a valid certificate-
- -As a developer, I want to be able to update the name of a certificate-
- -As a developer, I want to be able to replace a certificate by uploading a new certificate-
- -As a developer, I want to be able to delete a certificate-
- -As a developer, I want a validation error when creating/updating certificate when the certificate name is already in use-

- -As a developer, I want to define the user model-
- As a developer, I want to be able to create a user
- As a developer, I want to see a list of all users
- As a developer, I want to be able to update a user
- As a developer, I want to be able to delete a user
- As a developer, I want a validation error when creating/updating a user when the users e-mail address is already in use

- TBD: Roles

- TBD: OpenIdConnect middleware

- TBD: Login page

- TBD: Consent page

- TBD: Forgot password

- TBD: Self registration

- TBD: External login providers

- TBD: Two-factor authentication to access Idento management pages

- TBD: As a user, I want to be able to register an account using an external identity provider

- TBD: As a user, I want to be able to link/unlink external identity providers

- TBD: Multiple WS-Federation support
