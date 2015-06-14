[NDepend](http://www.ndepend.com/).  That’s the answer.

However, the question was, how can I visualize dependancies in a .NET assembly? And as it turns, quite easily indeed.

First of all, this is what I am talking about.  Here is a graph showing constructor injection (you know, IoC, DI, etc.) [in an application](https://bitbucket.org/benmcevoy/mp3grabber) I wrote a while back that downloads MP3 from RSS/ATOM feeds.

[![alt text][1]](http://benmcevoy.co.nz/blog/get/media_injection_isom.png)

*The Downloader model class expects Download and RSS Managers to be injected, among other things.  You can also see possible issues, such as a concrete reference to History, instead of IHistory, for example.*

When you use a tool like Castle, or Unity, or Structure map, etc, you will become familiar with the term “[dependency graph](https://secure.wikimedia.org/wikipedia/en/wiki/Dependency_graph)” – this is literally a visualization of such a graph.

We can also visualize class inheritance and interface implementations:

[![alt text][2]](http://benmcevoy.co.nz/blog/get/media_inherit_isom.png)

*MainView is a WPF window by the look of it, and inherits a lot of stuff because of it.*

Or assembly references:
![alt text][3]

*The MediaGrabber assembly references the usual system assemblies, as well as Castle and the RSS and Download manager components.*

And if I ever get around to it, you could also visualize namespace references as well...

In essence all I am doing is some simple reflection over an assembly, and graphing relationships between types and/or assemblies.

<pre><code>
        private void ShowConstructorInjection(Assembly rootAssembly)
        {
            AddAllParentTypes(rootAssembly);

            foreach (var type in rootAssembly.GetTypes())
            {
                // inspect constructors
                foreach (var constructor in type.GetConstructors())
                {
                    // here we look at constructor injection
                    // to determine a "dependency"
                    foreach (var param in constructor.GetParameters())
                    {
                        try
                        {
                            AddVertex(param.ParameterType.FullName);
                            GraphToVisualize.AddEdge(new Edge&lt;object&gt;(param.ParameterType.FullName, type.FullName));
                        }
                        catch { }
                    }
                }
            }
        }
</code></pre>

The graphing and visualization is handled by the most excellent [QuickGraph](http://quickgraph.codeplex.com/) and [Graph#](http://graphsharp.codeplex.com/) respectively.

You can grab the [source code here](https://bitbucket.org/benmcevoy/dependancy-graph) and have a play.  I can tell you now, it’s pretty brittle and will crash a lot :)

For comparison, this is a small part of DotNetNuke6, constructor injection (that's System.Object in the center, btw...):

[![alt text][4]](http://benmcevoy.co.nz/blog/get/dnn6_injection_isom.png)


  [1]: assets/media_injection.JPG "The Downloader model class expects Download and RSS Managers to be injected, among other things.  You can also see possible issues, such as a concrete reference to History, instead of IHistory, for example"
  [2]: assets/media_inherit.jpg "MainView is a WPF window by the look of it, and inherits a lot of stuff because of it"
  [3]: assets/media_assembly.JPG "The MediaGrabber assembly references the usual system assemblies, as well as Castle and the RSS and Download manager components"
  [4]: assets/dnn6_injection.JPG "The pain that is DNN"