# Idento

Idento wants to be a complete, manageable OpenID Connect server for adding Single Sign On (SSO) functionality to your projects. It is based on the popular IdentityServer4 and ASP.NET Identity but adds a user-interface for managing apps, external login providers (like Facebook, LinkedIn, Outlook, Google, etc), users, roles and claims.

Idento will also include user self-service where users can manage their credentials, applications and link/unlink their external logins.

## Development

Please install .NET Core 1.1 SDK if you haven't already. See https://www.microsoft.com/net/download/core#/current

## Create certificate

- `openssl.exe req -newkey rsa:2048 -days 730 -x509 -keyout Idento.key -out Idento.crt -config "..\share\openssl.cnf"` (use 'IdentoTest' as password f.e.)
- `openssl pkcs12 -export -out Idento.pfx -inkey Idento.key -in Idento.crt` (use 'IdentoTest' as password f.e.)
- Replace Idento.Web\Idento.pfx with the newly generated file

## Create database

- Open Developer Command prompt
- Select correct version: `dnvm use 1.0.0-rc1-update1 -p` (note that the version should match the one specified in global.json)
- Go to the src/Idento folder
- Create database by applying migrations: `dnx ef database update -p Idento.Domain`

## Issues

## DO NOT USE JUST YET

This project is in early development stage and is far from complete. A lot of code still needs to be written and existing code will change. This said, don't hesitate to star/watch the project if you want to be kept up-to-date.
