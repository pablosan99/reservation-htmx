using System.Security.Cryptography;
using Lib.AspNetCore.ServerSentEvents;

namespace Reservation.Frontend.Background;

public class ServerSentEventsWorker: BackgroundService
{
    private readonly IServerSentEventsService client;

    public ServerSentEventsWorker(IServerSentEventsService client)
    {
        this.client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var clients = client.GetClients();
                if (clients.Any())
                {
                    Number.Value = RandomNumberGenerator.GetInt32(1, 100);
                    await client.SendEventAsync(
                        new ServerSentEvent
                        {
                            Id = "number",
                            Type = "number",
                            Data = new List<string>
                            {
                                Number.Value.ToString()
                            }
                        },
                        stoppingToken
                    );
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        catch (TaskCanceledException ex)
        {
            //TODO handle exception
            
        }
    }
    
    public static class Number
    {
        public static int Value { get; set; } = 1;
    }
}