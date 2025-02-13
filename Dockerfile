### Build Project ###
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything
COPY . .

WORKDIR ./src/WhoIsHome.Host
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release --no-restore --output ./out

### Build RUN Image ###
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/WhoIsHome.Host/out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "WhoIsHome.Host.dll"]