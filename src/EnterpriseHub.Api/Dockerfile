# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution / project files first for layer caching
COPY . .

# Restore
RUN dotnet restore src/EnterpriseHub.Api/EnterpriseHub.Api.csproj

# Publich ( self-containes false , framework dependent )
RUN dotnet publish src/EnterpriseHub.Api/EnterpriseHub.Api.csproj -c Release -o /app/publish /p:UseAppHost=false 

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080 
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EnterpriseHub.Api.dll"]

