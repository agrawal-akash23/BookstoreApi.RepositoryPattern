using BookstoreApi.RepositoryPattern.Data;
using BookstoreApi.RepositoryPattern.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        // Handle circular references - stops at the first loop
        options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register BookstoreDbContext with the DI container
builder.Services.AddDbContext<BookstoreDbContext>(options =>
options.UseSqlite("Data Source=bookstore.db")
.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
.EnableSensitiveDataLogging()); // shows actual parameter values

// To use SQL Server instead, replace with:
// options.UseSqlServer(builder.Configuration.ConnectionString("DefaultConnection"))
// Then add to appsettings.json: "ConnectionStrings": { "DefaultConnection": "Server=.....;...."}

// -- NEW: Register repositories -------------------------------------
// "When something asks for IBookRepository, give them an EfBookRepository"
// Scoped =  one instance per HTTP request (matches DbContext lifetime)
builder.Services.AddScoped<IBookRepository, EfBookRepository>();
builder.Services.AddScoped<IAuthorRepository, EfAuthorRepository>();
// -------------------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseAuthorization();
app.MapControllers();
app.Run();

