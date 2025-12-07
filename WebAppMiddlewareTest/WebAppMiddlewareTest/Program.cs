var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

// ===== MIDDLEWARE 0: Global Exception Handling (en tepede) =====
app.Use(async (context, next) =>
{
    Console.WriteLine("EXC - REQUEST aşaması");
    try
    {
        await next(context); // alttaki tüm pipeline çalışsın
    }
    catch (Exception ex)
    {
        Console.WriteLine($"EXC - HATA YAKALANDI: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Global error middleware: bir hata oluştu.");
    }
    Console.WriteLine("EXC - RESPONSE aşaması");
});

// ===== MIDDLEWARE 1 =====
app.Use(async (context, next) =>
{
    Console.WriteLine("M1 - REQUEST aşaması");
    Console.WriteLine($"  Path: {context.Request.Path}");

    await next(context);  // M2'ye, routing'e, controller'a vs. devam eder

    Console.WriteLine("M1 - RESPONSE aşaması");
});

// ===== MIDDLEWARE 2 =====
app.Use(async (context, next) =>
{
    Console.WriteLine("  M2 - REQUEST aşaması");

    await next(context);  // M3'e geçer

    Console.WriteLine("  M2 - RESPONSE aşaması");
});

// ===== MIDDLEWARE 3 =====
app.Use(async (context, next) =>
{
    Console.WriteLine("    M3 - REQUEST aşaması");

    await next(context);  // Endpoint'e geçer (MapControllers)

    Console.WriteLine("    M3 - RESPONSE aşaması");
});

// ===== ROUTING + ENDPOINTLER =====
app.UseRouting();

// Bu middleware Routing'den sonra, Controller'dan önce çalışır
app.Use(async (context, next) =>
{
    Console.WriteLine("      AFTER ROUTING - REQUEST aşaması");
    Console.WriteLine($"      Seçilen endpoint: {context.GetEndpoint()?.DisplayName}");

    await next(context);  // Controller'a geç

    Console.WriteLine("      AFTER ROUTING - RESPONSE aşaması");
});

// (Buraya istersen UseAuthentication / UseAuthorization da eklenir normalde)
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers(); // Controller action'ları map'lenir

app.Run();

/*
 
       Content root path: C:\Users\Hp35\Desktop\Projects\Dotnet\dotnet-refreshing\WebAppMiddlewareTest\WebAppMiddlewareTest
st
EXC - REQUEST asaması
M1 - REQUEST asaması
  Path: /WeatherForecast
  M2 - REQUEST asaması
    M3 - REQUEST asaması
      AFTER ROUTING - REQUEST asaması
      Seçilen endpoint: WebAppMiddlewareTest.Controllers.WeatherForecastController.Get (WebAppMiddlewareTest)
      AFTER ROUTING - RESPONSE asaması
    M3 - RESPONSE asaması
  M2 - RESPONSE asaması
M1 - RESPONSE asaması
EXC - RESPONSE asaması

 
 */
