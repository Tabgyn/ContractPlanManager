# Contract Plan Manager

Enterprise-grade fullstack application for managing contract payment plan modifications.

## 🎯 Purpose

Demonstrates senior-level fullstack development with:
- Clean Architecture & Domain-Driven Design
- SOLID and GRASP principles
- .NET Core Web API (80% backend focus)
- Angular frontend (20%)
- Multi-database strategy (SQL Server + PostgreSQL)

## 🏗️ Architecture

**Clean Architecture Layers:**
- **Domain**: Core entities and business rules
- **Application**: Use cases, DTOs, validation
- **Infrastructure**: Data access, external integrations
- **API**: Controllers, middleware
- **Web**: Angular SPA (coming soon)

## 🚀 Tech Stack

**Backend:**
- .NET 10.0
- Entity Framework Core
- SQL Server (transactional)
- PostgreSQL (reporting)
- FluentValidation
- xUnit

**Infrastructure:**
- Docker & Docker Compose
- Containerized development environment

## 📦 Project Status

**✅ Completed:**
- [x] Solution structure with Clean Architecture
- [x] Project dependencies configured
- [x] Docker Compose with SQL Server & PostgreSQL
- [x] Domain entities with business rules
- [x] Application layer (DTOs, validators, services)
- [x] Infrastructure layer (EF Core, repositories)
- [x] Database migrations and seeding
- [x] RESTful API controllers
- [x] Swagger/OpenAPI documentation
- [x] API testing via Swagger UI

**🚧 In Progress:**
- [ ] Angular frontend
- [ ] Unit tests
- [ ] Integration tests

**📋 Planned:**
- [ ] CI/CD pipeline
- [ ] Docker compose for full stack
- [ ] AWS Lambda integration (simulated)
- [ ] PostgreSQL reporting integration

## 🔄 CI/CD Pipeline

This project uses GitHub Actions for continuous integration and deployment:

- ✅ Automated building and testing on every push
- ✅ Unit and integration test execution
- ✅ Code coverage reporting
- ✅ Code quality and security scanning
- ✅ Docker image building and validation
- ✅ Dependency vulnerability checking

See [CI/CD Documentation](.github/README.md) for details.

![Build Status](https://github.com/Tabgyn/ContractPlanManager/workflows/CI-CD%20Pipeline/badge.svg)

## 📌 Versioning

This project follows [Semantic Versioning](https://semver.org/):
- **MAJOR**: Incompatible API changes
- **MINOR**: New functionality (backwards compatible)
- **PATCH**: Bug fixes (backwards compatible)

**Current Version**: v0.1.0 (Initial MVP)

**Release History**:
- v0.1.0 - Initial release with backend API, tests, Docker, and CI/CD

## 🛠️ Getting Started

### Prerequisites
- .NET 10 SDK
- Docker Desktop
- Git

### Setup

1. **Clone repository**
```bash
git clone <repository-url>
cd ContractPlanManager
```

2. **Start databases**
```bash
docker-compose up -d
```

3. **Verify database connectivity**
```bash
docker exec -it contract-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q "SELECT @@VERSION"
docker exec -it contract-postgres psql -U postgres -c "SELECT version();"
```

4. **Build solution**
```bash
dotnet build
```

5. **Run tests** (when available)
```bash
dotnet test
```

## 📁 Project Structure
```
ContractPlanManager/
├── src/
│   ├── Domain/           # Business entities, interfaces
│   ├── Application/      # Business logic, DTOs, validators
│   ├── Infrastructure/   # EF Core, repositories, external services
│   ├── API/              # REST API, controllers
│   └── Web/              # Angular app (upcoming)
├── tests/
│   ├── UnitTests/
│   └── IntegrationTests/
├── docker-compose.yml
└── README.md
```

## 🔐 Configuration

Database credentials (development only):
- SQL Server: `sa / YourStrong@Passw0rd`
- PostgreSQL: `postgres / YourStrong@Passw0rd`

**Note:** Change passwords for production use.

## 🌐 API Endpoints

### Contracts
- `GET /api/contracts` - Get all contracts
- `GET /api/contracts/active` - Get active contracts
- `GET /api/contracts/{id}` - Get contract by ID
- `GET /api/contracts/by-number/{contractNumber}` - Get contract by number
- `POST /api/contracts` - Create new contract
- `PUT /api/contracts/{id}` - Update contract
- `POST /api/contracts/{id}/terminate` - Terminate contract
- `POST /api/contracts/{id}/suspend` - Suspend contract
- `POST /api/contracts/{id}/reactivate` - Reactivate contract

### Payment Plans
- `GET /api/paymentplans` - Get all payment plans
- `GET /api/paymentplans/active` - Get active plans
- `GET /api/paymentplans/{id}` - Get plan by ID
- `POST /api/paymentplans` - Create new plan
- `PUT /api/paymentplans/{id}` - Update plan
- `POST /api/paymentplans/{id}/deactivate` - Deactivate plan
- `POST /api/paymentplans/{id}/reactivate` - Reactivate plan

### Plan Change Requests
- `GET /api/planchangerequests/pending` - Get pending requests
- `GET /api/planchangerequests/{id}` - Get request by ID
- `GET /api/planchangerequests/contract/{contractId}` - Get requests by contract
- `POST /api/planchangerequests` - Create change request
- `POST /api/planchangerequests/{id}/process` - Approve/reject request
- `POST /api/planchangerequests/{id}/cancel` - Cancel request

**API Documentation:** Available at Swagger UI when running the API (`http://localhost:5190/swagger`)

## 📝 Development Principles

- **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **GRASP**: Information Expert, Creator, Controller, Low Coupling, High Cohesion
- **Clean Code**: Meaningful names, small functions, clear intent

## 📄 License

MIT License - See LICENSE file for details

## 👤 Author

Tiago Azevedo Borges

---
