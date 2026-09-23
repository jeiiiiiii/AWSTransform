# Docker Build & Run Instructions

## Overview

**InventoryDemo.Api** is an ASP.NET Core 8.0 Web API and MVC application for inventory management. It uses SQLite as its embedded database, runs EF Core migrations automatically on startup, and seeds initial data if the database is empty.

---

## Prerequisites

- Docker installed and running
- Build context: the **root of the repository** (same directory as `Dockerfile`)

---

## Build

```bash
# From the root of the repository:
docker build -t inventorydemo-api .
```

---

## Run

```bash
docker run -d \
  --name inventorydemo-api \
  -p 8080:8080 \
  -v inventorydemo-data:/app/data \
  inventorydemo-api
```

The application will be available at: [http://localhost:8080](http://localhost:8080)

Swagger UI (when `ASPNETCORE_ENVIRONMENT=Development`): [http://localhost:8080/swagger](http://localhost:8080/swagger)

---

## Environment Variables

| Variable | Default | Description |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Controls environment-specific behavior. Set to `Development` to enable Swagger UI. |
| `ConnectionStrings__Default` | `Data Source=/app/data/inventory.db` | SQLite connection string. Override to use a different path or database file. |

---

## Persistent Storage

The SQLite database is stored at `/app/data/inventory.db` inside the container. Mount a volume to persist data across container restarts:

```bash
# Named volume (recommended)
docker run -d \
  -p 8080:8080 \
  -v inventorydemo-data:/app/data \
  inventorydemo-api

# Bind mount (local directory)
docker run -d \
  -p 8080:8080 \
  -v $(pwd)/data:/app/data \
  inventorydemo-api
```

> ⚠️ Without a volume mount, the SQLite database is lost when the container stops.

---

## Secret Handling

For production deployments, manage sensitive environment variables (e.g., connection strings) using:
- **ECS:** [AWS Secrets Manager integration with ECS](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/secrets-envvar-secrets-manager.html)
- **EKS:** [Secrets Manager or KMS encryption for EKS](https://docs.aws.amazon.com/eks/latest/userguide/security-k8s.html)

---

## Multi-stage Build Details

| Stage | Base Image | Purpose |
|---|---|---|
| `builder` | `amazonlinux:2023` + `dotnet-sdk-8.0` | Compiles and publishes the application |
| `runtime` | `amazonlinux:2023` + `aspnetcore-runtime-8.0` | Runs the published application |

The runtime image contains only the ASP.NET Core runtime (no SDK), keeping the final image lean.
