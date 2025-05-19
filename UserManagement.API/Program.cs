using MassTransit;
using Microsoft.Extensions.Configuration;
using UserManagement.Core;
using UserManagement.Domain;
using UserManagement.Consumers;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UserManagement.API.Dependancies;
using UserManagement.Infrastructure.IRepository;
using UserManagement.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<UserManagementDbContext>(options =>
     options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("UserManagement.API") // Updated migrations assembly to match the target project
    ));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped(typeof(GenericProducer));

builder.ConfigureMassTransit();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
//RateLimiterOptions rateLimiterOptions = new RateLimiterOptions()
//{
//    GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
//    {
//        return RateLimitPartition.Create<HttpContext, string>(
//            partitionKey: context.Request.Path.ToString(),
//            factory: partition =>
//            {
//                return RateLimitPartition.Create<HttpContext, string>(
//                    partitionKey: partition,
//                    factory: _ => new ConcurrencyLimiter(new ConcurrencyLimiterOptions
//                    {
//                        PermitLimit = 5,
//                        QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
//                        QueueLimit = 0
//                    }));
//            });
//    })
//};
//app.UseRateLimiter(rateLimiterOptions);

app.MapControllers();

app.Run();

