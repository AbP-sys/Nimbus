using System.Linq;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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
    // Perform any logic you need when 'open-folder' is received
    // For example, you might want to open a folder dialog and send the selected path back
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
