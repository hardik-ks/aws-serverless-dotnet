# AWS Serverless Application with .NET

A practical reference implementation demonstrating how to build scalable, maintainable, and cost-effective serverless applications using .NET and AWS cloud services.

This project showcases serverless application development using AWS Lambda, API Gateway, Amazon SQS, Amazon RDS (MySQL), and Dapper, following a clean, layered architecture with a strong focus on separation of concerns, dependency injection, and maintainable application design.

## Architecture Overview

The solution follows a layered architecture that separates AWS-specific execution concerns, business logic, data access, and shared infrastructure components.

**Architecture Layers:**

- **Lambda:** Handles AWS event triggers, request processing, and response mapping.
- **Application:** Contains business logic, service orchestration, and application workflows.
- **Repository:** Handles database operations and data access using Dapper.
- **Shared:** Provides reusable infrastructure components, configuration, logging, database connectivity, and common utilities.
- **Database:** MySQL hosted on Amazon RDS, with RDS Proxy for managed database connectivity.

## Technology Stack

| Category | Technologies |
|---|---|
| Language | C# |
| Framework | .NET |
| Compute | AWS Lambda |
| API | Amazon API Gateway |
| Messaging | Amazon SQS |
| Database | Amazon RDS (MySQL) |
| Data Access | Dapper |
| Connection Management | Amazon RDS Proxy |
| Configuration | AWS Systems Manager Parameter Store |
| Infrastructure as Code | AWS CloudFormation |
| Logging | Microsoft.Extensions.Logging |
| Dependency Injection | Microsoft.Extensions.DependencyInjection |

## Key Features

- Serverless application architecture using AWS Lambda.
- Event-driven processing using Amazon SQS.
- API integration through Amazon API Gateway.
- Scheduled background processing.
- Database access using Dapper and MySQL.
- Connection management through Amazon RDS Proxy.
- Centralized configuration using environment variables and AWS Systems Manager Parameter Store.
- Reusable shared infrastructure components.
- Dependency injection and separation of business logic.
- Centralized exception handling and standardized API responses.
- Infrastructure provisioning using AWS CloudFormation.

## Solution Structure

The solution is organized into independent layers to maintain clear responsibilities and improve reusability.

```text
src/
├── Lambda/
│   └── Entry points and AWS event handlers
│
├── Application/
│   └── Business logic and service orchestration
│
├── Repository/
│   └── Data access and database operations
│
└── Shared/
    └── Common infrastructure and reusable components

tests/
├── Application.Tests/
└── Repository.Tests/

infrastructure/
└── CloudFormation templates
```

## Architecture & Design Principles

The implementation follows these engineering principles:

- Separation of concerns and layered architecture.
- Dependency inversion and dependency injection.
- Reusable infrastructure components.
- Minimal Lambda execution pipeline without introducing a full ASP.NET Core application pipeline.
- Centralized configuration and logging.
- Consistent exception handling and API response contracts.
- Efficient database connectivity and query execution.
- Event-driven and asynchronous processing.

## Getting Started

### Prerequisites

- .NET SDK
- AWS CLI
- AWS account with appropriate permissions
- MySQL database
- Visual Studio or Visual Studio Code

### Local Development

Clone the repository:

```bash
git clone https://github.com/YOUR_USERNAME/aws-serverless-dotnet.git
cd aws-serverless-dotnet
```

Build the solution:

```bash
dotnet build
```

Run automated tests:

```bash
dotnet test
```

Additional setup instructions for local configuration, database initialization, and Lambda execution will be documented as the implementation evolves.

## Infrastructure & Deployment

AWS infrastructure is managed through CloudFormation templates to support repeatable and environment-specific deployments.

The deployment approach is designed to support multiple environments, such as Development, UAT, and Production.

## Roadmap

- [ ] Establish the layered solution architecture.
- [ ] Implement Lambda dependency injection and startup configuration.
- [ ] Integrate MySQL using Dapper.
- [ ] Implement RDS Proxy connectivity.
- [ ] Integrate AWS Systems Manager Parameter Store.
- [ ] Implement API Gateway integration.
- [ ] Implement SQS-based asynchronous processing.
- [ ] Implement scheduled Lambda execution.
- [ ] Add centralized logging and exception handling.
- [ ] Add unit and integration tests.
- [ ] Add CloudFormation infrastructure templates.
- [ ] Document local development and deployment workflows.

## License

This project is intended for learning, demonstration, and reference purposes. Licensing details will be added as the repository evolves.
