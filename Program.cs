using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;

using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//read the known proxy
var knownProxies = builder.Configuration.GetSection("KnownProxies").Value;
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = null;
    options.KnownProxies.Clear();
    foreach (var ip in knownProxies.Split(new char[] { ',' }))
    {
        options.KnownProxies.Add(IPAddress.Parse(ip));
    }
});


var app = builder.Build();

// This should be called before any other middleware
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
