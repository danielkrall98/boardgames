using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace TicTacToeServer
{
    public class TicTacToeHub : Hub
    {
        private static Dictionary<string, (string Player1, string? Player2)> Games = new();

        // Asynchronous Lock to avoid Conflicts (one Thread at a Time)
        private static readonly SemaphoreSlim _lock = new(1, 1);

        public async Task JoinGame()
        {
            await _lock.WaitAsync();
            try
            {
                foreach (var g in Games)
                {
                    if (g.Value.Player2 == null)
                    {
                        string gameId = g.Key;
                        string player1Id = g.Value.Player1;
                        Games[gameId] = (player1Id, Context.ConnectionId);

                        await Clients.Client(player1Id).SendAsync("OpponentJoined", Context.ConnectionId);
                        await Clients.Caller.SendAsync("GameJoined", gameId);
                        await Clients.Caller.SendAsync("AssignSymbol", "O", Context.ConnectionId);
                        await Clients.Client(player1Id).SendAsync("AssignSymbol", "X", player1Id);

                        return;
                    }
                }

                // No Game available -> Create new Game
                string newGameId = Guid.NewGuid().ToString();
                Games[newGameId] = (Context.ConnectionId, null);
                await Clients.Caller.SendAsync("GameCreated", newGameId);
                await Clients.Caller.SendAsync("AssignSymbol", "X", Context.ConnectionId);
            }
            finally
            {
                // Release Lock
                _lock.Release();
            }
        }

        public async Task SendMove(string gameId, int row, int column, string playerId)
        {
            if (Games.TryGetValue(gameId, out var players) && players.Player2 != null)
            {
                // Notify both players of the move
                await Clients.Client(players.Player1).SendAsync("ReceiveMove", row, column, playerId);
                await Clients.Client(players.Player2).SendAsync("ReceiveMove", row, column, playerId);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TicTacToeHub>("/ticTacToeHub");
            });
        }
    }
}