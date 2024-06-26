using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tasket.Client;
using Tasket.Client.Services;
using Tasket.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// HTTP Client
builder.Services.AddScoped(ps => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// InterFacee
builder.Services.AddScoped<IProjectDTOService, WASMProjectDTOService>();
builder.Services.AddScoped<ITicketDTOService, WASMTicketDTOService>();
builder.Services.AddScoped<ICompanyDTOService, WASMCompanyDTOService>();

await builder.Build().RunAsync();
