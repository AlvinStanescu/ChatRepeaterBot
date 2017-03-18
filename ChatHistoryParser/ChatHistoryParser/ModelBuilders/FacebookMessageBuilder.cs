using FacebookDataModel;
using System;
using System.Globalization;

namespace ChatHistoryParser
{
    public class FacebookMessageBuilder
    {
        private string sender;
        private DateTimeOffset sentDate;
        private string message;

        public FacebookMessageBuilder WithSender(string sender)
        {
            this.sender = sender;
            return this;
        }

        public FacebookMessageBuilder WithSentDate(string dateTime)
        {
            var formattedDateTime = dateTime.Replace(" at ", ",").Replace("UTC", "");
            this.sentDate = DateTimeOffset.Parse(formattedDateTime, CultureInfo.InvariantCulture);
            return this;
        }

        public FacebookMessageBuilder WithMessage(string message)
        {
            this.message = message;
            return this;
        }

        public FacebookMessage Build()
        {
            return new FacebookMessage()
            {
                Message = this.message,
                Sender = this.sender,
                SentDate = this.sentDate
            };
        }
    }
}
