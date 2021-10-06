using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BotTraining
{
    public class Worker : BackgroundService
    {
        private ILogger<Worker> logger;
        private IConfiguration configuration;
        private DiscordClient discordClient;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting discord bot");

            string discordBotToken = configuration["DiscordBotToken"];
            discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = discordBotToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            discordClient.MessageCreated += OnMessageCreated;
            await discordClient.ConnectAsync();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            discordClient.MessageCreated -= OnMessageCreated;
            await discordClient.DisconnectAsync();
            discordClient.Dispose();
            logger.LogInformation("Discord bot stopped");
        }

        private async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            if (!IsBotTrainingChannel(e)  || IsMessageFromBot(e))
                return;
            if (IsMessageHello(e))
            {
                logger.LogInformation("respond to hello");
                await e.Message.RespondAsync("Hello");
                return;
            }
            if (IsMessageHelp(e))
            {
                logger.LogInformation("help asked");
                await e.Message.RespondAsync("I only respond to 'hello' with 'hello'");
                return;
            }
        }

        private bool IsMessageFromBot(MessageCreateEventArgs e)
        {
            return e.Author.IsBot;
        }


        private bool IsBotTrainingChannel(MessageCreateEventArgs e)
        {
            return e.Channel.Name.Equals("bot-training");
        }

        private bool IsMessageHello(MessageCreateEventArgs e)
        {
            return e.Message.Content.Equals("Hello", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsMessageHelp(MessageCreateEventArgs e)
        {
            return e.Message.Content.Equals("Help", StringComparison.OrdinalIgnoreCase);
        }
    }
}