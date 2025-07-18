# GetHub - E-Commerce Microservices Platform

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.12-orange)
![Docker](https://img.shields.io/badge/Docker-Compose-blue)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2B%20CQRS-green)

GetHub is a comprehensive e-commerce microservices platform built with ASP.NET Core 8, implementing Clean Architecture, CQRS pattern, and Event-Driven Architecture. The platform enables users to buy and sell products with full transaction tracking, cart management, and order processing.

##  **Architecture Overview**

### **Microservices Architecture**
The platform follows a distributed microservices architecture with the following services:

### **Technology Stack**

- **Backend Framework**: ASP.NET Core 8.0 with Minimal APIs
- **Authentication**: JWT Bearer tokens
- **Database**: PostgreSQL 15 with Entity Framework Core, MongoDB
- **Message Broker**: RabbitMQ with MassTransit
- **API Gateway**: YARP (Yet Another Reverse Proxy)
- **Containerization**: Docker & Docker Compose
- **Patterns**: Clean Architecture, CQRS, Event Sourcing, Result Pattern

##  **Services Overview**

### **1. API Gateway Service**
- **Technology**: YARP Reverse Proxy
- **Purpose**: Single entry point for all client requests
- **Features**:
  - Request routing to appropriate microservices
  - Load balancing and service discovery
  - Authentication middleware integration
  - Cross-cutting concerns handling

### **2. Authentication Service**
- **Technology**: ASP.NET Core with JWT
- **Purpose**: User authentication and authorization
- **Features**:
  - JWT token generation and validation
  - Integration with Keycloak for OAuth2/OIDC
  - External vs Internal token schemes
  - Role-based access control

### **3. User Service**
- **Technology**: ASP.NET Core Minimal APIs
- **Purpose**: User profile and account management
- **Features**:
  - User registration and profile management
  - Account settings and preferences
  - User verification and validation

### **4. Catalog Service**
- **Architecture**: Clean Architecture with EAV Pattern
- **Purpose**: Product catalog and inventory management
- **Features**:
  - Entity-Attribute-Value (EAV) database design
  - Flexible product attributes
  - Product search and filtering
  - Inventory tracking

### **5. Cart Service**
- **Architecture**: CQRS with Event Sourcing
- **Purpose**: Shopping cart management
- **Features**:
  - Add/remove items from cart
  - Update item quantities
  - Cart persistence and session management
  - Real-time cart synchronization

### **6. Order Service**
- **Architecture**: CQRS with MediatR
- **Purpose**: Order processing and management
- **Features**:
  - Order creation from cart items
  - Order status tracking
  - Order history and analytics
  - Integration with payment processing

### **7. Transaction Service**
- **Architecture**: CQRS with Advanced Analytics
- **Purpose**: Transaction tracking and financial analytics
- **Features**:
  - Complete purchase/sale transaction recording
  - User trading statistics and analytics
  - Financial reporting and insights
  - Transaction status management

##  **Clean Architecture Implementation**

Each service follows Clean Architecture principles with clear separation of concerns:

##  **CQRS Pattern Implementation**

Commands handle write operations while queries handle read operations, providing clear separation between data modification and data retrieval.

##  **Security & Authentication**

### **JWT Authentication Strategy**
- **External Tokens**: For client-to-service communication
- **Internal Tokens**: For service-to-service communication
- **Keycloak Integration**: OAuth2/OIDC identity provider
- **Role-Based Access**: Fine-grained authorization

## **Event-Driven Architecture**

Services communicate through domain events for loose coupling and eventual consistency.

### **Event Integration**
- **Cross-Service Communication**: Services communicate via events
- **Eventual Consistency**: Distributed transaction handling
- **Event Sourcing**: Complete audit trail of all operations
- **Business Intelligence**: Real-time analytics and reporting

##  **Database Design**

### **PostgreSQL with EF Core**
Each service has its own database following Database-per-Service pattern.

### **CQRS Database Separation**
- **Write Models**: Optimized for commands and business logic
- **Read Models**: Optimized for queries and reporting
- **Event Store**: Optional event sourcing implementation

##  **Docker & Deployment**

The platform uses Docker Compose for local development and container orchestration.

### **Service Configuration**
Each service includes:
- **Dockerfile**: Multi-stage build optimization
- **Environment Variables**: Secure configuration management
- **Health Checks**: Service monitoring and diagnostics
- **.env Files**: Service-specific configuration

##  **Getting Started**

### **Prerequisites**
- .NET 8.0 SDK
- Docker & Docker Compose
- PostgreSQL (or use Docker)
- Visual Studio Code or Visual Studio

### **Running the Platform**

Follow these steps to run the platform locally:
- API Gateway: `http://localhost:5000`
- Swagger Documentation: `http://localhost:{port}/swagger`
- RabbitMQ Management: `http://localhost:15672`
- Keycloak Admin: `http://localhost:8080`

## **API Documentation**

Each service provides REST API endpoints for their specific domain operations.

## **Testing Strategy**

### **Unit Testing**
- **Domain Logic Testing**: Business rule validation
- **Handler Testing**: Command/Query handler logic
- **Repository Testing**: Data access layer testing

### **Integration Testing**
- **API Endpoint Testing**: End-to-end request/response testing
- **Database Testing**: Repository integration testing
- **Event Testing**: Message publishing/consuming testing

### **Performance Testing**
- **Load Testing**: High-traffic scenario testing
- **Stress Testing**: System limits and breaking points
- **Database Performance**: Query optimization testing

## **Development Workflow**

### **Adding New Features**
1. **Domain First**: Define entities, events, and business logic
2. **Application Layer**: Create commands, queries, and handlers
3. **Infrastructure**: Implement repositories and data access
4. **API Layer**: Define endpoints and integrate with handlers
5. **Testing**: Unit tests, integration tests, and documentation

## **Monitoring & Observability**

### **Logging**
- **Structured Logging**: JSON-formatted logs with Serilog
- **Correlation IDs**: Request tracing across services
- **Error Tracking**: Exception logging and monitoring

### **Health Checks**
- **Service Health**: Individual service health endpoints
- **Database Health**: Database connection monitoring
- **External Dependencies**: Third-party service monitoring

### **Metrics**
- **Performance Metrics**: Response times and throughput
- **Business Metrics**: Orders, sales, and user activity
- **System Metrics**: CPU, memory, and disk usage

## **Future Enhancements**

### **Planned Features**
- [ ] Payment Gateway Integration
- [ ] Inventory Management Service
- [ ] Notification Service (Email/SMS)
- [ ] Search Service with Elasticsearch
- [ ] Recommendation Engine
- [ ] Mobile API Support
- [ ] GraphQL Gateway
- [ ] Localization

### **Technical Improvements**
- [ ] Event Sourcing Implementation
- [ ] CQRS Read Model Projections
- [ ] Distributed Caching with Redis
- [ ] Circuit Breaker Pattern
- [ ] API Rate Limiting
- [ ] Comprehensive Monitoring Dashboard

---

**GetHub** - Building the future of e-commerce with modern microservices architecture!
