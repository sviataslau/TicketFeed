using System;
using System.Diagnostics;

namespace TicketFeed
{
    public sealed class MeasuredPerformance : IDisposable
    {
        private readonly Stopwatch stopwatch;

        public MeasuredPerformance()
        {
            this.stopwatch = Stopwatch.StartNew();
        }

        private long Milliseconds => this.stopwatch.ElapsedMilliseconds;

        public void Dispose()
        {
            this.stopwatch.Stop();
            System.Console.WriteLine($"Measured time: {Milliseconds} ms");
        }
    }
}