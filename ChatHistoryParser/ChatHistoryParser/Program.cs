using ChatHistoryParser.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatHistoryParser
{
    public class Program
    {
        private const string UserRegexPattern = @"<h1>(.*)?</h1>";
        private const string RegexPattern = @"<div\s*class=""message""><div\s*class=""message_header""><span\s*class=""user"">(.*?)<\/span><span\s* class=""meta"">(.*?)<\/span><\/div><\/div><p>(.*?)<\/p>";

        private static Regex FacebookUserPattern = new Regex(UserRegexPattern, RegexOptions.Compiled);
        private static Regex FacebookMessagePattern = new Regex(RegexPattern, RegexOptions.Compiled);

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please pass the full path to the messages file.");
            }

            InitializeJsonConverter();
            var regexFileParser = new RegexFileParser();

            var sender = regexFileParser.ParseFileByRegex(
                args[0],
                FacebookUserPattern,
                match => match.Groups[1].Value,
                true).First();

            var messages = regexFileParser.ParseFileByRegex(
                args[0],
                FacebookMessagePattern,
                match =>
                    new FacebookMessageBuilder()
                    .WithSender(match.Groups[1].Value)
                    .WithSentDate(match.Groups[2].Value)
                    .WithMessage(match.Groups[3].Value)
                    .Build());

            var conversations = new FacebookConversationsSplitter().GetConversations(messages);

            if (args.Length > 1)
            {
                var outputDirectory = args[1];

                Console.WriteLine($"Conversations between the same participants will be saved together as JSON files to {outputDirectory}");
                new FacebookConversationAggregatesWriter().WriteConversations(sender, conversations, outputDirectory, new FileUtility());
            }

            new FunFactsLogger().ShowFunFacts(sender, messages, conversations);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void InitializeJsonConverter()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        private static int GetEndIndexOfLastMatch(IList<int> matchEndPositions)
        {
            int max = -1;

            foreach (var position in matchEndPositions)
            {
                if (position > max)
                {
                    max = position;
                }
            }

            return max;
        }
    }
}