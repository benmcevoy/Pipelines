There are many  logging frameworks and many techniques for capturing diagnostic information from your running systems.

You want sufficient information logged to help diagnose issues, but not so much as to impact performance. Most logging frameworks will have a verbosity level, allowing a high level of logging during development and diagnosis, and a minimal level during normal operation.

I currently have a preference for [nLog]( http://nlog-project.org/) mainly because I’ve used it on a few projects, it was easy to configure and it is getting the job done.  In the good old days we rolled our own, it was called S.W.I.N.E for Severe Warning INformation Error logging. Mainly we just liked the name :)

In order to make my life a little bit easier I usually have a static helper class that exposes methods for logging informational messages and exceptions and errors.

Most of the time this is pretty much I all need.  Perhaps I am an unsophisticated guy, but it works for me.

To keep the typing to a minimum we can automagically log the originating method of the logged message:

<pre><code>
       // grab calling frame
       var frame = new StackFrame(1, false);
       // grab the namespace and method name
       var callingMethod = string.Format("{0}.{1}",
            frame.GetMethod().ReflectedType.FullName,
            frame.GetMethod().Name);
</code>
</pre>

This is going to be taking a hit with the reflection, so I wrap these calls with a check to see if the logging level warrants it. It's also assuming that it was the previous call that is the originating method.  If you have a base class or some kind eventing to a central location for exceptions then this may cause you some issues.

Another handy helper lets me write out ADO commands.  Often we’ll be using some ORM or another which often have their own methods of emitting the executed SQL, but if your rocking ADO.NET then:

<pre><code>
        public static void Info(DbCommand command)
        {
            if (_logger.IsInfoEnabled)
            {
                var sb = new StringBuilder();
                
                sb.AppendLine(command.CommandType.ToString());
                sb.AppendLine(command.CommandText);

                foreach (DbParameter item in command.Parameters)
                {
                    sb.AppendLine(item.ParameterName + ":" + item.Value);
                }

                // grab calling frame
                var fr = new StackFrame(1, false);
                var callingMethod = string.Format("{0}.{1}",
                    fr.GetMethod().ReflectedType.FullName,
                    fr.GetMethod().Name);

                Debug.WriteLine(callingMethod);
                Debug.WriteLine(sb);

                _logger.Info(string.Format("INFO: {0}", callingMethod));
                _logger.Info(sb);
            }
        }
</code></pre>

With a little effort I could clean that up further to emit a statement that could be pasted into query analyser directly.

One last thing that has been proving useful isn’t in the logging class (but maybe it could be) and that’s a little ToJson() extension method on **object** to let us easily write out an object in a meaningful way using the **System.Web.Script.Serialization.JavaScriptSerializer**.

<pre><code>
  public static void ToJson(this object value)
  {
        return _serializer.Serialize(value);
  }
</pre></code>

