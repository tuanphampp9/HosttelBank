using Btl_web_nc.Models;
using Btl_web_nc.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Sign up repositories
builder.Services.AddScoped<IFavouriteRepositories, FavouriteRepository>();
builder.Services.AddScoped<IUserRepositories, UserRepository>();
builder.Services.AddScoped<IRoleRepositories, RoleRepository>();
builder.Services.AddScoped<ITypeRepositories, TypeRepository>();
builder.Services.AddScoped<IPostRepositories, PostRepository>();
builder.Services.AddScoped<INotifyRepositories, NotifyRepository>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();