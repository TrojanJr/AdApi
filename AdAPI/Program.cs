// cd C:\Users\јнтон ¬ладимирович\source\repos\AdAPI\AdAPI
// curl -X POST -F "file=@sample.txt" http://localhost:5000/api/platforms/load
// http://localhost:5000/api/platforms/search?location=/ru/svrd/revda

using AdAPI.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<Storage>();

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>{context.Response.StatusCode = 500;await context.Response.WriteAsync("¬нутренн€€ ошибка сервера");});
});
app.Run();

var path = Path.Combine(AppContext.BaseDirectory, "sample.txt");
Console.WriteLine($"ѕуть к файлу: {path}");
Console.WriteLine($"—уществует?: {File.Exists(path)}");