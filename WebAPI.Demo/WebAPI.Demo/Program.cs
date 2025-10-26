using Microsoft.AspNetCore.Builder;        // <-- önemli
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

//builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
