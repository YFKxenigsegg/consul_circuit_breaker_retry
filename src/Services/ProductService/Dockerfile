# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/ProductService/ProductService.csproj", "src/Services/ProductService/"]

# Restore dependencies
RUN dotnet restore "src/Services/ProductService/ProductService.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Services/ProductService"
RUN dotnet build "ProductService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ProductService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "ProductService.dll"]
