using Customer.API.Services;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Register the CustomerRepository as a singleton since it's our in-memory database
builder.Services.AddSingleton<CustomerRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API V1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();
app.Run();
