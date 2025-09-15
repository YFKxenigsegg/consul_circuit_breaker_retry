# ECommerce Microservices with Consul, Circuit Breaker & Retry Patterns

A comprehensive .NET 9 microservices architecture for an e-commerce application featuring service discovery, circuit breaker patterns, retry mechanisms, and resilient communication between services.

## 🏗️ Architecture Overview

This project implements a modern microservices architecture with the following key components:

### Core Services
- **API Gateway** (Ocelot): Single entry point with service discovery and load balancing
- **User Service**: User management, authentication, and profile operations
- **Product Service**: Product catalog management and inventory tracking
- **Order Service**: Order processing, status management, and transaction handling
- **Notification Service**: Multi-channel notifications (Email, Push, In-App) with template rendering

### Infrastructure Components
- **Consul**: Service discovery and health monitoring
- **Circuit Breaker**: Fault tolerance and service protection
- **Resilient HTTP Client**: Retry policies with exponential backoff
- **PostgreSQL**: Database per service pattern
- **Docker Compose**: Container orchestration

## 🚀 Key Features

### Service Discovery & Load Balancing
- **Consul Integration**: Service discovery handled by API Gateway only
- **Health Checks**: Continuous service health monitoring via API Gateway
- **Load Balancing**: Round-robin distribution across service instances

### Resilience Patterns
- **Circuit Breaker**: Prevents cascading failures with configurable thresholds
- **Retry Policies**: Exponential backoff for transient failures
- **Timeout Handling**: Configurable request timeouts
- **Fallback Mechanisms**: Graceful degradation when services are unavailable

### Communication Patterns
- **HTTP-based**: RESTful APIs with JSON serialization
- **Service-to-Service**: Resilient inter-service communication
- **Event-driven**: Asynchronous notification processing

### Data Management
- **Database per Service**: Isolated data storage
- **Entity Framework Core**: ORM with PostgreSQL
- **Migration Support**: Database schema versioning

## 📁 Project Structure

```
src/
├── ApiGateway/                 # Ocelot API Gateway
│   ├── Program.cs             # Gateway configuration
│   └── ocelot.json           # Routing and service discovery config
├── Services/                  # Business Services
│   ├── UserService/          # User management
│   ├── ProductService/       # Product catalog
│   ├── OrderService/         # Order processing
│   └── NotificationService/  # Multi-channel notifications
├── Shared/                   # Shared Libraries
│   ├── CircuitBreaker/       # Circuit breaker implementation
│   ├── ServiceDiscovery/     # Consul integration
│   └── Common/              # Resilient HTTP client
└── Infrastructure/          # Infrastructure concerns
```

## 🛠️ Technology Stack

- **.NET 9**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Ocelot**: API Gateway
- **Consul**: Service discovery
- **PostgreSQL**: Database
- **Entity Framework Core**: ORM
- **Docker**: Containerization
- **Polly**: Resilience patterns
- **Handlebars**: Template rendering

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Docker Desktop
- PostgreSQL (if running locally)

### Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd consul_circuit_breaker_retry
   ```

2. **Build and run all services**
   ```bash
   docker-compose up --build
   ```

3. **Access the services**
   - API Gateway: http://localhost:5000
   - Consul UI: http://localhost:8500
   - User Service: http://localhost:5001
   - Product Service: http://localhost:5002
   - Order Service: http://localhost:5003
   - Notification Service: http://localhost:5004

### Local Development

1. **Build the solution**
   ```bash
   dotnet build
   ```

2. **Start Consul** (required for service discovery)
   ```bash
   consul agent -dev
   ```

3. **Run individual services**
   ```bash
   # Terminal 1 - API Gateway
   cd src/ApiGateway
   dotnet run

   # Terminal 2 - User Service
   cd src/Services/UserService
   dotnet run

   # Terminal 3 - Product Service
   cd src/Services/ProductService
   dotnet run

   # Terminal 4 - Order Service
   cd src/Services/OrderService
   dotnet run

   # Terminal 5 - Notification Service
   cd src/Services/NotificationService
   dotnet run
   ```

## 📡 API Endpoints

### API Gateway (Port 5000)
All requests should go through the API Gateway:

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `DELETE /api/users/{id}` - Delete user

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}/stock` - Update product stock

- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}/status` - Update order status

- `GET /api/notifications` - Get user notifications
- `POST /api/notifications` - Send notification
- `PUT /api/notifications/{id}/read` - Mark as read

### Direct Service Access (Development)
- User Service: http://localhost:5001
- Product Service: http://localhost:5002
- Order Service: http://localhost:5003
- Notification Service: http://localhost:5004

## 🔧 Configuration

### Service Discovery
The API Gateway uses Consul for service discovery and load balancing. Individual services do not register themselves with Consul - they are discovered by the gateway through their exposed endpoints.

### Circuit Breaker
- **Failure Threshold**: 5 consecutive failures
- **Timeout**: 1 minute
- **States**: Closed → Open → Half-Open

### Retry Policy
- **Max Retries**: 3 attempts
- **Backoff Strategy**: Exponential (2^retryAttempt seconds)
- **Transient Errors**: HTTP 5xx, timeouts, network errors

### Database Configuration
Each service uses its own PostgreSQL database:
- User Service: `users` database
- Product Service: `products` database
- Order Service: `orders` database
- Notification Service: `notifications` database

## 🏥 Health Monitoring

All services expose health check endpoints at `/health`:
- **User Service**: http://localhost:5001/health
- **Product Service**: http://localhost:5002/health
- **Order Service**: http://localhost:5003/health
- **Notification Service**: http://localhost:5004/health

## 🔄 Resilience Patterns

### Circuit Breaker Implementation
```csharp
// Automatic failure detection and service protection
var result = await circuitBreaker.ExecuteAsync(async () => {
    return await httpClient.GetAsync("http://product-service/api/products");
});
```

### Retry with Exponential Backoff
```csharp
// Automatic retry for transient failures
var response = await resilientHttpClient.GetAsync("http://user-service/api/users");
```

### Service Discovery
```csharp
// Service discovery is handled by the API Gateway
// Services communicate through the gateway, not directly
// Example: Client -> API Gateway -> User Service
```

## 📊 Monitoring & Observability

- **Consul UI**: Service health and discovery status
- **Health Checks**: Individual service health endpoints
- **Structured Logging**: JSON-formatted logs with correlation IDs
- **Error Tracking**: Comprehensive exception handling and logging

## 🧪 Testing

### API Testing
Use the provided `.http` files for testing individual services:
- `src/ApiGateway/ApiGateway.http`
- `src/Services/UserService/UserService.http`
- `src/Services/ProductService/ProductService.http`
- `src/Services/OrderService/OrderService.http`
- `src/Services/NotificationService/NotificationService.http`

### Load Testing
The system is designed to handle load with:
- Circuit breakers preventing cascade failures
- Retry policies handling transient issues
- Load balancing across service instances

## 🚀 Deployment

### Docker Deployment
```bash
# Production deployment
docker-compose -f docker-compose.prod.yml up -d
```

### Kubernetes Deployment
The services can be deployed to Kubernetes with:
- Consul for service discovery
- PostgreSQL as StatefulSets
- Services with proper health checks

## 🔒 Security Considerations

- **CORS**: Configured for cross-origin requests
- **Input Validation**: Model validation on all endpoints
- **Error Handling**: Secure error responses without sensitive data
- **Service Isolation**: Each service runs in its own container

## 📈 Performance Features

- **Connection Pooling**: Efficient database connections
- **Async/Await**: Non-blocking I/O operations
- **Caching**: Service discovery caching
- **Batch Processing**: Notification processing in batches

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Troubleshooting

### Common Issues

1. **Service Discovery Issues**
   - Ensure Consul is running
   - Check API Gateway configuration
   - Verify service endpoints are accessible

2. **Database Connection Issues**
   - Verify PostgreSQL is running
   - Check connection strings
   - Ensure databases exist

3. **Circuit Breaker Issues**
   - Check failure thresholds
   - Monitor service health
   - Review retry policies

### Logs
Check Docker logs for detailed error information:
```bash
docker-compose logs [service-name]
```

## 📚 Additional Resources

- [Ocelot Documentation](https://ocelot.readthedocs.io/)
- [Consul Documentation](https://www.consul.io/docs)
- [Polly Documentation](https://github.com/App-vNext/Polly)
- [.NET Microservices Guide](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)