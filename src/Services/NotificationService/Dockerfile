# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5004

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/NotificationService/NotificationService.csproj", "src/Services/NotificationService/"]

# Restore dependencies
RUN dotnet restore "src/Services/NotificationService/NotificationService.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Services/NotificationService"
RUN dotnet build "NotificationService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "NotificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5004
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "NotificationService.dll"]
