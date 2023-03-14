using DevEncurtaURL.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=> {
    c.SwaggerDoc("v1", new OpenApiInfo{
        Title= "DevEncurtaUrl.API",
        Version = "v1",
        Contact = new OpenApiContact{
            Name = "Higor Dutra",
            Email = "higor.dutra@hotmail.com",
            Url = new Uri("https://github.com/higordutraa")
        }
    });
    var xmlFile = "DevEncurtaUrl.API.XML";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var connectionString = builder.Configuration.GetConnectionString("DevEncurtaUrl");

builder.Services.AddDbContext<DevEncurtaUrlDbContext>(o => o.UseSqlServer(connectionString));

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    Serilog.Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.MSSqlServer(connectionString,
    sinkOptions: new MSSqlServerSinkOptions()
    {
        AutoCreateSqlTable = true,
        TableName = "Logs"
    })
    .WriteTo.Console()
    .CreateLogger();
}).UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
