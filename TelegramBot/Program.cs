using System;
using System.Net;
using ApiAiSDK;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace telega_bot
{
    class MainClass
    {
        static TelegramBotClient Bot;
        static ApiAi apiAi;
        static void Main()
        {
			WebProxy wb = new WebProxy("109.167.99.199:8080");
			Bot = new TelegramBotClient("637124033:AAHzwcUoLeuIibFxdHPiTyyb_5hmsilT5wI", wb);
			AIConfiguration aI = new AIConfiguration("b89cbdb23207406abb93c4f4afd77f4d", SupportedLanguage.Russian);

            apiAi = new ApiAi(aI);

            var me = Bot.GetMeAsync().Result;
            Console.WriteLine(me.FirstName);

            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private async static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null && message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return;
            var name = message.From.FirstName;
            name += " ";
            name += message.From.LastName;
            Console.WriteLine($"{name} отправил сообщение {message.Text}");
            switch (message.Text)
            {
                case "/start":
                    string text =
@"/start - active bot,
/inline - menu,
/keyboard - keyboard.";
                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;
                case "/inline":
                    var inlinekeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("vk", "https://vk.com/id80546723"),
                            InlineKeyboardButton.WithUrl("YouTube", "https://www.youtube.com/?gl=RU")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("tap1"),
                            InlineKeyboardButton.WithCallbackData("tap2")
                        }

                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Choose:", replyMarkup: inlinekeyboard);
                    break;
                case "/keyboard":
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Location") {RequestLocation = true},
                            new KeyboardButton("Contact") {RequestContact = true}
                        },
                        new[]
                        {
                            new KeyboardButton("Hello"),
                            new KeyboardButton("Sayonara Boy")
                        }
                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Choose:", replyMarkup: keyboard);
                    break;
                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;

                    if (answer == "")
                        answer = "Sorry, I don't understand you)";
                    await Bot.SendTextMessageAsync(message.From.Id, answer);
                    break;
            }

        }

        static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var buttonName = e.CallbackQuery.Data;
            var name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} press on {buttonName}");
            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Hey!");
        }

    }
}
