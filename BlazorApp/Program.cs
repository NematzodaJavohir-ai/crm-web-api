using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp;
using BlazorApp.ApiServices;
using BlazorApp.ApiServices.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5240/")
});

builder.Services.AddScoped<StudentApiService>();

builder.Services.AddScoped<IStudentApiService, StudentApiService>();
builder.Services.AddScoped<IMentorApiService, MentorService>();
builder.Services.AddScoped<IGroupApiService,GroupApiService>();


await builder.Build().RunAsync();