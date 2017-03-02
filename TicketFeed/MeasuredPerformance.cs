using System;
using System.Diagnostics;

namespace TicketFeed
{
    public class MeasuredPerformance : IDisposable
    {
        private readonly Stopwatch stopwatch;

        public MeasuredPerformance()
        {
            this.stopwatch = Stopwatch.StartNew();
        }

        public long Milliseconds => this.stopwatch.ElapsedMilliseconds;

        public void Dispose()
        {
            this.stopwatch.Stop();
            System.Console.WriteLine($"Measured time: {Milliseconds} ms");
        }
    }
}