# Services

The service layer keeps controllers simple by moving mission-related business logic into one place.

## Mission service responsibilities

- Read mission data with async EF Core queries.
- Create, update, and delete missions through `ApplicationDbContext`.
- Support filtering by mission status and threat level.
- Provide a clean interface (`IMissionService`) that controllers can depend on.

This project intentionally uses **MVC + EF Core + service layer** only (no repository pattern) so students can focus on common ASP.NET Core architecture first.
