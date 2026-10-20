# LibriApi

REST API in .NET per gestire una lista di libri: CRUD, validazione, EF Core + SQLite, autenticazione JWT, test unitari e di integrazione.

## Avvio

```bash
cd LibriApi
dotnet user-secrets set "Jwt:Key" "<chiave di almeno 32 caratteri>"
dotnet ef database update
dotnet run
```

Swagger UI: http://localhost:5026/swagger

## Test

```bash
dotnet test
```
