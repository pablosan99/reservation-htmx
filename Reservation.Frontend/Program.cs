using Lib.AspNetCore.ServerSentEvents;
using Reservation.Backend;
using Reservation.Frontend.Background;
using Reservation.Frontend.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ApiClientOptions>(builder.Configuration.GetSection("ApiClient"));
builder.Services.ReservationModule();
builder.Services.AddControllersWithViews().AddRazorOptions(options =>
{
    options.ViewLocationFormats.Add("/{0}.cshtml");
});
builder.Services.AddServerSentEvents();
builder.Services.AddHostedService<ServerSentEventsWorker>();
builder.Services.AddScoped<DataFormProvider>();

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
app.MapServerSentEvents("/rn-updates");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ReservationForm}/{action=Index}/{id?}");
app.Run();