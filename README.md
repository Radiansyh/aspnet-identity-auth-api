# 🔐 Secure Authentication API - ASP.NET Core 8

A production-ready RESTful API for user authentication and authorization featuring JWT tokens, refresh tokens, audit logging, and rate limiting.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build](https://img.shields.io/badge/Build-Passing-brightgreen.svg)]()

## ✨ Features

### Core Authentication
- 🔑 **JWT Token Authentication** - Stateless authentication with role-based authorization
- 🔄 **Refresh Token System** - Automatic token rotation with 7-day expiry
- 👤 **User Management** - Registration, login, and profile management
- 🛡️ **Password Security** - ASP.NET Identity with secure hashing

### Advanced Security
- 📊 **Login Audit Logging** - Track all authentication events (success, failure, refresh, logout)
- 🚦 **Rate Limiting** - Prevent brute-force attacks (5 req/min on auth endpoints)
- 🎭 **Role-Based Access Control** - Admin and User roles with granular permissions
- ⚠️ **Global Exception Handling** - Consistent error responses across API

### Developer Experience
- 📚 **Swagger/OpenAPI** - Interactive API documentation with JWT support
- 🏗️ **Clean Architecture** - Layered design following SOLID principles
- 🗃️ **SQLite Database** - File-based database with auto-migrations
- 📝 **Comprehensive Documentation** - API examples and testing guides

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Code editor (Visual Studio 2022, VS Code, or Rider)
- Optional: [DB Browser for SQLite](https://sqlitebrowser.org/)

### Installation

```bash
# Clone the repository
git clone https://github.com/Radiansyh/aspnet-identity-auth-api.git
cd aspnet-identity-auth-api/ApiAuth

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The API will start at:
- **HTTPS**: `https://localhost:7xxx`
- **Swagger UI**: `https://localhost:7xxx/swagger`

The SQLite database (`auth.db`) is created automatically on first run with:
- ✅ Identity tables
- ✅ Admin and User roles
- ✅ Default admin account (see below)

### Default Admin Credentials

The application automatically creates an admin user on first run:

```
📧 Email: admin@apiauth.com
🔑 Password: Admin@123
👑 Roles: Admin, User
```

**⚠️ Important:** Change these credentials immediately in production!

## 📖 API Documentation

### Authentication Endpoints

#### 1️⃣ Register New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response (200 OK)**
```json
{
  "userId": "guid",
  "email": "john@example.com",
  "fullName": "John Doe",
  "token": "eyJhbGci...",           // Access token (60 min)
  "refreshToken": "Kx7Mv9...",     // Refresh token (7 days)
  "roles": ["User"]
}
```

#### 2️⃣ Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

🚦 **Rate Limited**: 5 requests per minute per IP

**Response**: Same as registration

#### 3️⃣ Refresh Access Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "your-refresh-token"
}
```

🚦 **Rate Limited**: 5 requests per minute per IP

**Features:**
- ♻️ Old refresh token is automatically revoked
- 🔄 New refresh token issued
- ⏱️ Access token expires after 60 minutes
- 📅 Refresh token expires after 7 days

#### 4️⃣ Logout
```http
POST /api/auth/logout
Authorization: Bearer {access-token}
```

**Response (200 OK)**
```json
{
  "message": "Logged out successfully"
}
```

**Behavior**: Revokes all refresh tokens for the user

#### 5️⃣ Get Current User
```http
GET /api/auth/me
Authorization: Bearer {access-token}
```

**Response (200 OK)**
```json
{
  "userId": "guid",
  "email": "john@example.com",
  "fullName": "John Doe",
  "roles": ["User"]
}
```

#### 6️⃣ Get All Users (Admin Only)
```http
GET /api/auth/admin/users
Authorization: Bearer {admin-access-token}
```

**Response (200 OK)**
```json
[
  {
    "userId": "guid-1",
    "email": "user@example.com",
    "fullName": "Regular User",
    "roles": ["User"]
  },
  {
    "userId": "guid-2",
    "email": "admin@apiauth.com",
    "fullName": "System Administrator",
    "roles": ["Admin", "User"]
  }
]
```

## 🔒 Security Features

### Refresh Token System

| Feature | Description |
|---------|-------------|
| **Token Generation** | Cryptographically secure using `RandomNumberGenerator` |
| **Access Token Expiry** | 60 minutes |
| **Refresh Token Expiry** | 7 days |
| **Token Rotation** | Old tokens revoked on refresh |
| **Active Tokens** | One per user to prevent sharing |

**Authentication Flow:**
```
1. Login → Access Token (60m) + Refresh Token (7d)
2. Access Expires → Use Refresh Token
3. Get New Tokens → Old Refresh Revoked
4. Refresh Expires → Must Login Again
```

### Login Audit Logging

All authentication events are logged to the database:

| Event | Trigger | Data Captured |
|-------|---------|---------------|
| **LoginSuccess** | Successful login/register | User ID, Email, IP, User Agent, Timestamp |
| **LoginFailed** | Invalid credentials | Email (no User ID), IP, User Agent, Timestamp |
| **Refresh** | Token refresh | User ID, Email, IP, User Agent, Timestamp |
| **Logout** | User logout | User ID, Email, IP, User Agent, Timestamp |

**Query Examples:**
```sql
-- Failed login attempts
SELECT * FROM LoginAuditLogs WHERE Action = 'LoginFailed';

-- User activity
SELECT * FROM LoginAuditLogs WHERE Email = 'user@example.com';

-- Suspicious IPs
SELECT IpAddress, COUNT(*) as Attempts 
FROM LoginAuditLogs 
WHERE Action = 'LoginFailed' 
GROUP BY IpAddress 
HAVING COUNT(*) > 5;
```

### Rate Limiting

Protection against brute-force attacks:

- **Algorithm**: Fixed window
- **Limit**: 5 requests per minute per IP
- **Scope**: Login and refresh endpoints only
- **Response**: HTTP 429 Too Many Requests

**Why only auth endpoints?**
Other endpoints require valid JWT, making them already secure.

## 🏗️ Architecture

### Project Structure

```
ApiAuth/
├── 📁 Domain/                          # Core entities
│   ├── ApplicationUser.cs              # Custom user entity
│   ├── RefreshToken.cs                 # Refresh token entity
│   └── LoginAuditLog.cs                # Audit log entity
│
├── 📁 Application/                     # Business logic
│   ├── DTOs/                           # Data transfer objects
│   │   ├── RegisterRequest.cs
│   │   ├── LoginRequest.cs
│   │   ├── AuthResponse.cs
│   │   ├── AuthResponseWithRefreshToken.cs
│   │   ├── RefreshTokenRequest.cs
│   │   └── UserResponse.cs
│   ├── Interfaces/                     # Service contracts
│   │   ├── IAuthService.cs
│   │   ├── IJwtTokenGenerator.cs
│   │   ├── IRefreshTokenService.cs
│   │   └── IAuditLogService.cs
│   └── Services/                       # Service implementations
│       ├── AuthService.cs
│       ├── RefreshTokenService.cs
│       └── AuditLogService.cs
│
├── 📁 Infrastructure/                  # External concerns
│   ├── Persistence/
│   │   └── ApplicationDbContext.cs     # EF Core context
│   ├── Security/
│   │   └── JwtTokenGenerator.cs        # JWT generation
│   └── Seed/
│       └── DatabaseSeeder.cs           # Admin user seeding
│
└── 📁 API/                             # Presentation layer
    ├── Controllers/
    │   └── AuthController.cs           # API endpoints
    ├── Middleware/
    │   └── ExceptionMiddleware.cs      # Error handling
    └── Helpers/
        └── HttpContextHelper.cs        # HTTP utilities
```

### Design Principles

✅ **SOLID Principles**
- Single Responsibility - Each class has one purpose
- Open/Closed - Extensible through interfaces
- Liskov Substitution - Proper inheritance
- Interface Segregation - Focused contracts
- Dependency Inversion - Depends on abstractions

✅ **Clean Architecture**
- Domain layer has no dependencies
- Application layer depends only on Domain
- Infrastructure implements Application interfaces
- API layer orchestrates everything

✅ **Best Practices**
- Async/await throughout
- Dependency injection
- DTOs for data transfer
- Repository pattern (via EF Core)
- Global exception handling

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=auth.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKey...",  // Change in production!
    "Issuer": "ApiAuthServer",
    "Audience": "ApiAuthClient",
    "ExpiryMinutes": "60"
  }
}
```

### Environment Variables (Production)

```bash
# JWT Configuration
JWT__SECRETKEY=<strong-random-key>
JWT__ISSUER=<your-issuer>
JWT__AUDIENCE=<your-audience>

# Database
CONNECTIONSTRINGS__DEFAULTCONNECTION=<connection-string>
```

### Password Requirements

- Minimum 6 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- Non-alphanumeric optional (configurable)

## 🧪 Testing

### Using Swagger UI

1. **Start the application** - Navigate to `https://localhost:7xxx/swagger`

2. **Register a user** - Use `/api/auth/register` endpoint

3. **Get tokens** - Copy `token` and `refreshToken` from response

4. **Authorize** - Click 🔓 **Authorize** button, enter: `Bearer {token}`

5. **Test endpoints** - Try protected endpoints like `/api/auth/me`

6. **Test refresh** - Use `/api/auth/refresh` with saved refresh token

7. **Test admin** - Login as admin, test `/api/auth/admin/users`

### Using Postman

Import the provided collection: `ApiAuth.postman_collection.json`

The collection includes:
- ✅ All endpoints with examples
- ✅ Automatic token variable setting
- ✅ Rate limiting tests

### Using .http File

Use the included `ApiAuth.http` file with VS Code REST Client extension:

```http
### Login
POST https://localhost:7001/api/auth/login
Content-Type: application/json

{
  "email": "admin@apiauth.com",
  "password": "Admin@123"
}
```

### Manual Testing Scenarios

**Test Refresh Token Flow:**
```
1. Login → Save refresh token
2. Use refresh token → Get new tokens
3. Try old refresh token → Should fail (401)
```

**Test Rate Limiting:**
```
1. Make 5 login requests → All processed
2. Make 6th request → Should return 429
3. Wait 1 minute → Should work again
```

**Test Logout:**
```
1. Login → Get tokens
2. Logout → Tokens revoked
3. Try refresh → Should fail (401)
```

## 🗄️ Database

### Tables

| Table | Description |
|-------|-------------|
| `AspNetUsers` | User accounts |
| `AspNetRoles` | Roles (Admin, User) |
| `AspNetUserRoles` | User-role mappings |
| `RefreshTokens` | Active refresh tokens |
| `LoginAuditLogs` | Authentication events |

### Viewing the Database

Use [DB Browser for SQLite](https://sqlitebrowser.org/):

1. Open `auth.db` file
2. Browse tables and data
3. Execute SQL queries
4. Export data if needed

### Migrations

Migrations are **automatically applied** on startup.

For manual migration management:

```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Create new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 🚢 Production Deployment

### Pre-Deployment Checklist

- [ ] Change JWT secret key to strong random value
- [ ] Store secrets in environment variables/Key Vault
- [ ] Enable HTTPS only (disable HTTP)
- [ ] Configure CORS for your frontend
- [ ] Set up proper logging (Serilog, Application Insights)
- [ ] Implement log retention policy
- [ ] Review and adjust rate limits
- [ ] Change default admin password
- [ ] Consider SQL Server/PostgreSQL instead of SQLite
- [ ] Set up health check endpoints
- [ ] Configure monitoring and alerts
- [ ] Implement Redis for distributed rate limiting (if scaling)
- [ ] Add comprehensive tests
- [ ] Set up CI/CD pipeline

### Deployment Options

**Azure App Service:**
```bash
# Publish to Azure
dotnet publish -c Release
az webapp deploy --resource-group <rg> --name <app-name> --src-path ./publish
```

**Docker:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiAuth.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiAuth.dll"]
```

**Linux/Windows Server:**
```bash
# Build and publish
dotnet publish -c Release -o ./publish

# Copy to server and run as service
dotnet ApiAuth.dll
```

### Scaling Considerations

For distributed deployments:
- Use **SQL Server** or **PostgreSQL** (not SQLite)
- Implement **Redis** for distributed rate limiting
- Use **distributed caching** for refresh tokens
- Set up **load balancer** with sticky sessions
- Implement **async audit logging** with message queue
- Archive old audit logs to **cold storage**

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| Database not created | Check write permissions in project directory |
| JWT token invalid | Verify `Bearer` prefix, check expiry, verify secret key |
| 403 Forbidden on admin endpoint | Ensure user has Admin role, check token claims |
| Refresh token returns 401 | Token expired (7 days) or revoked, check database |
| Rate limit too strict | Adjust `PermitLimit` in Program.cs |
| Audit logs too large | Implement cleanup job, set retention policy |
| Admin has no roles | Delete database and restart (auto-seeding will fix) |

## 📊 HTTP Status Codes

| Code | Meaning | When |
|------|---------|------|
| 200 OK | Success | Successful operation |
| 400 Bad Request | Validation error | Invalid input data |
| 401 Unauthorized | Authentication failed | Invalid/expired token or credentials |
| 403 Forbidden | Insufficient permissions | User lacks required role |
| 404 Not Found | Resource not found | User or resource doesn't exist |
| 429 Too Many Requests | Rate limit exceeded | Too many auth attempts |
| 500 Internal Server Error | Server error | Unexpected server error |

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- ASP.NET Core Team for excellent documentation
- Microsoft Identity for secure authentication
- Entity Framework Core for data access
- Community for best practices and patterns

## 📞 Support

If you have questions or issues:

1. Check this README thoroughly
2. Review the API documentation in Swagger
3. Test using provided Postman collection
4. Check existing GitHub issues
5. Create a new issue with details

---

**Built with ❤️ using .NET 8 and Clean Architecture principles**

**Status**: ✅ Production Ready | 🔒 Secure | 📚 Well Documented
