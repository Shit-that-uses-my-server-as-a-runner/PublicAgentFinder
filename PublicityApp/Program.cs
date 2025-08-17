using PublicityApp.Abstractions;
using PublicityApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.
    ClearProviders().
    AddConsole();

//feel free to test with "InMemoryDictionary", but this implementation turns out to be a bit faster
builder.Services.AddSingleton<IInMemoryDb, InMemoryConcurrentDictionary>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
