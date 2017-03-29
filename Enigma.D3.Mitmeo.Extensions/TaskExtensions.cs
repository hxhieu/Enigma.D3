﻿using System.Threading;
using System.Threading.Tasks;

namespace Enigma.D3.Mitmeo.Extensions
{
    public static class TaskExtensions
    {
        public static void RunOnMainThread(this Task task)
        {
            Task.Run(async () => await task);
        }

        public static T GetResultSafe<T>(this Task<T> task)
        {
            if (SynchronizationContext.Current == null)
                return task.Result;

            if (task.IsCompleted)
                return task.Result;

            var tcs = new TaskCompletionSource<T>();
            task.ContinueWith(t =>
            {
                var ex = t.Exception;
                if (ex != null)
                    tcs.SetException(ex);
                else
                    tcs.SetResult(t.Result);
            }, TaskScheduler.Default);

            return tcs.Task.Result;
        }

    }
}
