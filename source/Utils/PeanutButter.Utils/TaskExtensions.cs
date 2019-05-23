using System.Threading.Tasks;

namespace PeanutButter.Utils
{
    /// <summary>
    /// Extension methoss for tasks
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Runs a task which returns a result synchronously, returning that result
        /// </summary>
        /// <param name="task"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetResultSync<T>(
            this Task<T> task
        )
        {
            return task
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Waits on a void-result task for completion
        /// </summary>
        /// <param name="task"></param>
        public static void WaitSync(
            this Task task)
        {
            task.ConfigureAwait(false);
            task.Wait();
        }

    }
}