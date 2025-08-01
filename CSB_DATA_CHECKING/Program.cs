using CSB_DATA_CHECKING.Services;
using OfficeOpenXml; // ✅ Required for setting EPPlus license

var builder = WebApplication.CreateBuilder(args);

// ✅ Set EPPlus license context (required to use ExcelPackage)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.
builder.Services.AddControllers();

// ✅ Register your service implementation
builder.Services.AddScoped<ICsbValidatorService, CsbValidatorService>();

// Swagger support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in Development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
