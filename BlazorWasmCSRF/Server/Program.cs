using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//CSRF
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

IAntiforgery? antiforgery = app.Services.GetRequiredService<IAntiforgery>();

app.Use(next => context =>
{
    if (string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
    {
        // The request token can be sent as a JavaScript-readable cookie
        var tokens = antiforgery.GetAndStoreTokens(context);
        context.Response.Cookies.Append("CSRF-TOKEN", tokens.RequestToken);
    }

    return next(context);
});


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
