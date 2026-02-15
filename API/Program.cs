using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using API.Servieces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
  opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions =>
    {
      sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null);
    });
});
builder.Services.AddCors();
builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddScoped<PaymentsService>();
builder.Services.AddIdentityApiEndpoints<User>(opt => {
  opt.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<StoreContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

//app.UseDeveloperExceptionPage (is used)
app.UseCors(opt =>
{
  opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:3000");
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapGroup("api").MapIdentityApi<User>(); // api/login
app.MapFallbackToController("Index","Fallback");

try
{
  await DbInitializer.InitDb(app);
}
catch (Exception ex)
{
  Console.WriteLine("Database initialization failed:");
  Console.WriteLine(ex);
}

app.Run();
