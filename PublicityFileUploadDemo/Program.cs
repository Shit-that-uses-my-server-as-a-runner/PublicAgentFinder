using PublicityFileUploadDemo.Abstractions;
using PublicityFileUploadDemo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.
    ClearProviders().
    AddConsole();

builder.Services.AddSingleton<IInMemoryDb, InMemoryDictionary>();

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
