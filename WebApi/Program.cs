﻿using Microsoft.EntityFrameworkCore;

using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Services;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    // use sql server db in production and sqlite db in development
    if (env.IsProduction())
        services.AddDbContext<DataContext>();
    else
    {
        services.AddDbContext<DataContext, SqliteDataContext>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();


    }


    services.AddCors();
    services.AddControllers().AddJsonOptions(
            options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve
        );
    // configure automapper with all automapper profiles from this assembly
    services.AddAutoMapper(typeof(Program));

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.AddHttpContextAccessor();
    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
     services.AddScoped<ICashflowService, CashflowService>();
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

app.Run("http://localhost:4000");