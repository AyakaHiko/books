using books.Authorization;
using books.Data;
using books.Models;
using books.Models.DTO;
using books.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(Policies.SuperAdminAccessOnly,
            policy => policy.RequireAssertion(
                context => context.User.HasClaim(claim => claim.Type == Claims.SuperAdmin
                )));
        options.AddPolicy(Policies.AdminAndAboveAccess,
            policy => policy.RequireAssertion(
                context => context.User.HasClaim(claim => claim.Type is Claims.SuperAdmin or Claims.Admin
                )));
        options.AddPolicy(Policies.MemberAndAboveAccess,
            policy => policy.RequireAssertion(
                context => context.User.HasClaim(claim => claim.Type is Claims.SuperAdmin or Claims.Admin or Claims.Member
                )));
    }
    );

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(10);
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<BookContext>(options =>
{
    var cs = builder.Configuration
        .GetConnectionString("DefaultConnection");

    options.UseSqlServer(cs);
});
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(BookProfile), typeof(AuthorProfile), typeof(GenreProfile));
builder.Services.AddTransient<IEmailService, EmailService>();


builder.Services.AddIdentity<User, IdentityRole>((options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;

}))
    .AddEntityFrameworkStores<BookContext>()
    .AddDefaultTokenProviders();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider sp = scope.ServiceProvider;

    await SeedData.InitializeAsync(
        sp,
        app.Environment,
        app.Configuration);
}


app.Run();
