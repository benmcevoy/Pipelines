More Windows Phone/Silverlight/WPF/(and probably WinRT) goodness.

Sometimes you want to know that any work pushed to the UI dispatcher has completed.  For awhile I worked with the idea that [Thread.Sleep(1)][1] would let any pending work complete. It doesn't.

[A much better approach][2] is to use some [thread signalling][3]. The calling thread will wait until the dispatched work has completed, or give up after some period.

<pre><code>

        private static void WaitWithDispatcher(Action action)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                action();
                return;
            }

            var wait = new AutoResetEvent(false);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    action();
                }
                finally
                {
                    // always signal so code can continue
                    wait.Set();
                }
            });

            // wait for a reasonable period for dispatcher to complete
            wait.WaitOne(TimeSpan.FromSeconds(10));
        }

        private static void WithDispatcher(Action action)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                action();
                return;
            }

            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

</code></pre>


  [1]: http://msdn.microsoft.com/en-us/library/d00bd51t(v=vs.95).aspx
  [2]: http://stackoverflow.com/questions/9453553/windows-phone-how-to-tell-when-deployment-current-dispatcher-begininvoke-has-co
  [3]: http://msdn.microsoft.com/en-us/library/system.threading.autoresetevent(v=vs.95).aspx