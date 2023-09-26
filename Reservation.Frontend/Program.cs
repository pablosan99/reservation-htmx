using Lib.AspNetCore.ServerSentEvents;
using Reservation.Backend;
using Reservation.Frontend.Background;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ApiClientOptions>(builder.Configuration.GetSection("ApiClient"));
builder.Services.ReservationModule();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddServerSentEvents();
builder.Services.AddHostedService<ServerSentEventsWorker>();

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
app.MapGroup("/api/reservation").WithTags("Reservations")
    .MapGet("/reservation-date/{locationId}", async (int locationId, HttpContext context, ApiClient client) =>
    {
    });
app.UseRouting();
app.UseAuthorization();
app.MapServerSentEvents("/rn-updates");
app.MapRazorPages();

app.Run();