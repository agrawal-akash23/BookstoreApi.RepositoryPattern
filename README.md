# BookstoreApi.Repository — Repository Pattern + SOLID

Refactors the Project in BookstoreApi reopo to introduce the Repository pattern,
removing all direct DbContext dependencies from controllers and applying
every SOLID principle with concrete examples in the codebase.

**Stack:** ASP.NET Core 8 · EF Core · SQLite · Repository Pattern · SOLID

## Why this project exists

Project BookstoreApi had controllers calling `BookstoreDbContext` directly.
That meant controllers had two reasons to change (HTTP shape AND data access),
unit testing required a real database, and swapping ORMs meant rewriting controllers.

This project fixes all three problems.

## Architecture

```
HTTP Request
    ↓
BooksController (knows only IBookRepository)
    ↓
IBookRepository (the contract — interface)
    ↓
EfBookRepository (implements the contract using EF Core)
    ↓
BookstoreDbContext → SQLite
```

## SOLID in this codebase

| Principle | Where |
|---|---|
| S — Single Responsibility | Controllers handle HTTP only. Repositories handle data access only. |
| O — Open/Closed | Add CachedBookRepository without touching the controller. |
| L — Liskov Substitution | FakeBookRepository can replace EfBookRepository in tests. |
| I — Interface Segregation | IBookRepository and IAuthorRepository are separate, focused interfaces. |
| D — Dependency Inversion | Controllers depend on IBookRepository (abstraction), not EfBookRepository (concrete). |

## Project structure

```
BookstoreApi.RepositoryPattern/
├── Controllers/
│   ├── BooksController.cs       # HTTP only — no EF Core imports
│   └── AuthorsController.cs
├── Repositories/
│   ├── IBookRepository.cs       # contract — what the repo can do
│   ├── EfBookRepository.cs      # implementation — how it does it
│   ├── IAuthorRepository.cs
│   └── EfAuthorRepository.cs
├── Data/
│   └── BookstoreDbContext.cs    # unchanged from Project 4
├── Models/
│   ├── Book.cs                  # unchanged
│   └── Author.cs                # unchanged
├── Migrations/                  # unchanged
└── Program.cs                   # registers repositories in DI
```

## Running locally

```bash
git clone https://github.com/YOUR_USERNAME/BookstoreApi.RepositoryPattern
cd BookstoreApi.RepositoryPattern
dotnet ef database update
dotnet run
```

## Key decisions

**Why Scoped, not Singleton?** The repository holds a reference to DbContext.
DbContext is Scoped (one per request). A Singleton repository would hold a Scoped
DbContext beyond its intended lifetime — a classic "captive dependency" bug.

**Why bool return from Update/Delete?** "Not found" is not exceptional — returning
`false` is cheaper than throwing and makes the calling code cleaner than try/catch.
