# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY InventorySystem.csproj ./
RUN dotnet restore

# Copy the rest of the app
COPY . ./
RUN dotnet publish InventorySystem.csproj -c Release -o /out

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose the default port (optional)
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "InventorySystem.dll"]
