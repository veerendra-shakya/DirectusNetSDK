# Directus.Net SDK Project

## Overview
This is a production-ready .NET 9 class library that implements a comprehensive SDK for Directus headless CMS, mirroring the functionality of the official JavaScript SDK (@directus/sdk).

## Project Structure

```
.
├── src/
│   └── Directus.Net/              # Main SDK library
│       ├── Abstractions/          # Interfaces (IDirectusClient, IAuthService, etc.)
│       ├── Exceptions/            # Custom exception types
│       ├── Extensions/            # DI extensions for service registration
│       ├── Models/                # Data models and query builders
│       ├── Services/              # Service implementations (Auth, Items, Files, etc.)
│       ├── Storage/               # Token storage implementations
│       ├── Transport/             # HTTP transport layer
│       └── DirectusClient.cs      # Main client entry point
├── tests/
│   └── Directus.Net.Tests/        # xUnit tests
│       ├── QueryBuilderTests.cs   # Query builder tests
│       ├── TokenStoreTests.cs     # Token storage tests
│       └── ExceptionTests.cs      # Exception handling tests
├── samples/
│   └── Directus.Net.Sample/       # Console sample application
│       └── Program.cs             # Demonstrates SDK usage
└── Directus.Net.sln               # Solution file
```

## Key Features

### Core Architecture
- **DirectusClient**: Main entry point exposing all services via a fluent API
- **Composable Services**: Auth, Items, Files, Users, Roles, GraphQL, Realtime, Utils
- **Extensible Abstractions**: IDirectusClient, IDirectusTransport, ITokenStore, IAuthService

### API Support
- **REST API**: Full CRUD operations with advanced query capabilities
- **GraphQL**: Query and mutation support with typed responses
- **WebSocket Realtime**: Subscribe to collection updates with automatic reconnection

### Query System
- **Fluent Query Builder**: Chainable methods for building complex queries
- **Filter Operators**: _eq, _neq, _in, _lt, _gt, _contains, _and, _or, etc.
- **Advanced Features**: Sorting, pagination, deep relations, aggregations, field selection

### Authentication
- **Token-based Auth**: Login, logout, automatic refresh
- **Extensible Storage**: ITokenStore abstraction with in-memory implementation
- **Session Management**: Access and refresh token handling

### Resilience
- **Polly Integration**: Retry policies, circuit breakers, timeout strategies
- **Error Handling**: Custom exception types with detailed error information

## Recent Changes
- **2025-11-11**: Initial implementation
  - Created complete SDK architecture with all core services
  - Implemented REST, GraphQL, and WebSocket support
  - Added comprehensive test suite (20 tests, all passing)
  - Created sample console application
  - Configured NuGet package metadata

## Dependencies

### Main Library
- Polly 8.6.4 (resilience policies)
- Microsoft.Extensions.DependencyInjection.Abstractions 9.0.10
- Microsoft.Extensions.Logging.Abstractions 9.0.10
- Microsoft.Extensions.Http 9.0.10

### Testing
- xUnit 2.9.2
- Moq 4.20.72
- FluentAssertions 8.8.0

### Sample App
- Microsoft.Extensions.Hosting 9.0.10
- Microsoft.Extensions.Configuration.Json 9.0.10

## Build & Test

### Build
```bash
dotnet build Directus.Net.sln
```

### Run Tests
```bash
dotnet test Directus.Net.sln
```

### Run Sample
```bash
dotnet run --project samples/Directus.Net.Sample
```

### Create NuGet Package
```bash
dotnet pack src/Directus.Net/Directus.Net.csproj -c Release
```

## Development Guidelines

### Code Conventions
- Full XML documentation on all public APIs
- Async/await throughout with CancellationToken support
- Nullable reference types enabled
- Follow .NET naming conventions
- Comprehensive error handling with custom exceptions

### Testing
- Unit tests for all core functionality
- Mocking with Moq for external dependencies
- Fluent assertions for readable test code
- Aim for high code coverage

## User Preferences
- Production-ready, NuGet-packable code
- Comprehensive documentation
- Idiomatic .NET conventions
- Type-safe implementations
- Feature parity with JavaScript SDK

## Future Enhancements
- Advanced aggregation functions (count, sum, avg, min, max)
- Batch operations optimization
- Caching layer with IMemoryCache integration
- Full OpenAPI/Swagger spec code generation
- GraphQL subscription support with System.Reactive
