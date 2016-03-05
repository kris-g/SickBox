using System;
using System.Threading;
using KrisG.Utility.Interfaces;
using log4net;

namespace KrisG.Utility
{
    public class RetryAction : IRetryAction
    {
        private readonly ILog _log;

        public RetryAction(ILog log)
        {
            _log = log;
        }

        public void DoWithRetries(Action action, int numberOfAttempts, TimeSpan delayBetweenRetries)
        {
            // hacky null return to keep a single implementation of retry logic
            DoWithRetries<object>(() =>
            {
                action();
                return null;
            }, numberOfAttempts, delayBetweenRetries);
        }

        public TResult DoWithRetries<TResult>(Func<TResult> action, int numberOfAttempts, TimeSpan delayBetweenRetries)
        {
            int attempt = 1;
            bool succeeded = false;

            while (attempt <= numberOfAttempts && !succeeded)
            {
                try
                {
                    var result = action();
                    succeeded = true;
                    return result;
                }
                catch (Exception ex)
                {
                    if (attempt < numberOfAttempts)
                    {
                        _log.Error(string.Format("Attempt {0} of {1} failed, retrying in {2}", attempt, numberOfAttempts, delayBetweenRetries), ex);
                        Thread.Sleep(delayBetweenRetries);
                    }
                    else
                    {
                        _log.Error(string.Format("Failed after max number of attempts {0}", numberOfAttempts));
                        throw;
                    }
                }
                finally
                {
                    attempt++;
                }
            }

            return default(TResult);
        }
    }
}