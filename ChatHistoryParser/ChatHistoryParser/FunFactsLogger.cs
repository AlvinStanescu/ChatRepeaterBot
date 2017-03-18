using FacebookDataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatHistoryParser
{
    public class FunFactsLogger
    {
        private const int charactersPerSecond = 2;
        private const int minutesPerDay = 60 * 24;

        public void ShowFunFacts(string sender, IList<FacebookMessage> messages, IList<FacebookConversation> conversations)
        {
            messages = messages.Where(m => m.Sender.Equals(sender)).ToList();
            var length = messages.Select(m => m.Message).Average(s => s.Length);
            var orderedLengthList = messages.Select(m => m.Message.Length).OrderBy(s => s).ToList();
            var medianLength = orderedLengthList[orderedLengthList.Count / 2];

            Console.WriteLine($"You wrote a total of {messages.Count} instant messages, with an average length of {length.ToString("F2")} characters, and a median length of {medianLength} characters.");

            var hoursSpentTexting = messages.Count * medianLength / charactersPerSecond / 3600;
            var daysSpentTexting = hoursSpentTexting / 24;
            var percentageOfDaySpentTexting = (hoursSpentTexting / 24.0) / 1826.0;  // for 5 years.

            Console.WriteLine($"Given an average speed of 2 characters per second (if you do most of your messaging from your phone), it seems that you spent around {hoursSpentTexting} hours writing (not including reading!) instant messages on Facebook - that's {daysSpentTexting} days.");
            Console.WriteLine($"If you mostly used Facebook Messenger from 2012 on-wards, give-or-take, you've spent around {(percentageOfDaySpentTexting * minutesPerDay).ToString("F2")} minutes a day writing IMs on Facebook");

            Console.WriteLine($"You had a total of {conversations.Count} conversations, and your average conversation duration was around {conversations.Average(c => (c.EndDate.Subtract(c.StartDate).Minutes))} minutes.");

            var conversationsPerParticipant = new Dictionary<string, int>();
            foreach (var conversation in conversations)
            {
                foreach (var participant in conversation.Participants)
                {
                    if (!conversationsPerParticipant.TryGetValue(participant, out int conversationCount))
                    {
                        conversationsPerParticipant[participant] = 0;
                    }

                    conversationsPerParticipant[participant] = ++conversationCount;
                }
            }

            conversationsPerParticipant.Remove(sender);

            // Filter out id-based conversations (we should ideally correlate, but...)
            conversationsPerParticipant = conversationsPerParticipant.Where(c => !c.Key.Contains("facebook.com")).ToDictionary(kvPair => kvPair.Key, kvPair => kvPair.Value);

            var topFiveConversationParticipants = conversationsPerParticipant.OrderByDescending(kvPair => kvPair.Value).Take(5).Select(k => k.Key).ToList();

            Console.WriteLine($"You had the most conversations with: 1. {topFiveConversationParticipants[0]}, 2. {topFiveConversationParticipants[1]}, 3. {topFiveConversationParticipants[2]}, 4. {topFiveConversationParticipants[3]}, 5. {topFiveConversationParticipants[4]}");
        }

        //private string DetermineSender(IList<FacebookMessage> messages)
        //{
        //    var dict = new Dictionary<string, int>();

        //    foreach (var message in messages)
        //    {
        //        if (!dict.TryGetValue(message.Sender, out int sentCount))
        //        {
        //            dict[message.Sender] = 0;
        //        }

        //        dict[message.Sender] = ++sentCount;
        //    }

        //    return dict.OrderByDescending(kvPair => kvPair.Value).First().Key;
        //}
    }
}
