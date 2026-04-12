# Integration test data setup

These integration tests run the full ASP.NET Core request pipeline with `WebApplicationFactory` and swap out the app's SQL Server registration for an in-memory SQLite database.

Why this setup is used for integration tests:
- It avoids the live Azure SQL database entirely, so classroom demos are safe and repeatable.
- SQLite in-memory keeps the tests fast while still exercising EF Core with a relational provider.
- The factory resets and seeds known data before each test, so every test starts from the same state and is easy for students to reason about.
