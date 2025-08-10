# Auth Platform

A hobby project focused on building a flexible authentication and authorization platform for web and mobile applications. This repository includes modules for user login, registration, session management, and integration with external identity providers (OAuth2, OpenID Connect, etc.). The main goal is to experiment with security solutions and learn about modern authentication standards.

## What’s included?

- REST API endpoints for user registration and login
- Multi-factor authentication (MFA) support for improved security
- Integration with popular external identity providers (OAuth2)
- Session and JWT token management
- Codebase structured for easy modifications and extensions

## Repository structure

- **auth-platform-api** — Backend REST API (this repository)
- **auth-platform-client** — Client-side web application (see [auth-platform-client](https://github.com/Aadameqq/auth-platform-client))

## Quick Start

### Prerequisites
- Docker

### Steps
1. Copy `.env.docker.example` file and rename copy to `.env.docker`
2. (Optional) Fill `.env.docker` with valid secrets (only required for OAuth-related features).
3. Run the following command
```bash
docker-compose -f docker-compose.dev.yml up app
```
Or if you have Node.js installed:
```bash
npm run quick-start
```
4. Open http://localhost/api-docs in your browser

## Development

### Setup
1. Secrets
```bash
dotnet user-secrets set "OAuth:GithubClientId" "<your-client-id>"
dotnet user-secrets set "OAuth:GithubSecret" "<your-secret>"
```
Work in progress

### Running app

Run app

```bash
npm run start
```

Run app in watch mode

```bash
npm run watch
```

### Other Services

Run all services (db, redis, smtp, etc.)

```bash
npm run services
```

### Database

Create migration

```bash
npm run migration:create -- migration_name
```

Apply migration

```bash
npm run migration:push
```

### Linting

Check code style

```bash
npm run style
```

Check formatting

```bash
npm run style:csharpier
```
