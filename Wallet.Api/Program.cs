using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Serilog.Formatting.Compact;
using System.Text;
using Wallet.Api.Repositories;
using Wallet.Models;
using Wallet.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "./logs/transactions-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{Properties:j} {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.Configure<WalletDBSettings>(
    builder.Configuration.GetSection("WalletDB"));
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BankAccountService>();
builder.Services.AddScoped<TransactionsService>();
builder.Services.AddScoped<KafkaSendFundsService>();
builder.Services.AddHostedService<TransactionProcessorService>();


//builder.Services.AddSingleton<Serilog.ILogger>(log);

builder.Services.AddSingleton<IMongoClient>(services =>
{
    var walletDbSettings = services.GetRequiredService<IOptions<WalletDBSettings>>();
    return new MongoClient(walletDbSettings.Value.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(services => {
    var walletDbSettings = services.GetRequiredService<IOptions<WalletDBSettings>>();
    var client = services.GetRequiredService<IMongoClient>();
    return client.GetDatabase(walletDbSettings.Value.DatabaseName);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
///644008659e40371bf3268ff3
//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiJlZGQyYjliOC0xNDljLTQ2NWEtOWRjOS1mMmViYjQ2ZDNlYTQiLCJpYXQiOiI0LzI0LzIwMjMgMTo0ODozNSBQTSIsIlVzZXJJZCI6IjY0MTg5MTMxMjA1NGM1NmI4NmM3NjI0YSIsIkZpcnN0TmFtZSI6IkpvaG4iLCJFbWFpbCI6ImFkbWluQHdhbGxldC5sdCIsImV4cCI6MTY4MjM0NDcxNSwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9._dzAuLJs_-9wHzeeTYxgTmo75U3aegwMtpB8BJsHD6g
var app = builder.Build();
app.UseSerilogRequestLogging();
//{
//    "type": "transfer",
//  "state": "running",
//  "txnId": "TX0006",
//  "txnTime": "2023-04-24T14:44:40.416Z",
//  "from": "LT00 1111 2222 3333 4444",
//  "to": "LT99 1111 2222 3333 4444",
//  "amount": 99
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }