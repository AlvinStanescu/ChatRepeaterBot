using ChatHistoryParser.Interfaces;
using FacebookDataModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChatHistoryParser
{
    public class FacebookConversationAggregatesWriter
    {
        public void WriteConversations(string sender, IList<FacebookConversation> conversations, string outputDirectory, IFileUtility fileUtility)
        {
            // Clean up the conversations which seem to be with a single person to include the sender.
            foreach (var conversation in conversations)
            {
                if (conversation.Participants.Count < 2 && conversation.Participants.First() != sender)
                {
                    conversation.Participants.Add(sender);
                }
            }

            foreach (var conversationGroup in conversations.GroupBy(g => g.Participants.OrderBy(o => o).Aggregate((a, b) => a.Trim() + b.Trim())))
            {
                var conversationList = conversationGroup.ToList();
                var serializedConversation = JsonConvert.SerializeObject(conversationList);
                var filePath = Path.Combine(outputDirectory, (conversationList.First().Participants.Take(5).Aggregate((a, b) => a + "-" + b)).Replace("/", "-").Replace(":", "-").Replace("facebook.com", string.Empty));

                if (fileUtility.Exists(filePath))
                {
                    fileUtility.Delete(filePath);
                }

                fileUtility.WriteAllText(filePath, serializedConversation);
            }
        }
    }
}
