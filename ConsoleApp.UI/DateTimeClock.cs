using System;

namespace ConsoleApp.UI
{
    public sealed class DateTimeClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}