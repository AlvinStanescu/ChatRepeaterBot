using FacebookDataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ChatHistoryParser
{
    public class FacebookConversationBuilder
    {
        private DateTimeOffset startDate;
        private DateTimeOffset endDate;
        private ICollection<string> participants;
        private IList<FacebookMessage> messages;

        public FacebookConversationBuilder WithConversations(IList<FacebookMessage> messages)
        {
            this.messages = messages;
            return this;
        }

        public FacebookConversationBuilder WithParticipants(ICollection<string> participants)
        {
            this.participants = participants;
            return this;
        }

        public FacebookConversationBuilder WithStartDate(DateTimeOffset startDate)
        {
            this.startDate = startDate;
            return this;
        }

        public FacebookConversationBuilder WithEndDate(DateTimeOffset endDate)
        {
            this.endDate = endDate;
            return this;
        }

        public FacebookConversation Build()
        {
            return new FacebookConversation()
            {
                Messages = this.messages.OrderBy(m => m.SentDate).ToList(),
                EndDate = this.endDate,
                StartDate = this.startDate,
                Participants = this.participants                
            };
        }
    }
}
