using System.Linq;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nimbus.Interfaces;
using Nimbus.Services;
using Nimbus.Services.Encryption;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);


// Add services to the container.
DotNetEnv.Env.TraversePath().Load();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ITDLibRepository, TDLibRepository>(provider =>
{
    int? API_ID = int.TryParse(Environment.GetEnvironmentVariable("API_ID"), out var apiId) ? apiId : (int?)null;
    string? API_HASH = Environment.GetEnvironmentVariable("API_HASH");
    string? PHONE_NUMBER = Environment.GetEnvironmentVariable("PHONE_NUMBER");
    return new TDLibRepository(API_ID, API_HASH, PHONE_NUMBER);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllers();
await app.StartAsync();

// Open the Electron-Window here
await Electron.WindowManager.CreateWindowAsync();


//handle open folder dialog box using system's file explorer
await Electron.IpcMain.On("open-folder", async (args) =>
{
    var window = Electron.WindowManager.BrowserWindows.First();
    var folderPaths = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions
    {
        Title = "Select a folder",
        Properties = new OpenDialogProperty[] { OpenDialogProperty.openDirectory },
    });

    if (folderPaths != null && folderPaths.Length > 0)
    {
        var selectedFolderPath = folderPaths[0];
        Electron.IpcMain.Send(window, "folder-selected", selectedFolderPath);
    }
});

app.WaitForShutdown();
app.Run();
