# Syria News

Syria News is a web-based news platform built with ASP.NET Core (targeting .NET 9). It provides a RESTful API for managing news articles, newspapers, members, and administrative tasks. The project is designed with modularity, security, and scalability in mind.

## Features

- **Authentication & Authorization:**  
  Supports JWT-based authentication, user registration (members and newspapers), email confirmation, password reset, and refresh tokens.

- **Role-based Access:**  
  Separate controllers and endpoints for Admins and Members, with fine-grained access control.

- **Article Management:**  
  CRUD operations for articles, categories, tags, comments, likes, and saves.

- **User Management:**  
  Profile management for members and newspapers, including profile images and following/followers.

- **Admin Tools:**  
  Admins can manage users, articles, categories, tags, notifications, and more.

- **Health Checks & Monitoring:**  
  Integrated health checks and Hangfire dashboard for background jobs.

- **Logging:**  
  Uses Serilog for structured logging to both console and file.

- **CORS & Security:**  
  Configurable CORS, HTTPS redirection, and rate limiting.

## Project Structure

- `Controllers/` - API endpoints for authentication, admin, and member operations.
- `Models/`, `DTOs/` - Data models and transfer objects.
- `Repository/`, `UnitOfWork/` - Data access and business logic.
- `Validations/` - Input validation using FluentValidation.
- `AutoMapper/` - Object mapping profiles.
- `HelperTools/`, `Errors/`, `ExceptionHandler/` - Utilities and error handling.
- `wwwroot/` - Static files, including article and profile images.
- `Documentation/` - Project notes and TODOs.

## Configuration

- **Database:**  
  Uses SQL Server (see `appsettings.json` for connection strings).

- **Logging:**  
  Configured via Serilog (see `appsettings.json`).

- **Mail:**  
  SMTP settings for email confirmation and notifications.

- **Images:**  
  Configurable size and format restrictions for article and profile images.

- **Hangfire:**  
  Background job processing and dashboard (protected with basic authentication).

## Getting Started

1. **Clone the repository**
2. **Configure `appsettings.json`** with your database, mail, and Hangfire credentials.
3. **Restore NuGet packages** and build the solution.
4. **Run database migrations** if needed.
5. **Start the application** (`dotnet run` or via Visual Studio).

## API Overview

- **/auth/** - Authentication and registration endpoints.
- **/api/admins/** - Admin operations (requires Admin role).
- **/api/members/** - Member operations (requires Member role).
- **/jobs** - Hangfire dashboard (admin only).
- **/health** - Health check endpoint.
