using Microsoft.OpenApi.Models;
using virtualmuseum.web.api.Components;
using virtualmuseum.web.api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddTransient<IMediaService, MediaService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
