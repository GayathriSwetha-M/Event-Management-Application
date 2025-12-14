using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventManagement.API.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<EventManagement.API.Data.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// Authorization
builder.Services.AddAuthorization();

// JWT Service
builder.Services.AddScoped<EventManagement.API.JWT.IJwtService, EventManagement.API.JWT.JwtService>();

// Repositories
builder.Services.AddScoped<EventManagement.API.Repositories.IUserRepository, EventManagement.API.Repositories.UserRepository>();
builder.Services.AddScoped<EventManagement.API.Repositories.IEventRepository, EventManagement.API.Repositories.EventRepository>();
builder.Services.AddScoped<EventManagement.API.Repositories.IBookingRepository, EventManagement.API.Repositories.BookingRepository>();
builder.Services.AddScoped<EventManagement.API.Repositories.IRefreshTokenRepository, EventManagement.API.Repositories.RefreshTokenRepository>();

// Services
builder.Services.AddScoped<EventManagement.API.Services.IAuthService, EventManagement.API.Services.AuthService>();
builder.Services.AddScoped<EventManagement.API.Services.IEventService, EventManagement.API.Services.EventService>();
builder.Services.AddScoped<EventManagement.API.Services.IBookingService, EventManagement.API.Services.BookingService>();
builder.Services.AddScoped<EventManagement.API.Services.IAdminService, EventManagement.API.Services.AdminService>();

// Validators
builder.Services.AddScoped<EventManagement.API.Validators.SignupRequestValidator>();
builder.Services.AddScoped<EventManagement.API.Validators.LoginRequestValidator>();
builder.Services.AddScoped<EventManagement.API.Validators.CreateEventRequestValidator>();
builder.Services.AddScoped<EventManagement.API.Validators.UpdateEventRequestValidator>();

// Facades
builder.Services.AddScoped<EventManagement.API.Facades.IAuthFacade, EventManagement.API.Facades.AuthFacade>();
builder.Services.AddScoped<EventManagement.API.Facades.IEventFacade, EventManagement.API.Facades.EventFacade>();
builder.Services.AddScoped<EventManagement.API.Facades.IBookingFacade, EventManagement.API.Facades.BookingFacade>();
builder.Services.AddScoped<EventManagement.API.Facades.IAdminFacade, EventManagement.API.Facades.AdminFacade>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// MIDDLEWARE 

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
