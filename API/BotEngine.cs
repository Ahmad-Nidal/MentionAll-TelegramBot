using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace API
{
    public class BotEngine
    {
        private readonly TelegramBotClient _botClient;

        public BotEngine(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task ListenForMessagesAsync()
        {
            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await _botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates
            if (update.Message is not { } message)
            {
                return;
            }

            // Only process text messages
            if (message.Text is not { } messageText)
            {
                return;
            }
            if (message.Chat.Type == ChatType.Private)
            {
                // Handle private messages
                string senderName = "You";

                Console.WriteLine($"Received a '{messageText}' message from {senderName} in private chat.");
            }
            else if (message.Chat.Type == ChatType.Group || message.Chat.Type == ChatType.Supergroup)
            {
                if (message.ReplyToMessage is { Text: string repliedText })
                {
                    // Reply to the replied message
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"You replied with: '{repliedText}'", replyToMessageId: message.ReplyToMessage.MessageId);

                    var me = await _botClient.GetMeAsync();
                    if (messageText.Contains($"@{me.Username}"))
                    //if (true)
                    {
                        await botClient.PinChatMessageAsync(message.Chat.Id, message.ReplyToMessage.MessageId);

                        // Mention all group members
                        List<string> membersList = new()
                        { "@Ahmad_Nidal", "@Ahmad_Nidal2", "@Ahmad_Nidal3", "@Ahmad_Nidal4", "@Ahmad_Nidal5", "@Ahmad_Nidal6", "@Ahmad_Nidal7", "@Ahmad_Nidal8", "@Ahmad_Nidal9", "@Ahmad_Nidal10", "@Ahmad_Nidal11", "@Ahmad_Nidal12", "@Ahmad_Nidal13", "@Ahmad_Nidal14", "@Ahmad_Nidal15", "@Ahmad_Nidal16", "@Ahmad_Nidal17", "@Ahmad_Nidal18", "@Ahmad_Nidal19", "@Ahmad_Nidal20", "@Ahmad_Nidal21" };
                        foreach (var member in membersList)
                        {
                            // wait 1 sec
                            await botClient.SendTextMessageAsync(message.Chat.Id, member, replyToMessageId: message.ReplyToMessage.MessageId);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Received a '{messageText}' message in group {message.Chat.Id} ({message.Chat.Title}).");

                }
            }
        }
        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
