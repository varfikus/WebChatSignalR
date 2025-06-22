using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebChatSignalR.Hubs;
using WebChatSignalR.Models;
using WebChatSignalR.Data;
using Microsoft.AspNetCore.Http.Features;
using WebChatSignalR.Services;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; 
});

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ChatDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});

builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var connectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

    var npgsqlBuilder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
    var targetDatabase = npgsqlBuilder.Database;

    npgsqlBuilder.Database = "postgres";
    using var systemConnection = new Npgsql.NpgsqlConnection(npgsqlBuilder.ToString());
    systemConnection.Open();

    bool dbJustCreated = false;

    using (var checkCmd = systemConnection.CreateCommand())
    {
        checkCmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{targetDatabase}'";
        var exists = checkCmd.ExecuteScalar() != null;

        if (!exists)
        {
            using var createDbCmd = systemConnection.CreateCommand();
            createDbCmd.CommandText = $"CREATE DATABASE \"{targetDatabase}\"";
            createDbCmd.ExecuteNonQuery();
            dbJustCreated = true;
        }
    }

    systemConnection.Close();

    if (dbJustCreated)
    {
        var dbContext = services.GetRequiredService<ChatDbContext>();
        var connection = dbContext.Database.GetDbConnection();
        connection.Open();

        var sqlFilePath = Path.Combine(app.Environment.ContentRootPath, "ChatDB.sql");
        if (File.Exists(sqlFilePath))
        {
            var sql = File.ReadAllText(sqlFilePath);

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

        connection.Close();
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<ChatHub>("/chathub");
app.MapHub<GroupHub>("/grouphub");

app.Run();
