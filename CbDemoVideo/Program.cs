using Azure.Identity;
using Azure.Storage.Blobs;
using CbDemoVideo.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var configuredConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection configuration is required.");

    var sqlBuilder = new SqlConnectionStringBuilder(configuredConnectionString)
    {
        Authentication = SqlAuthenticationMethod.ActiveDirectoryManagedIdentity,
        UserID = string.Empty,
        Password = string.Empty
    };

    options.UseSqlServer(sqlBuilder.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var serviceUri = configuration["BlobImage:ServiceUri"]
        ?? throw new InvalidOperationException("BlobImage:ServiceUri configuration is required.");

    // Managed Identity in Azure; falls back to developer credentials (VS / az login) locally.
    return new BlobServiceClient(new Uri(serviceUri), new DefaultAzureCredential());
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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
