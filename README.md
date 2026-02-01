# Employee Management System

A comprehensive ASP.NET Core MVC application for managing employees with role-based access control, claims-based authorization, and external authentication providers.

## Features

### Core Functionality

- Employee Management: Create, read, update, and delete employee records
- User Authentication: Local account registration and login with email confirmation
- External Authentication: Sign in with Google and Facebook
- Role-Based Access Control: Admin, Super Admin role management
- Claims-Based Authorization: Fine-grained permissions using custom claims
- Password Management: Change password, reset password, add password for external login users
- Account Lockout: Automatic lockout after failed login attempts
- Data Protection: Encrypted employee IDs in URLs
- Photo Upload: Employee profile photo management

### Security Features

- Email confirmation required for new accounts
- Custom email confirmation token provider with extended lifespan
- Account lockout after 5 failed attempts (15-minute duration)
- Password requirements: Minimum 8 characters, 2 unique characters
- Claims-based authorization for administrative actions
- Custom authorization handlers for role management

### Technologies Used

- Framework: ASP.NET Core 10.0 (MVC)
- Database: SQL Server with Entity Framework Core
- Authentication: ASP.NET Core Identity
- External Auth: Google OAuth 2.0, Facebook Login
- Logging: NLog
- Frontend: Bootstrap 5, Razor Views
- Security: Data Protection API, Custom Authorization Handlers

### Prerequisites

- .NET 10.0 SDK or later
- SQL Server (LocalDB, Express, or full version)
- Visual Studio Code or Visual Studio 2022
- Google Cloud Console account (for Google authentication)
- Facebook Developer account (for Facebook authentication)

## Getting Started

1. Clone the Repository

```bash
git clone <repository-url>
cd EmployeeManagement
```

2. Setup User Secrets
   This application uses User Secrets to store sensitive configuration data. Never commit secrets to source control.
   Initialize User Secrets

```bash
dotnet user-secrets init
```

Add Database Connection String

```bash
dotnet user-secrets set "ConnectionStrings:EmployeeDBConnection" "Server=(localdb)\\mssqllocaldb;Database=EmployeeDB;Trusted_Connection=true;MultipleActiveResultSets=true"
```

For SQL Server Express, use:

```bash
dotnet user-secrets set "ConnectionStrings:EmployeeDBConnection" "Server=localhost\\SQLEXPRESS;Database=EmployeeDB;Trusted_Connection=true;MultipleActiveResultSets=true"
```

For SQL Server with credentials:

```bash
dotnet user-secrets set "ConnectionStrings:EmployeeDBConnection" "Server=your-server;Database=EmployeeDB;User Id=your-username;Password=your-password;MultipleActiveResultSets=true"
```

Add Google Authentication Credentials

1. Go to Google Cloud Console
2. Create a new project or select an existing one
3. Go to Credentials → Create Credentials → OAuth 2.0 Client ID
4. Set application type to "Web application"
5. Add authorized redirect URI: http://localhost:5181/signin-google
6. Copy the Client ID and Client Secret

```bash
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"
```

Add Facebook Authentication Credentials

1. Go to Facebook Developers
2. Create a new app or select an existing one
3. Add Facebook Login product
4. Go to Settings → Basic
5. Add platform: Website
6. Set Site URL: https://localhost:5181
7. In Facebook Login Settings, add Valid OAuth Redirect URI: https://localhost:5181/signin-facebook
8. Copy the App ID and App Secret

```bash
dotnet user-secrets set "Authentication:Facebook:AppId" "your-facebook-app-id"
dotnet user-secrets set "Authentication:Facebook:AppSecret" "your-facebook-app-secret"
```

Verify User Secrets

```bash
dotnet user-secrets list
```

You should see:

```bash
Authentication:Facebook:AppId = your-facebook-app-id
Authentication:Facebook:AppSecret = your-facebook-app-secret
Authentication:Google:ClientId = your-google-client-id
Authentication:Google:ClientSecret = your-google-client-secret
ConnectionStrings:EmployeeDBConnection = Server=...;Database=EmployeeDB;...
```

3. Database Setup
   Apply Migrations
   The project uses Entity Framework Core Code-First approach. Apply migrations to create the database:

```bash
# Restore NuGet packages
dotnet restore

# Apply migrations and create database
dotnet ef database update
```

If you don't have the dotnet-ef tool installed:

```bash
dotnet tool install --global dotnet-ef
```

Create Migrations (Optional - for development)
If you need to create new migrations after model changes:

```bash
# Add a new migration
dotnet ef migrations add YourMigrationName

# Apply the migration
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

Seed Initial Data (Manual)
After the database is created, you'll need to manually create:

1. Admin User:
   - Register a new user through the application
   - Manually add to Admin role using SQL or create through code

2. Initial Roles:

```sql
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES
(NEWID(), 'Admin', 'ADMIN', NEWID()),
(NEWID(), 'Super Admin', 'SUPER ADMIN', NEWID());
```

3. Claims (Available claims are defined in Models/ClaimsStore.cs):
   - Create Role
   - Edit Role
   - Delete Role

4. Run the Application

```bash
dotnet run


Or using Visual Studio:
- Press `F5` or click "Run"

The application will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## Project Structure

