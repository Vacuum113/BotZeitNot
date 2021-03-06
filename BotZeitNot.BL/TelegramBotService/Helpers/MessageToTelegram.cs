﻿using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.Helpers
{
    public static class MessageToTelegram
    {
        public static async Task SendCallBackMessageTelegram(CallbackQuery callbackQuery, string message, TelegramBotClient client)
        {
            await client.AnswerCallbackQueryAsync(callbackQuery.Id);
            await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message);
            await client.DeleteMessageAsync
                (
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId
                );
        }
    }
}