# Copilot Instructions for Loan Eligibility Service

## Project Overview
This is a Loan Eligibility Service built with .NET 8 and C# 12.0, following Clean Architecture principles with separate layers for API, Application, and Domain.

## Architecture & Project Structure

### Layer Organization
- **API Layer** (`LoanEligibilityService.API`): Contains controllers, HTTP endpoints, and API configuration
- **Application Layer** (`LoanEligibilityService.Application`): Contains services, DTOs, and business logic interfaces
- **Domain Layer** (`LoanEligibilityService.Domain`): Contains domain models and core business entities

### Folder Structure
- Controllers go in `Controllers/` folder
- Services go in `Services/` folder
- Interfaces go in `Interfaces/` folder
- DTOs go in `DTOs/` folder
- Domain models go in `Models/` folder

## Coding Standards

### Naming Conventions
- Use **PascalCase** for class names, method names, properties, and constants
- Use **camelCase** with underscore prefix (e.g., `_serviceName`) for private fields
- Use descriptive names that clearly indicate purpose
- Interface names should start with `I` (e.g., `ILoanEligibilityService`)
- Async methods should end with `Async` suffix

### File Organization
- One class per file
- File name must match the class name
- Using statements at the top, sorted alphabetically
- Namespace declaration using file-scoped namespaces where appropriate
- Order: using statements → namespace → class definition

### Class Structure Order
1. Constants (private const)
2. Fields (private readonly)
3. Constructor(s)
4. Public methods
5. Private methods

### Constants and Configuration
- Define magic numbers and strings as named constants at the top of the class
- Use `const` for compile-time constants
- Use descriptive constant names (e.g., `MinimumAge`, `MinimumCreditScore`)

### Dependency Injection
- Use constructor injection for all dependencies
- Register services in `Program.cs` with appropriate lifetime (`AddScoped`, `AddSingleton`, `AddTransient`)
- Prefer interface-based dependencies over concrete implementations

### API Controllers
- Inherit from `ControllerBase`
- Use `[ApiController]` attribute
- Use `[Route("api/[controller]")]` for routing
- Use `[HttpPost]`, `[HttpGet]`, etc. for action methods
- Include `[ProducesResponseType]` attributes to document responses
- Use `async`/`await` for all I/O operations
- Implement proper validation and return appropriate HTTP status codes
- Use structured logging with `ILogger<T>`

### Service Layer
- Implement services through interfaces defined in `Interfaces/` folder
- Business logic should be in the Application layer services
- Use async methods returning `Task<T>` for operations
- Keep methods focused and single-purpose
- Extract validation logic into separate private methods

### DTOs (Data Transfer Objects)
- Use POCOs with public properties
- Initialize collection properties to empty collections (e.g., `new List<string>()`)
- Initialize string properties to `string.Empty` to avoid null reference issues
- DTOs should be in the `DTOs/` folder within the Application layer

### Domain Models
- Domain models should represent core business entities
- Use constructors to ensure required properties are set
- Keep domain models in the Domain layer, separate from DTOs

### Validation
- Perform basic validation in controllers
- Implement business rule validation in services
- Return clear, descriptive error messages
- Use `BadRequest()` for invalid input, `Ok()` for successful operations

### Logging
- Inject `ILogger<T>` where T is the class name
- Use structured logging with named parameters
- Log at appropriate levels (Information, Warning, Error)
- Log important business events (e.g., eligibility checks)

### Error Handling
- Check for null inputs and return appropriate error responses
- Provide meaningful error messages
- Use list of reasons for complex validation failures

### Async/Await
- Use `async`/`await` for all I/O operations
- Methods performing async operations should return `Task<T>`
- Don't use `.Result` or `.Wait()` - use await instead

### Comments and Documentation
- Code should be self-documenting through clear naming
- Add comments only when necessary to explain complex business logic
- Avoid obvious comments that merely repeat what the code does

### Modern C# Features
- Use C# 12.0 features where appropriate
- Prefer expression-bodied members for simple methods (e.g., `private bool IsAgeEligible(int age) => age >= MinimumAge;`)
- Use collection initializers for collections
- Use string interpolation for building messages

### Testing Considerations
- Write testable code by using dependency injection
- Keep business logic separate from infrastructure concerns
- Use interfaces to enable mocking in tests

## .NET 8 Specific
- Target .NET 8 for all projects
- Use minimal API approach in `Program.cs`
- Leverage .NET 8 performance improvements

## Development Environment
- IDE: Visual Studio 2026
- Shell: PowerShell
- Git workflow: Feature branches merged to main

## Best Practices
1. Follow SOLID principles
2. Keep controllers thin, services focused
3. Separate concerns across architectural layers
4. Use dependency injection throughout
5. Write clean, readable, maintainable code
6. Prefer composition over inheritance
7. Make code easy to test and mock
