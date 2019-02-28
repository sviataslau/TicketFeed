using System;

namespace TicketFeed.SDK
{
    public sealed class WithConsoleColor : IDisposable
    {
        public WithConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public void Dispose()
        {
            Console.ResetColor();
        }
    }
}