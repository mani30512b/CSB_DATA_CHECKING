using CSB_DATA_CHECKING.Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// ✅ Set EPPlus License (required)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// ✅ Register services
builder.Services.AddControllers();
builder.Services.AddScoped<ICsbValidatorService, CsbValidatorService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Define CORS policy
var MyAllowSpecificOrigins = "MyAllowSpecificOrigins"; // ⚠️ Must match below exactly!

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // React dev server origin
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// ✅ Enable middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

// ✅ CORS middleware MUST come before controllers
app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseAuthorization();

// ✅ Map controllers
app.MapControllers();

// ✅ Start app
app.Run();
