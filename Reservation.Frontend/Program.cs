using Lib.AspNetCore.ServerSentEvents;
using Reservation.Backend;
using Reservation.Frontend.Background;
using Reservation.Frontend.Pages;
using Reservation.Frontend.Pages.Hubs;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();
try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
    builder.Services.Configure<ApiClientOptions>(builder.Configuration.GetSection("ApiClient"));
    builder.Services.ReservationModule();
    builder.Services.AddControllersWithViews().AddRazorOptions(options => { options.ViewLocationFormats.Add("/{0}.cshtml"); });
    builder.Services.AddServerSentEvents();
    builder.Services.AddHostedService<ServerSentEventsWorker>();
    builder.Services.AddScoped<DataFormProvider>();
    builder.Services.AddTransient<ErrorProvider>();
    builder.Services.AddSignalR();
    builder.Host.UseSerilog();
    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.UseMiddleware<BusinessExceptionMiddleware>();
    app.MapServerSentEvents("/rn-updates");
    app.MapHub<ReservationHub>("/info");
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=TyreChangeReservation}/{action=Index}/{id?}");
    app.MapControllerRoute(
        name: "tyre-change",
        pattern: "{TyreChangeReservation}/{action=Index}/{id?}");
    app.MapControllerRoute(
        name: "air-condition",
        pattern: "{AirConditionReservation}/{action=Index}/{id?}");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}