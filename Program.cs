using TribalWarsCheckAPI.Interfaces;
using TribalWarsCheckAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
      .AllowAnyMethod()
    .AllowAnyHeader();
    });
});

// Register services as Singletons
builder.Services.AddSingleton<IAttackStorage, AttackStorage>();
builder.Services.AddSingleton<IScheduleStorage, ScheduleStorage>();
builder.Services.AddSingleton<IIDImporter, IDImporter>();
builder.Services.AddSingleton<ICommandGenerator, CommandGenerator>();
builder.Services.AddSingleton<IImporterFromSqlFile, ImporterFromSqlFile>();
builder.Services.AddSingleton<IActionChecker, ActionChecker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();

Console.WriteLine("TribalWars Check API is running...");
app.Run();
