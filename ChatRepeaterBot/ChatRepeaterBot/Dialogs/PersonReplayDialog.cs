using FacebookDataModel;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Bot.Connector;

namespace AnnoyingMomBot.Dialogs
{
    [Serializable]
    public class PersonReplayDialog : IDialog<object>
    {
        private const string conversationDirectoryPath = "C:\\Users\\Alvin\\Desktop\\Conversations";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var sender = "Alvin Stanescu"; //context.Activity.From.Name;
            var conversationFiles = Directory.EnumerateFiles(conversationDirectoryPath, "*" + sender + "*");
            var data = conversationFiles.Select(c => JsonConvert.DeserializeObject<List<FacebookConversation>>(File.ReadAllText(c))).ToList();
            var text = context.Activity.AsMessageActivity().Text;
            text = text.Replace("?", string.Empty).Replace("!", string.Empty).ToLowerInvariant();
            
            var validConversations = data.SelectMany(c => c.Where(c1 => c1.Messages.Any(m => m.Sender == sender && m.Message.ToLowerInvariant().Contains(text)))).ToList();

            if (validConversations.Count == 0)
            {
                return SendFailureMessage(context);
            }

            var validReplies = validConversations.Select(c =>
            {
                // Order the messages randomly (inefficient, but good enough for our purpose), and pick the first one
                var messagesWithText = c.Messages.Where(m => m.Message.ToLowerInvariant().Contains(text) && m.Sender == sender).OrderBy(o => Guid.NewGuid()).First();

                // Get a valid message
                var messageIndex = c.Messages.IndexOf(messagesWithText);

                // Find the next message sent by the other person
                while (++messageIndex < c.Messages.Count && c.Messages[messageIndex].Sender == sender)
                {
                }

                if (messageIndex < c.Messages.Count)
                {
                    return c.Messages[messageIndex].Message;
                }

                return string.Empty;
            });

            var randomReply = validReplies.Where(r => r != string.Empty).OrderBy(o => Guid.NewGuid()).FirstOrDefault();

            if (randomReply == null)
            {
                return SendFailureMessage(context);
            }

            var message = context.MakeMessage();
            message.Text = randomReply;
            return context.PostAsync(message);
        }

        private static async Task SendFailureMessage(IDialogContext context)
        {
            var failureMessage = context.MakeMessage();
            failureMessage.Text = "Nu am inteles ce vrei sa zici...";
            await context.PostAsync(failureMessage);
        }
    }
}