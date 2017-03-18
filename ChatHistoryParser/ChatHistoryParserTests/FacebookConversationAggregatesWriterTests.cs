using ChatHistoryParser;
using ChatHistoryParser.Interfaces;
using FacebookDataModel;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ChatHistoryParserTests
{
   public class FacebookConversationAggregatesWriterTests
    {
        private FacebookConversationAggregatesWriter writer = new FacebookConversationAggregatesWriter();
        private Mock<IFileUtility> fileUtilityMock;
        private IDictionary<string, string> fileEntries = new Dictionary<string, string>();

        public FacebookConversationAggregatesWriterTests()
        {
            this.fileUtilityMock = new Mock<IFileUtility>();
            this.fileUtilityMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(false);
            this.fileUtilityMock
                .Setup(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>(
                (path, contents) =>
                {
                    if (fileEntries.ContainsKey(path))
                    {
                        throw new InvalidOperationException("We should never attempt to overwrite a conversation file.");
                    }

                    fileEntries[path] = contents;
                });
        }

        [Fact]
        public void MultipleConversationsShouldBeGroupedTogether()
        {
            // Arrange
            var conversations = new List<FacebookConversation>()
            {
                new FacebookConversation() { Participants = new List<string>() { "a1", "a2" }},
                new FacebookConversation() { Participants = new List<string>() { "a1", "a2" }},
                new FacebookConversation() { Participants = new List<string>() { "a2", "a1" }}
            };

            // Act
            writer.WriteConversations("Alvin Stanescu", conversations, string.Empty, this.fileUtilityMock.Object);

            // Assert
            fileEntries.Count.Should().Be(1);
        }
    }
}
