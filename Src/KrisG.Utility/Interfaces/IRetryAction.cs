using System;

namespace KrisG.Utility.Interfaces
{
    public interface IRetryAction
    {
        void DoWithRetries(Action action, int numberOfAttempts, TimeSpan delayBetweenRetries);
        TResult DoWithRetries<TResult>(Func<TResult> action, int numberOfAttempts, TimeSpan delayBetweenRetries);
    }
}