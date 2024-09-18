using Azure.Storage.Blobs;
//using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebBanVang.Data;
using WebBanVang.Mapping;
using WebBanVang.Models.Domain;
using WebBanVang.Repository;
using WebBanVang.Services;
using WebBanVang.Validation;

var builder = WebApplication.CreateBuilder(args);

// send email
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SmtpResetPassword>(builder.Configuration.GetSection("SmtpResetPassword"));
builder.Services.AddHostedService<OrderCleanupService>();
builder.Services.AddHostedService<DailyOrderCleanupService>();

builder.Configuration.AddJsonFile("appsettings.json");


// Register EmailService with the configuration from SmtpSettings
builder.Services.AddScoped<RevenueService>();
builder.Services.AddScoped<EmailService>();

// cap quyen cho fontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000And15723035249",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "http://157.230.35.249", "http://www.kimhoanngan.shop", "http://kimhoanngan.shop", "https://kimhoanngan.shop")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JewelrySales",
        Version = "v1",

    });


    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "0auth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
//add blob store
builder.Services.AddScoped(_ =>
{
    return new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStore"));
});
builder.Services.AddDbContext<JewelrySalesSystemDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("JewelrySalesSystembeta")));

// add code first
//builder.Services.AddDbContext<JewelrySalesSystemDbContext>(option => option.UseSqlServer(builder.Configuration
//    .GetConnectionString("JewelrySalesSystem"),sqlOptions => sqlOptions.EnableRetryOnFailure()
//    )
//);
// add auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option => option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

    });
// Set token expiration to 3 hours
// add mapper
builder.Services.AddScoped<IProductRepository, SQLProductRepository>();
builder.Services.AddScoped<IAuthenRepository, SQLAuthenRepository>();
builder.Services.AddScoped<IGoldTypeRepository, SQLGoldTypeRepository>();
builder.Services.AddScoped<IWarrantyRepository, SQLWarrantyRepository>();
builder.Services.AddScoped<IPaymentRepository, SQLPaymentRepository>();
builder.Services.AddScoped<ICategoryRepository, SQLCategoryRepository>();
builder.Services.AddScoped<ICustomerRepository, SQLCustomerRepository>();

builder.Services.AddScoped<IDashboardRepository, SQLDashboardRepository>();

builder.Services.AddScoped<IStoneRepository, SQLStoneRepository>();
builder.Services.AddScoped<IRolesRepository, SQLRolesRepository>();
builder.Services.AddScoped<IUsersRepository, SQLUserRepository>();
builder.Services.AddScoped<IOrderRepository, SQLOrderRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
// Change EmailService registration to scoped

builder.Services.AddScoped<SQLOrderRepository>();

builder.Services.AddScoped<IStorageRepository, StorageRepository>
             (provider =>
             {
                 var azureBlobStorageConfiguration = provider.GetRequiredService<IOptions<AzureBlobStorageConfiguration>>().Value;
                 return new StorageRepository(azureBlobStorageConfiguration.ConnectionString);
             });
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["ConnectionStrings:AzureBlobStore:blob"]!, preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["ConnectionStrings:AzureBlobStore:queue"]!, preferMsi: true);
});


////////////////////////////////
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
//Validation
app.UseMiddleware<ModelValidationMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowLocalhost3000And15723035249");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.MapControllers();
app.UseCors();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
