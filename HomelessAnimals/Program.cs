using Serilog.Formatting.Compact;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using HomelessAnimals.Policies;
using HomelessAnimals.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using HomelessAnimals.Mapping;
using System.Threading.RateLimiting;
using HomelessAnimals.Shared.Models;
using HomelessAnimals.Extensions;
using HomelessAnimals.BusinessLogic.EmailMessageBuilders;
using HomelessAnimals.BusinessLogic.EmailSender;
using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.BusinessLogic.Services;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.Repositories;
using FluentValidation;
using HomelessAnimals.Validators;
using Microsoft.EntityFrameworkCore;
using HomelessAnimals.DataAccess;
using Microsoft.AspNetCore.HttpOverrides;
using HomelessAnimals.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

    builder.Host.UseSerilog();
}

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = builder.Environment.EnvironmentName switch
        {
            "Development" => SameSiteMode.Lax,
            "Staging" => SameSiteMode.None,
            "Production" => SameSiteMode.Strict,
            _ => SameSiteMode.Strict
        };

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.EventsType = typeof(CookieValidationPolicy);
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        };
    });

// Add services to the container.
builder.Services.AddScoped<CookieValidationPolicy>();

builder.Services.AddAuthorization(Policy.AddPolicies);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));

    options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));

    options.Filters.Add(new SwaggerResponseAttribute(StatusCodes.Status500InternalServerError,
        type: typeof(ErrorResponse)));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddAutoMapper(typeof(VolounteerProfileMapping));

if (!builder.Environment.IsProduction())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:3000",
                    "https://hs.azurewebsites.net")
                    .AllowAnyMethod();
        });
    });
}

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.AddPolicy("HiddenSensitiveValuesPolicy", context =>
    {
        var ipAddress = context.GetIPAddress().ToString();

        var settings = builder.Configuration
            .GetSection(nameof(RateLimiterSettings))
            .Get<RateLimiterSettings>();

        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, options =>
            new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(settings.Window),
                PermitLimit = settings.PermitLimit,
                QueueLimit = settings.QueueLimit
            });
    });
});

builder.Services.AddScoped<IVolunteerService, VolunteerService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddTransient<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddTransient<IEmailSender, GmailEmailSender>();
builder.Services.AddTransient<IEmailTemplateBuilder, EmailTemplateBuilder>();
builder.Services.AddScoped<ISignUpRequestService, SignUpRequestService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IScopeVerificationService, ScopeVerificationService>();

builder.Services.AddScoped<IDataAccessFactory, DataAccessFactory>();

ValidatorOptions.Global.LanguageManager.Enabled = false;
builder.Services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddTransient<IValidator<SetPasswordRequest>, SetPasswordRequestValidator>();
builder.Services.AddTransient<IValidator<SubmitSignUpRequest>, SubmitSignUpRequestValidator>();
builder.Services.AddTransient<IValidator<AdminVolunteerEdit>, VolunteerEditValidator>();
builder.Services.AddTransient<IValidator<Animal>, CreateAnimalValidator>();
builder.Services.AddTransient<IValidator<UpdateSignUpRequestStatus>, UpdateSignUpRequestStatusValidator>();

builder.Services.AddDbContext<HomelessAnimalsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HomelessAnimalsContext")));

builder.Services.AddMemoryCache();

builder.Services.Configure<GmailSettings>(options =>
{
    builder.Configuration.GetSection(nameof(GmailSettings)).Bind(options);
});

builder.Services.Configure<SetPasswordSettings>(options =>
{
    builder.Configuration.GetSection(nameof(SetPasswordSettings)).Bind(options);
});

builder.Services.Configure<EmailTemplatesSettings>(options =>
{
    builder.Configuration.GetSection(nameof(EmailTemplatesSettings)).Bind(options);
});

builder.Services.Configure<ReCaptchaSettings>(options =>
{
    builder.Configuration.GetSection(nameof(ReCaptchaSettings)).Bind(options);
});

builder.Services.AddHttpClient<ICaptchaValidationService, CaptchaValidationService>(options =>
{
    options.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsProduction())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor
    });
}

// Configure the HTTP request pipeline.
if (!builder.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

    app.UseRouting();
    app.UseCors();
}

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMappingMiddleware>();

app.Run();
