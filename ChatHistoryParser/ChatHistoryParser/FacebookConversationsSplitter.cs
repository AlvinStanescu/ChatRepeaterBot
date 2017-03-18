using FacebookDataModel;
using System;
using System.Collections.Generic;

namespace ChatHistoryParser
{
    public class FacebookConversationsSplitter
    {
        private const int maxConversationDeadTimeInSeconds = 9000;
        private const int maxConversationDeadTimeForNonParticipantsInSeconds = 1800;

        public IList<FacebookConversation> GetConversations(IList<FacebookMessage> messages)
        {
            var conversations = new List<FacebookConversation>();
            var conversationMessages = new List<FacebookMessage>();
            DateTimeOffset conversationStartDate;
            DateTimeOffset conversationEndDate;

            var conversationParticipants = new HashSet<string>();

            // Conversations in Facebook begin from the end, so the end-date will always be the first date
            foreach (var message in messages)
            {
                // Just add the first message to the conversation
                if (conversationMessages.Count == 0)
                {
                    conversationEndDate = message.SentDate;
                    conversationStartDate = message.SentDate;

                    if (!conversationParticipants.Contains(message.Sender))
                    {
                        conversationParticipants.Add(message.Sender);
                    }

                    conversationMessages.Add(message);
                }
                else
                {
                    // First, let's figure out how many seconds we have between the moment we previously assumed that our conversation started, and the current message.
                    // If this value is positive (since messages are ordered end -> beginning), and within our dead-time hysteresis (which is smaller for non-participants), 
                    // then we should safely be able to assume that the message is part of our conversation
                    var secondsBetweenMessageSentAndAssumedStartDate = conversationStartDate.Subtract(message.SentDate).TotalSeconds;

                    if (secondsBetweenMessageSentAndAssumedStartDate >= 0 && 
                        ((secondsBetweenMessageSentAndAssumedStartDate < maxConversationDeadTimeInSeconds && 
                        conversationParticipants.Contains(message.Sender)) || (secondsBetweenMessageSentAndAssumedStartDate < maxConversationDeadTimeForNonParticipantsInSeconds))) 
                    {
                        conversationStartDate = message.SentDate;
                        if (!conversationParticipants.Contains(message.Sender))
                        {
                            conversationParticipants.Add(message.Sender);
                        }

                        conversationMessages.Add(message);
                    }
                    else
                    {
                        // End the previous conversation by adding it to the conversation list
                        conversations.Add(new FacebookConversationBuilder()
                            .WithConversations(conversationMessages)
                            .WithEndDate(conversationEndDate)
                            .WithStartDate(conversationStartDate)
                            .WithParticipants(conversationParticipants)
                            .Build());

                        // Reset all the variables
                        conversationEndDate = message.SentDate;
                        conversationStartDate = message.SentDate;
                        conversationParticipants = new HashSet<string>() { message.Sender };
                        conversationMessages = new List<FacebookMessage>() { message };
                    }
                }
            }

            if (conversationMessages.Count != 0)
            {
                // End the last conversation by adding it to the conversation list
                conversations.Add(new FacebookConversationBuilder()
                    .WithConversations(conversationMessages)
                    .WithEndDate(conversationEndDate)
                    .WithStartDate(conversationStartDate)
                    .WithParticipants(conversationParticipants)
                    .Build());
            }

            return conversations;
        }
    }
}
