using System;
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
        private IIntentGetter intentGetter;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting discord bot");

            string discordBotToken = configuration["DiscordBotToken"];
            string luisAppId = configuration["LuisAppId"];
            string luisPredictionKey = configuration["LuisPredictionKey"];
            string luisPredictionEndpoint = configuration["LuisPredictionEndpoint"];
            discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = discordBotToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            discordClient.MessageCreated += OnMessageCreated;
            await discordClient.ConnectAsync();

            intentGetter = new LuisIntentGetter(luisAppId, luisPredictionKey, luisPredictionEndpoint);
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
            if (!IsBotTrainingChannel(e) || IsMessageFromBot(e))
                return;
            
            var intent = await intentGetter.GetIntent(e.Message.Content);
            
            if (intent.IsHello)
            {
                logger.LogInformation("respond to hello");
                await e.Message.RespondAsync("Hello");
                return;
            }
            
            if (intent.IsHelp)
            {
                logger.LogInformation("help asked");
                await e.Message.RespondAsync("I only respond to 'hello' with 'hello'. See source code here https://github.com/worming004/BotTraining");
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