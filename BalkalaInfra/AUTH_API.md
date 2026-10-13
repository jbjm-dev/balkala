# Identity API (JWT-free mode)

The authentication, user, role, and permission endpoints are available without bearer tokens. No JWT signing key is required. **Every endpoint is publicly callable**, including endpoints that create or change users, roles, and permissions. Do not expose this configuration to an untrusted network.

## Local configuration

The MySQL `DefaultConnection` is configured in `BalkalaInfra.Api/appsettings.Development.json` for local development. Keep that development file out of public source control. The application does not read or require any `Jwt` settings.

## Routes

### Tenants

- `GET /api/tenants` — list tenants.
- `GET /api/tenants/{tenantId}` — get one tenant.
- `POST /api/tenants` — create a tenant; the API generates its GUID and creation time.

Example request:

```json
{"tenantCode":"BALKALA-NEW-01","name":"Balkala New Company","status":"Active"}
```

Use the returned `tenantCode` when registering a user for that tenant.

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login` — requires only `email` and `password`. If the account has one active tenant, a session is returned directly. If it has multiple active tenants, the response lists them and provides a short-lived `selectionToken`.
- `POST /api/auth/select-tenant` — after a multi-tenant login, send `{ "selectionToken": "<value-from-login>", "tenantId": "<selected-guid>" }` to receive the session for that tenant.
- `POST /api/auth/refresh`
- `POST /api/auth/logout` — body: `{ "userId": "<guid>", "accessToken": "<opaque-token>" }`
- `GET /api/auth/me?tenantId=<guid>&userId=<guid>`
- `POST /api/auth/change-password` — body includes `userId`, `currentPassword`, and `newPassword`.

Register, completed login, tenant selection, and refresh return opaque random session strings in the existing `accessToken` and `refreshToken` fields for API compatibility. They are not JWTs and are not validated by middleware; they do not protect any endpoint. The short-lived tenant `selectionToken` is only used by `/select-tenant` and is not accepted as an API credential.

### Users

All user and user-role routes require the tenant ID as a query parameter because no authenticated tenant claim exists:

- `GET /api/users?tenantId=<guid>`
- `GET /api/users/{id}?tenantId=<guid>`
- `POST /api/users?tenantId=<guid>`
- `PUT /api/users/{id}?tenantId=<guid>`
- `PATCH /api/users/{id}/status?tenantId=<guid>`
- `DELETE /api/users/{id}?tenantId=<guid>`
- `GET /api/users/{id}/roles?tenantId=<guid>`
- `POST /api/users/{id}/roles?tenantId=<guid>`
- `DELETE /api/users/{id}/roles/{roleId}?tenantId=<guid>`

### Roles

- `GET /api/roles?tenantId=<guid>`
- `GET /api/roles/{id}?tenantId=<guid>`
- `POST /api/roles?tenantId=<guid>`
- `PUT /api/roles/{id}?tenantId=<guid>`
- `DELETE /api/roles/{id}?tenantId=<guid>`
- `GET /api/roles/{id}/permissions?tenantId=<guid>`
- `PUT /api/roles/{id}/permissions?tenantId=<guid>`

### Permissions

- `GET /api/permissions`
- `GET /api/permissions/{id}`
- `POST /api/permissions`
- `PUT /api/permissions/{id}`
- `DELETE /api/permissions/{id}`

## Run and Swagger

From the API project directory run `dotnet run`, then open `/swagger` on the reported local address. The existing MySQL schema/migrations remain unchanged; apply the required migrations to the database before using database-backed endpoints.
