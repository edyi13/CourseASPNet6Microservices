using Discount.Grpc.Extensions;
using Discount.Grpc.Repositories;
using Discount.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

// Add services to the container.
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.WebHost.ConfigureKestrel(options =>
{   // Setup a HTTP/2 endpoint without TLS.
    options.Listen(IPAddress.Any, 8003, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.Listen(IPAddress.Any, 80, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.Listen(IPAddress.Any, 5284, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

//Creation of the table for the container
var host = WebApplication.CreateBuilder(args).Build();
host.MigrateDatabase<Program>(); 
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
