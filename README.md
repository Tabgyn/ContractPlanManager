# Contract Plan Manager

Enterprise-grade fullstack application for managing contract payment plan modifications.

![Build Status](https://github.com/Tabgyn/ContractPlanManager/workflows/CI-CD%20Pipeline/badge.svg)

## 🎯 Purpose

Demonstrates senior-level fullstack development with:
- Clean Architecture & Domain-Driven Design
- SOLID and GRASP principles
- .NET 10.0 Web API (80% backend focus)
- Angular 21 SPA (20% frontend)
- Multi-database strategy (SQL Server + PostgreSQL)
- Comprehensive testing strategy (70%+ coverage)
- CI/CD pipeline with GitHub Actions
- Docker containerization

## 🏗️ Architecture

**Clean Architecture Layers:**
- **Domain**: Core entities and business rules
- **Application**: Use cases, DTOs, validation
- **Infrastructure**: Data access, external integrations
- **API**: Controllers, middleware
- **Web**: Angular SPA with Material Design
```
┌─────────────────────────────┐
│   Angular 21 SPA            │
│   - Material Design         │
│   - Reactive Forms          │
│   - RxJS State Management   │
└──────────────┬──────────────┘
               │ HTTP/JSON
┌──────────────▼──────────────┐
│   .NET 10.0 Web API         │
│   - RESTful Endpoints       │
│   - FluentValidation        │
│   - Swagger/OpenAPI         │
└──────────────┬──────────────┘
               │
       ┌───────┴────────┐
       ▼                ▼
┌──────────────┐  ┌──────────────┐
│ SQL Server   │  │ PostgreSQL   │
│ (Primary DB) │  │ (Reports)    │
└──────────────┘  └──────────────┘
```

## 🚀 Tech Stack

### Backend (80%)
- **.NET 10.0** - Latest LTS version
- **Entity Framework Core 10.0** - ORM and migrations
- **SQL Server** - Transactional data
- **PostgreSQL** - Reporting and analytics ready
- **FluentValidation** - Input validation
- **xUnit** - Unit testing
- **FluentAssertions** - Readable test assertions
- **Moq** - Mocking framework

### Frontend (20%)
- **Angular 21** - Latest stable version
- **TypeScript 5.x** - Type-safe JavaScript
- **Angular Material 21** - Material Design components
- **RxJS 7.x** - Reactive programming
- **SCSS** - Styling

### Infrastructure & DevOps
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **GitHub Actions** - CI/CD pipeline
- **Coverlet** - Code coverage
- **DevSkim** - Security scanning

## 📦 Project Status

**✅ COMPLETE:**
- [x] Clean Architecture implementation
- [x] Domain entities with business rules
- [x] Application layer (DTOs, validators, services)
- [x] Infrastructure layer (EF Core, repositories)
- [x] RESTful API with 20+ endpoints
- [x] Comprehensive unit tests (70%+ coverage)
- [x] Integration tests with real API
- [x] Docker containerization
- [x] CI/CD pipeline with GitHub Actions
- [x] Angular 19 frontend with Material Design
- [x] Contract management UI
- [x] Payment plan management UI
- [x] Plan change request workflow
- [x] Complete CRUD operations
- [x] Responsive design

## 🌐 Features

### Contract Management
- ✅ View all contracts with filtering
- ✅ Create new contracts
- ✅ View detailed contract information
- ✅ Suspend/reactivate contracts
- ✅ Terminate contracts
- ✅ Request plan changes

### Payment Plan Management
- ✅ View all available plans
- ✅ Card-based grid layout
- ✅ Activate/deactivate plans
- ✅ Tier-based organization (Basic, Standard, Premium, Enterprise)
- ✅ Pricing and billing cycle display

### Plan Change Requests
- ✅ Submit change requests
- ✅ View pending requests
- ✅ Approve/reject requests
- ✅ View request history
- ✅ Upgrade/downgrade indicators
- ✅ Cancel pending requests

## 🛠️ Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22.x LTS](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

### Quick Start with Docker (Recommended)

**1. Clone repository**
```bash
git clone https://github.com/Tabgyn/ContractPlanManager.git
cd ContractPlanManager
```

**2. Start backend services**
```bash
docker-compose up -d
```

This starts:
- SQL Server on port 1433
- PostgreSQL on port 5432
- .NET API on port 5000

**3. Start frontend (separate terminal)**
```bash
cd src/Web
npm install
npm start
```

**4. Access the application**
- **Frontend**: http://localhost:4200
- **API Swagger**: http://localhost:5000
- **API Base**: http://localhost:5000/api

### Local Development (without Docker)

**1. Start databases only**
```bash
docker-compose up -d sqlserver postgres
```

**2. Run backend API**
```bash
dotnet run --project src/API
```

**3. Run frontend**
```bash
cd src/Web
npm install
npm start
```

**4. Run tests**
```bash
# Unit tests
dotnet test tests/UnitTests

# Integration tests
dotnet test tests/IntegrationTests

# All tests with coverage
dotnet test /p:CollectCoverage=true
```

## 🔄 CI/CD Pipeline

Automated pipeline with GitHub Actions:
- ✅ Build and test on every push
- ✅ Unit and integration test execution
- ✅ Code coverage reporting (70%+)
- ✅ Code quality analysis (dotnet format)
- ✅ Security scanning (DevSkim)
- ✅ Docker image building and validation
- ✅ Dependency vulnerability checking
- ✅ Automated release notes

See [CI/CD Documentation](.github/CI-CD.md) for details.

## 🌐 API Endpoints

### Contracts
- `GET /api/contracts` - Get all contracts
- `GET /api/contracts/active` - Get active contracts only
- `GET /api/contracts/{id}` - Get contract by ID
- `GET /api/contracts/by-number/{number}` - Get by contract number
- `POST /api/contracts` - Create new contract
- `PUT /api/contracts/{id}` - Update contract
- `POST /api/contracts/{id}/suspend` - Suspend contract
- `POST /api/contracts/{id}/reactivate` - Reactivate contract
- `POST /api/contracts/{id}/terminate` - Terminate contract

### Payment Plans
- `GET /api/paymentplans` - Get all payment plans
- `GET /api/paymentplans/active` - Get active plans only
- `GET /api/paymentplans/{id}` - Get plan by ID
- `POST /api/paymentplans` - Create new plan
- `PUT /api/paymentplans/{id}` - Update plan
- `POST /api/paymentplans/{id}/deactivate` - Deactivate plan
- `POST /api/paymentplans/{id}/reactivate` - Reactivate plan

### Plan Change Requests
- `GET /api/planchangerequests/pending` - Get pending requests
- `GET /api/planchangerequests/{id}` - Get request by ID
- `GET /api/planchangerequests/contract/{contractId}` - Get by contract
- `POST /api/planchangerequests` - Create change request
- `POST /api/planchangerequests/{id}/process` - Approve/reject
- `POST /api/planchangerequests/{id}/cancel` - Cancel request

**Full API Documentation**: http://localhost:5000/swagger

## 📁 Project Structure
```
ContractPlanManager/
├── src/
│   ├── Domain/              # Business entities, enums, interfaces
│   ├── Application/         # DTOs, validators, services
│   ├── Infrastructure/      # EF Core, repositories, data access
│   ├── API/                 # Controllers, middleware, Swagger
│   └── Web/                 # Angular 19 SPA
│       ├── core/            # Services, models, interceptors
│       ├── features/        # Feature modules (contracts, plans, requests)
│       └── shared/          # Shared components, layout
├── tests/
│   ├── UnitTests/          # Domain & application tests
│   └── IntegrationTests/   # API integration tests
├── .github/
│   └── workflows/          # CI/CD pipelines
├── docker-compose.yml      # Multi-container orchestration
├── DOCKER.md               # Docker setup guide
└── README.md
```

## 🔐 Configuration

**Development (Docker):**
- **SQL Server**: `localhost:1433` / `sa` / `YourStrong@Passw0rd`
- **PostgreSQL**: `localhost:5432` / `postgres` / `YourStrong@Passw0rd`
- **API**: `localhost:5000`
- **Frontend**: `localhost:4200`

⚠️ **Production**: Change all passwords and use secure secrets management.

## 📝 Development Principles

### SOLID Principles
- **Single Responsibility** - Each class has one reason to change
- **Open/Closed** - Open for extension, closed for modification
- **Liskov Substitution** - Subtypes are substitutable
- **Interface Segregation** - Many specific interfaces
- **Dependency Inversion** - Depend on abstractions

### GRASP Patterns
- **Information Expert** - Assign to information holders
- **Creator** - Who creates objects
- **Controller** - Coordinates operations
- **Low Coupling** - Minimize dependencies
- **High Cohesion** - Related functionality together

### Clean Code
- Meaningful names
- Small, focused functions
- Clear intent
- Comprehensive tests
- No code duplication

## 🧪 Testing Strategy

- **Unit Tests**: 40+ tests covering domain and application logic
- **Integration Tests**: 15+ tests for full API stack
- **Test Coverage**: 70%+ on critical business logic
- **Frameworks**: xUnit, FluentAssertions, Moq
- **Patterns**: AAA (Arrange-Act-Assert), Theory/InlineData

## 📊 Key Metrics

| Metric | Value |
|--------|-------|
| **Total Lines of Code** | ~5,000+ |
| **Backend Coverage** | 70%+ |
| **API Endpoints** | 20+ |
| **Unit Tests** | 40+ |
| **Integration Tests** | 15+ |
| **Components** | 15+ |
| **Services** | 6+ |

## 🎯 Alignment with Job Requirements

**Position**: Fullstack Sênior | C# | .NET | Angular

### Backend (80% - ✅ Complete)
- ✅ Advanced .NET 10.0 (C#) implementation
- ✅ SQL Server primary database
- ✅ PostgreSQL integration ready
- ✅ Clean Code, SOLID, GRASP principles
- ✅ CI/CD pipeline
- ✅ Docker containerization
- ✅ Comprehensive testing

### Frontend (20% - ✅ Complete)
- ✅ Angular 21 application
- ✅ Material Design UI
- ✅ Reactive Forms
- ✅ RxJS state management
- ✅ API integration
- ✅ Responsive design

### Differentials
- ✅ DevOps practices
- ✅ Automated testing
- ✅ Production-ready architecture

## 📌 Versioning

Follows [Semantic Versioning](https://semver.org/):

**Current Version**: v1.0.0 (Production Ready)

**Release History**:
- v1.0.0 - Complete fullstack application with backend and frontend
- v0.1.0 - Initial backend MVP

## 🚀 Future Enhancements

- [ ] AWS Lambda integration for notifications
- [ ] Advanced PostgreSQL reporting dashboard
- [ ] User authentication and authorization
- [ ] Role-based access control
- [ ] Email notifications
- [ ] Export to PDF/Excel
- [ ] Audit logging
- [ ] Performance monitoring

## 📄 License

MIT License - See LICENSE file for details

## 👤 Author

**Tiago Azevedo Borges**
- GitHub: [@Tabgyn](https://github.com/Tabgyn)

## 🙏 Acknowledgments

Built to demonstrate senior fullstack development skills with emphasis on:
- Enterprise software architecture
- Backend development expertise (80%)
- Modern frontend development (20%)
- DevOps and automation
- Clean code and best practices

---

**Status**: ✅ **PRODUCTION READY**

*A complete, production-ready demonstration of enterprise fullstack development with .NET 10.0 and Angular 21.*