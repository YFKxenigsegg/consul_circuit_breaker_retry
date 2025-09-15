# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/UserService/UserService.csproj", "src/Services/UserService/"]

# Restore dependencies
RUN dotnet restore "src/Services/UserService/UserService.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Services/UserService"
RUN dotnet build "UserService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "UserService.dll"]
