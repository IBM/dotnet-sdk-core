﻿namespace IBM.Cloud.SDK.Core.Sockets
{
    public class MarkTiming
    {
        public string word { get; }
        public double startTime { get; }

        public MarkTiming(string word, double startTime)
        {
            this.word = word;
            this.startTime = startTime;
        }

        public override string ToString()
        {
            return word + " " + startTime;
        }
    }
}
