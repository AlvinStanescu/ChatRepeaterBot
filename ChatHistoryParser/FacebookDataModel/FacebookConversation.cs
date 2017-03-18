using System;
using System.Collections.Generic;
using System.Text;

namespace FacebookDataModel
{
    public class FacebookConversation
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public ICollection<string> Participants { get; set; }
        public IList<FacebookMessage> Messages { get; set; }
    }
}
