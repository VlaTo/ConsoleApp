using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal static class CancellationTokenExtensions
    {
        public static async Task AsTask(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using (var disposable = cancellationToken.Register(() => tcs.SetResult(true)))
            {
                await tcs.Task;
            }
        }
    }
}