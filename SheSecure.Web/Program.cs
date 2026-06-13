using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("AuthService", c =>
    c.BaseAddress = new Uri("https://localhost:7271/"));
builder.Services.AddHttpClient("SafetyService", c =>
    c.BaseAddress = new Uri("https://localhost:7044/"));
builder.Services.AddHttpClient("ComplaintService", c =>
    c.BaseAddress = new Uri("https://localhost:7032/"));
builder.Services.AddHttpClient("NotificationService", c =>
    c.BaseAddress = new Uri("https://localhost:7179/"));
builder.Services.AddHttpClient("DashboardService", c =>
    c.BaseAddress = new Uri("https://localhost:7077/"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();