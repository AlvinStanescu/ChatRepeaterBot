using System;

namespace FacebookDataModel
{
    public class FacebookMessage
    {
        public string Sender { get; set; }

        public DateTimeOffset SentDate { get; set; }

        public string Message { get; set; }
    }
}
