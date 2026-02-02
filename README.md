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
- [x] NuGet packages installed

**🚧 In Progress:**
- [ ] Domain entities
- [ ] Application services
- [ ] Data access layer
- [ ] REST API controllers
- [ ] Angular frontend

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

## 📝 Development Principles

- **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **GRASP**: Information Expert, Creator, Controller, Low Coupling, High Cohesion
- **Clean Code**: Meaningful names, small functions, clear intent

## 📄 License

MIT License - See LICENSE file for details

## 👤 Author

Tiago Azevedo Borges

---

**Status:** Phase 1 - Infrastructure Setup ✅ | Next: Domain Layer Development