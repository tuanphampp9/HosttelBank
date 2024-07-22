using Btl_web_nc.Models;
using Btl_web_nc.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//sign up IFavouriteRepositories and favouriteRepository
builder.Services.AddScoped<IFavouriteRepositories, favouriteRepository>();

//sign up IUserRepositories and userRepository
builder.Services.AddScoped<IUserRepositories, userRepository>();

//sign up IRoleRepositories and roleRepository
builder.Services.AddScoped<IRoleRepositories, roleRepository>();

//sign up ITypeRepositories and typeRepository
builder.Services.AddScoped<ITypeRepositories, typeRepository>();

//sign up IPostRepositories and postRepository
builder.Services.AddScoped<IPostRepositories, postRepository>();

//sign up INotifyRepositories and notifyRepository
builder.Services.AddScoped<INotifyRepositories, notifyRepository>();

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
