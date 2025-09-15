# ECommerce Microservices

A simple .NET microservices architecture for an e-commerce application.

## Architecture

- **API Gateway**: Entry point for all client requests
- **User Service**: Handles user management and authentication
- **Product Service**: Manages product catalog
- **Order Service**: Processes orders and transactions
- **Notification Service**: Handles email/SMS notifications

## Getting Started

1. Build the solution:
   ```
   dotnet build
   ```

2. Run with Docker:
   ```
   docker-compose up --build
   ```

## Endpoints

- API Gateway: http://localhost:5000
- User Service: http://localhost:5001
- Product Service: http://localhost:5002
- Order Service: http://localhost:5003
- Notification Service: http://localhost:5004