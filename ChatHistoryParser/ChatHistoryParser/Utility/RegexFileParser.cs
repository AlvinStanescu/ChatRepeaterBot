using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatHistoryParser
{
    public class RegexFileParser
    {
        public IList<T> ParseFileByRegex<T>(string filePath, Regex regex, Func<Match, T> matchParser, bool stopAfterFirstMatch = false)
        {
            var tempString = string.Empty;
            var values = new List<T>();

            // We have to process the file in chunks, since the file can be too big to store in memory
            using (var messagesFileStream = File.OpenRead(filePath))
            {
                var tempByteArray = new byte[65536];
                var readBytes = messagesFileStream.Read(tempByteArray, 0, 65536);

                while (readBytes != 0)
                {
                    var readString = Encoding.UTF8.GetString(tempByteArray, 0, readBytes);

                    tempString += readString;
                    var match = regex.Match(tempString);

                    // Last match indexes always increase in value, so no need for a max-heap or any efficient data structure, just store the last index.
                    var lastMatchEndIndex = 0;

                    while (match.Success)
                    {
                        values.Add(matchParser(match));

                        if (stopAfterFirstMatch)
                        {
                            return values;
                        }

                        lastMatchEndIndex = match.Index + match.Length;
                        match = match.NextMatch();
                    }

                    tempString = tempString.Substring(lastMatchEndIndex);

                    readBytes = messagesFileStream.Read(tempByteArray, 0, 65536);
                }
            }

            return values;
        }
    }
}
