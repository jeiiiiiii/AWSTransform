# Stage 1: Build
FROM public.ecr.aws/amazonlinux/amazonlinux:2023 AS builder

RUN dnf update -y && \
    dnf install -y dotnet-sdk-8.0 && \
    dnf clean all

WORKDIR /build

COPY src/ src/
COPY InventoryDemo.sln .

RUN dotnet publish src/InventoryDemo.Api/InventoryDemo.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-self-contained

# Stage 2: Runtime
FROM public.ecr.aws/amazonlinux/amazonlinux:2023 AS runtime

RUN dnf update -y && \
    dnf install -y aspnetcore-runtime-8.0 shadow-utils && \
    dnf clean all

RUN groupadd -r appuser && useradd -r -g appuser appuser

WORKDIR /app

COPY --chown=appuser:appuser --from=builder /app/publish .

RUN mkdir -p /app/data && chown -R appuser:appuser /app/data

USER appuser

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__Default="Data Source=/app/data/inventory.db"

EXPOSE 8080

ENTRYPOINT ["dotnet", "InventoryDemo.Api.dll"]
