using bai1.Models;
using bai1.Data;
using bai1.Services;
using bai1.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MvcMovieContext")
        ?? throw new InvalidOperationException("Connection String 'MvcMovieContext' not found")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add VNPay configuration
builder.Services.Configure<VnPayConfig>(builder.Configuration.GetSection("VnPay"));
builder.Services.AddScoped<IVnPayService, VnPayService>();

// Add Inventory Service
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Add Order Service
builder.Services.AddScoped<IOrderService, OrderService>();

// Add Report Service
builder.Services.AddScoped<IReportService, ReportService>();

// Add User Service
builder.Services.AddScoped<UserService>();

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Authentication & Authorization
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/LoginPartial"; // Redirect khi ch?a ??ng nh?p
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect khi không có quy?n
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization(options =>
{
    // Policy cho Admin
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    
    // Policy cho Staff (Admin ho?c Staff)
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Admin", "Staff"));
    
    // Policy cho User ?ã ??ng nh?p
    options.AddPolicy("RequireLoggedIn", policy => policy.RequireAuthenticatedUser());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Ki?m tra tr?ng thái tài kho?n sau khi xác th?c
app.UseUserStatusCheck();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
