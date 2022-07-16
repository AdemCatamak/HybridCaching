FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN dotnet publish ./Src/HybridCaching.Api/HybridCaching.Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80
HEALTHCHECK --interval=5s --timeout=3s --retries=3 CMD curl -f / http://localhost:80/health-check || exit 1 

ENTRYPOINT ["./HybridCaching.Api"]