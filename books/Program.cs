using books.Data;
using books.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBookService, BookService>();


builder.Services.AddDbContext<BookContext>(options =>
{
    var cs = builder.Configuration
        .GetConnectionString("DefaultConnection");

    options.UseSqlServer(cs);
});
builder.Services.AddControllersWithViews();

var app = builder.Build();


using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;

    try
    {
        BookContext context = services.GetRequiredService<BookContext>();
        await InitBooks.Initialize(context);
    }
    catch (Exception)
    {
        throw;
    }
}
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
