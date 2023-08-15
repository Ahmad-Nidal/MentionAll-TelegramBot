using API;
using Telegram.Bot;

var botClient = new TelegramBotClient("API TOKEN");

var me = await botClient.GetMeAsync();
Console.WriteLine($"Hello, World! I am bot {me.Id} and my name is {me.FirstName}.");

// Create a new bot instance
var metBot = new BotEngine(botClient);

// Listen for messages sent to the bot
await metBot.ListenForMessagesAsync();