EmployeeManagement/
├── Controllers/
│   ├── AccountController.cs          # Authentication & user management
│   ├── AdministrationController.cs   # Role & claims management
│   └── HomeController.cs             # Employee CRUD operations
|
├── Models/
│   ├── ApplicationUser.cs            # Extended Identity user
│   ├── Employee.cs                   # Employee entity
│   ├── AppDbContext.cs               # Database context
│   ├── IEmployeeRepository.cs        # Employee Interface
│   ├── SQLEmployeeRepository.cs      # Implementation of the IEmployeeRepository interface
│   └── ClaimsStore.cs                # Available claims
|
├── ViewModels/                       # View models for forms
├── Security/
│   ├── CustomEmailConfirmationTokenProvider.cs
│   ├── DataProtectionPurposeStrings.cs
│   └── Authorization Handlers/
├── Views/                            # Razor views
├── wwwroot/                          # Static files
└── Program.cs                        # Application configuration
```

## Configuration

### Service Lifetimes

The application uses three types of service lifetimes:
**Scoped Services** (One instance per HTTP request):

```csharp
services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
```

- Database contexts
- Repositories
- Per-request state

**Singleton Services** (One instance for entire application):

```csharp
services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
services.AddSingleton<DataProtectionPurposeStrings>();
```

- Authorization handlers
- Configuration services
- Stateless services

**Transient Services** (New instance every time):

- Used for lightweight, stateless services (not currently used in this project)

### Authorization Policies

Defined in Program.cs:

- **DeleteRolePolicy**: Requires "Delete Role" claim with value "true"
- **EditRolePolicy**: Custom requirement - Admin with Edit Role claim, or Super Admin
- **AdminRolePolicy**: Requires Admin role
- **TestRolePolicy**: Requires both Delete Role and Create Role claims

### Identity Configuration

```csharp
options.Password.RequiredLength = 8;
options.Password.RequiredUniqueChars = 2;
options.SignIn.RequireConfirmedEmail = true;
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
```

### Token Lifespans

- Email Confirmation Token: 3 days
- Default Tokens (password reset, etc.): 5 hours

## Usage

### User Registration

1. Navigate to /Account/Register
2. Fill in email, password, and city
3. Check email for confirmation link
4. Click confirmation link to activate account
5. Login at /Account/Login

### External Login

1. Click "Google" or "Facebook" button on login page
2. Authorize the application
3. If first time: Account is automatically created
4. If email not confirmed: Confirmation link is logged (check console/logs)

### Managing Employees

**List Employees**: Navigate to home page
**Create Employee**: Click "Create" → Fill form → Upload photo (optional)
**View Details**: Click employee name
**Edit Employee**: Click "Edit" on details page
**Delete Employee**: Not implemented (can be added)

### Managing Users (Admin Only)

**List Users**: Navigate to /Administration/ListUsers
**Edit User**: Click "Edit" → Update email, username, city
**Manage Roles**: Click "Manage Roles" → Select/deselect roles
**Manage Claims**: Click "Manage Claims" → Select/deselect claims
**Delete User**: Click "Delete" → Confirm

### Managing Roles (Admin Only)

**List Roles**: Navigate to /Administration/ListRoles
**Create Role**: Click "Create Role" → Enter role name
**Edit Role**: Click role name → Update name and users
**Delete Role**: Click "Delete" (requires Delete Role claim)

### Password Management

**Change Password**: /Account/ChangePassword (for users with local password)
**Add Password**: /Account/AddPassword (for external login users)
**Forgot Password**: Click "Forgot Password" on login page
**Reset Password**: Use link from forgot password email

### Troubleshooting

Database Connection Issues
Error: "Cannot open database 'EmployeeDB'"
Solution:

```bash
# Check if SQL Server is running
# Verify connection string in user secrets
dotnet user-secrets list

# Recreate database
dotnet ef database drop
dotnet ef database update
```

**Email Confirmation Link Not Working**
Issue: Confirmation links are logged to console in development
Solution:

- Check application logs/console output for the confirmation link
- In production, configure an email service (SMTP, SendGrid, etc.)
- Implement IEmailService to send actual emails

**External Login Not Working**
Google/Facebook returns error:

- Verify redirect URIs in provider console match exactly
- Check client ID and secret in user secrets
- Ensure application is running on HTTPS
- Check that APIs are enabled in Google Cloud Console

**Authorization Issues**
Access Denied even as Admin:

1. Verify user has the required claims:

```sql
SELECT * FROM AspNetUserClaims WHERE UserId = 'your-user-id'
```

2. Check policy requirements in Program.cs
3. Verify authorization handler logic

**Migration Issues**
Error: "No migrations found"
Solution:

```bash
# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

## Development

### Adding New Features

1. Models: Add to `/Models` directory
2. ViewModels: Add to `/ViewModels` for form binding
3. Controllers: Add to `/Controllers`
4. Views: Add to `/Views/{ControllerName}`
5. Update DbContext: Add `DbSet<YourModel>` if needed
6. Create Migration: `dotnet ef migrations add YourFeatureName`
7. Update Database: `dotnet ef database update`

### Code Style

- Use dependency injection for all services
- Follow repository pattern for data access
- Use view models for form submission
- Implement proper validation (both client and server side)
- Handle errors gracefully with try-catch blocks
- Log important operations using ILogger

### Security Best Practices

✅ Implemented:

- User secrets for sensitive data
- Email confirmation required
- Account lockout after failed attempts
- Password requirements enforced
- Claims-based authorization
- Data protection for sensitive IDs
- HTTPS enforcement (production)
- CSRF protection (built-in)

⚠️ Recommendations for Production:

- Configure real email service (SendGrid, AWS SES, etc.)
- Enable two-factor authentication
- Implement rate limiting
- Add CAPTCHA for registration/login
- Set up proper logging and monitoring
- Use a secrets manager (Azure Key Vault, AWS Secrets Manager)
- Configure HTTPS certificate
- Enable HSTS (HTTP Strict Transport Security)

### License

This project is for educational purposes from the Kudvenkat dotnet core series. Modify as needed for your use case.
Support
