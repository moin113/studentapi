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
### Setup and Configuration

1. **Database Setup**:
   You have two options for setting up the database:
   
   **Option A: SQL Script (Recommended)**
   - Open SQL Server Management Studio (SSMS) or your preferred SQL tool.
   - Execute the `StudentManagement.sql` file provided in the root directory. This will create the `StudentManagementDb` and all necessary tables.
   
   **Option B: Entity Framework Migrations**
   - Open a terminal in the `StudentManagement` project folder.
   - Run: `dotnet ef database update --project ..\StudentManagement.Persistence\`

2. **Configuration**:
   - Open `StudentManagement/appsettings.json`.
   - Ensure the `ConnectionStrings.DefaultConnection` points to your SQL Server instance (e.g., `Server=localhost;Database=StudentManagementDb;Trusted_Connection=True;TrustServerCertificate=True;`).

3. **Running the API**:
   - Run: `dotnet run --project StudentManagement`
   - The API will be available at `http://localhost:5000` or `https://localhost:5001`.
   - Explore the API via Swagger at `http://localhost:5000/swagger`.

### API Documentation
The API is fully documented using Swagger XML comments. Each endpoint includes a description and expected response types.
- **Unwrapped Responses**: Successful responses now return the data object directly, while errors still return the structured `ApiResponse` wrapper.
- **Logging**: Detailed logging is implemented across all layers (Controllers, Services, Repositories).

---

## 🎨 Frontend Setup (React)

The frontend is a modern React application located in the `student-management-ui` folder.

### 1. Installation
Open a terminal in the frontend directory and install the dependencies:
```powershell
cd student-management-ui
npm install
```

### 2. Running Locally
Start the development server:
```powershell
npm run dev
```
The app will be available at `http://localhost:5173`.

---

## 🏗 Project Architecture & Security

### Project References
- **StudentManagement (API)**: Entry point, Controllers, and Middleware.
- **Application**: Business Logic, Interfaces, and DTOs.
- **Domain**: Core Entities.
- **Infrastructure**: JWT Generation and Logging.
- **Persistence**: Database context and Repository implementations.

### Security & Access Control
- **Authentication**: JWT Bearer tokens are required for the Students API.
- **Authorization**: **Only users with the "Admin" role can log in** and access student endpoints.
- **CORS**: The API is configured to allow requests from `http://localhost:5173`.

---

## 🔄 Request Flow (A to Z)
1. **Middleware**: Validates JWT and checks for Admin role.
2. **Controller**: Receives request and calls Service.
3. **Service**: Processes business logic and maps Entities to DTOs.
4. **Repository**: Fetches/Saves data in SQL Server via EF Core.


---

## 🔐 Security Note & Access Control

- **Authentication**: JWT Bearer tokens are required for all Student endpoints.
- **Authorization**: **Only users with the "Admin" role can log in** and access the dashboard.
- **CORS**: The API allows requests from `http://localhost:5173`.
- **Registration**: Open to all, but only "Admin" registered users can access the system.

---

## 🧩 Architectural Patterns Used

1.  **Repository Pattern**: Decouples business logic from data access code (e.g., `IStudentRepository`).
2.  **Service Pattern**: Orchestrates business operations and DTO mapping (e.g., `StudentService`).
3.  **Unit of Work**: Ensures atomsity by wrapping multiple repository operations into a single transaction.
4.  **Global Exception Handling**: A centralized middleware to catch all errors and return formatted JSON.
5.  **Clean Architecture**: Ensures dependency flow points inward toward the Domain.
