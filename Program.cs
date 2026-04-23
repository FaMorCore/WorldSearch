using WorldSearch.Hubs;
using WorldSearch.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSingleton<GameService>();
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 MB (for photos)
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();

app.MapHub<GameHub>("/gamehub");

app.Run();
