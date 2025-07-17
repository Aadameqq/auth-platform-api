# Contest app

A contest management platform that enables easy organization of programming competitions, designed to be convenient,
easy to develop, and highly customizable

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
