using System;

namespace ConsoleApp.UI
{
    public interface IClock
    {
        DateTime UtcNow
        {
            get;
        }
    }
}