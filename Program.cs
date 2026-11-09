using RaceDay.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.HttpOnly = true;
    options.IsEssential = true;
});

builder.Services.AddHttpClient<RaceDayClientService>(client =>
{
    var baseUrl = builder.Configuration["BackendApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl ?? throw new InvalidOperationException("API Base URI target config missing."));
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
