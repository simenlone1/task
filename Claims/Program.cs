using System.Reflection;
using System.Text.Json.Serialization;
using Azure.Identity;
using Claims;
using Claims.Api.Services;
using Claims.Api.Services.Interfaces;
using Claims.Auditing;
using Claims.Core.AutoMapper;
using Claims.Persistence.Auditing.SQL;
using Claims.Persistence.ServiceBus.Interface;
using Claims.Util;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var a  = builder.Configuration.GetSection("CosmosDb:Cover");
builder.Services.AddCosmosRepository<Claim>(builder.Configuration.GetSection("CosmosDb:Claim"));
builder.Services.AddCosmosRepository<Cover>(builder.Configuration.GetSection("CosmosDb:Cover"));

//builder.Services.AddAuditServiceBus(builder.Configuration.GetSection("ServiceBus:Audit"));
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
} );



builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ICoverService, CoverService>();
builder.Services.AddAutoMapper(typeof(ClaimsProfile));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}


app.Run();



public partial class Program { }