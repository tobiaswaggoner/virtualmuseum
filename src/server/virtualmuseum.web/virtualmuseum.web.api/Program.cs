using dotenv.net;
using Microsoft.OpenApi.Models;
using virtualmuseum.web.api.Components;
using virtualmuseum.web.api.Services;
using virtualmuseum.web.api.Services.Configuration;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.Configure<ReleaseServiceConfig>(builder.Configuration.GetSection("ReleaseService"));

builder.Services.AddSingleton<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddTransient<IMediaService, MediaService>();
builder.Services.AddSingleton<IReleaseService, ReleaseService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// add httpclient for release service
builder.Services.AddHttpClient<ReleaseService>();

// Add rest controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

// add swagger
app.UseSwagger();
app.UseSwaggerUI();

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
