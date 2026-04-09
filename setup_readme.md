# Student Management System API

A professional, production-ready ASP.NET Core Web API built with **Clean Architecture**, implementing Secure JWT Authentication, Repository/Service patterns, and Global Exception Handling.

---

## 🚀 Quick Setup Guide

### Prerequisites
- **.NET 8.0 SDK**
- **SQL Server** (LocalDB or Express)
- **Visual Studio 2022** (or VS Code)

### 1. Database Configuration
Open `StudentManagement/appsettings.json` and update the `DefaultConnection` string if your local SQL server instance uses a different name:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=StudentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2. Apply Migrations (if using EF Migrations) or Database Schema
If the database doesn't exist, ensure you run the `Update-Database` command in the Package Manager Console (targeting the Persistence project) or manually execute the schema scripts.

### 3. Run the Project
1. Set `StudentManagement` as the StartUp project.
2. Press `F5` or run `dotnet run` in the terminal.
3. Swagger will automatically open at `https://localhost:[port]/swagger`.

---

## 🏗 Project Architecture & References

This solution follows **Clean Architecture** principles to ensure high maintainability and testability.

| Project | Role | Dependencies |
| :--- | :--- | :--- |
| **StudentManagement** | **Web API Layer**: Controllers, Middleware, Configuration. | Application, Infrastructure, Persistence |
| **StudentManagement.Application** | **Core Logic**: Interfaces, Services, DTOs, Validators. | Domain |
| **StudentManagement.Infrastructure** | **External Services**: JWT Generation, Logging. | Application |
| **StudentManagement.Persistence** | **Data Access**: DbContext, Repositories, Unit of Work. | Domain, Application |
| **StudentManagement.Domain** | **Core Entities**: POCO classes, Base entities. | *None* |

---

## 🧩 Architectural Patterns Used

1.  **Repository Pattern**: Decouples business logic from data access code (e.g., `IStudentRepository`).
2.  **Service Pattern**: Orchestrates business operations and DTO mapping (e.g., `StudentService`).
3.  **Unit of Work**: Ensures atomsity by wrapping multiple repository operations into a single transaction.
4.  **Global Exception Handling**: A centralized middleware to catch all errors and return formatted JSON.
5.  **Clean Architecture**: Ensures dependency flow points inward toward the Domain.

---

## 🔄 Request Flow Walkthrough (A to Z)

Here is a step-by-step trace of what happens when you call **`GET /api/Students`**:

### 1. Middleware Layer (Pre-Processing)
- **Request Arrives**: The HTTP request hits the `GlobalExceptionMiddleware`. It wraps the entire request in a `try-catch` block.
- **Authentication**: The `JwtBearer` middleware extracts the token from the `Authorization` header. It validates the signature, issuer, and expiry.
- **Authorization**: The `Authorization` middleware checks if the user has the required claims/roles to access the endpoint.

### 2. API Layer (The Controller)
- The request reaches `StudentsController.GetAll()`.
- The Controller validates nothing but the presence of required parameters.
- It calls `_studentService.GetAllStudentsAsync()`.

### 3. Application Layer (The Service)
- `StudentService` receives the request.
- It interacts with the `IUnitOfWork` to request the database data.
- **Business Logic**: This layer filters out "deleted" students and performs any ordering required.
- **Mapping**: Data from the database (Entities) is mapped to `StudentResponse` (DTOs).
- It returns a standardized `ApiResponse<T>` object.

### 4. Persistence Layer (The Repository)
- `StudentRepository` executes the `LINQ` query against the `StudentManagementDbContext`.
- EF Core translates this to a SQL `SELECT` statement and executes it on **SQL Server**.

### 5. Response Flow (Back to Client)
- The Repository returns the data to the Service.
- The Service returns the `ApiResponse` to the Controller.
- The Controller returns `Ok(result)`, which converts the object to a **JSON** response.
- **Middleware Exit**: The response flows back through the middlewares. If an error had occurred, the `GlobalExceptionMiddleware` would have intercepted it here and returned a `500 Server Error` JSON instead.

---

## 🔐 Security Note
- Users must **Register** with a `role` (Admin or User).
- Only users with the **Admin** role can access the `DELETE` students endpoint.
- Registration allows anyone to set their role for testing purposes (this is intended for development verification).
