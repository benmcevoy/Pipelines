# Building a Windows Phone App Part 3 #

## Isolated Storage ##

[A good rundown of the options](http://msdn.microsoft.com/en-us/magazine/hh205658.aspx) available show as well as IsolatedStorage there is also SQL CE and Page and Application state dictionaries.  State can be tombstoned if the app is deactivated.

A very interesting option is Sterling, which sounds a bit like an object oriented database: 

> Sterling can serialize almost any
> class and organizes instances using
> keys that you provide (any property on
> the instance may be designated as the
> key). Sterling also provides indexes
> that can be queried in memory for
> speed before loading 

[Uh, yep it is exactly that.](https://sterling.codeplex.com/)

I'm pulling XML and JSON files and as such I think it is most efficient to just cache these, allowing hydration of the object model from the URL resource or the disk.

using System.IO.IsolatedStorage;

There's one static method available here GetUserStoreForApplication

I have briefly used IsolatedStorageFile in another context - building a little plugin for Windows Media Center, which is like some crazy research project of Microsofts' with it's own declarative language, MCML, and whole bunch of things that look like prototype versions of Silverlight or WPF. It wasn't a weather app either, by the way.

To read and write files we use the [IsolatedStorageFileStream](http://msdn.microsoft.com/en-us/library/system.io.isolatedstorage.isolatedstoragefilestream%28v=vs.95%29.aspx), which needs no explaination.

I ended up with a helper class to wrap it up a little:

<pre><code>
public static class IsolatedStorageHelper
    {
        public static bool FileExists(string path)
        {
            return Store.FileExists(path);
        }

        public static void WriteFile(string path, string content)
        {
            using (var s = new IsolatedStorageFileStream(path, FileMode.Create, Store))
            {
                using (var w = new StreamWriter(s))
                {
                    w.Write(content);
                    w.Close();
                }
            }
        }

        public static string ReadFile(string path)
        {
            if (FileExists(path))
            {
                using (var reader = new StreamReader(Store.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    return reader.ReadToEnd();
                }
            }
            return "";
        }

        public static bool IsFileCacheExpired(string path, int cacheHours)
        {
            if (FileExists(path))
            {
                // file more than cacheHours old
                return DateTime.Now.AddHours(-cacheHours) > Store.GetLastWriteTime(path);
            }

            return true;
        }

        public static IsolatedStorageFile Store { get { return IsolatedStorageFile.GetUserStoreForApplication(); } }
    }
</code></pre>

I have a feeling I may run into concurrency issues some time in the future...  Watch the FileAccess/FileShare parameters...

## Making Requests for Resources ##

WebClient.DownloadAsync, "piece of piss", as they say.

<pre><code>
        public void Get(Uri uri, string path)
        {
            var wc = new WebClient();

            wc.DownloadStringCompleted += (s, e) =&gt;
            {
                IsolatedStorageHelper.WriteFile(path, e.Result);
                WeatherService.Current.Refresh();
            };

            wc.DownloadStringAsync(uri);
       }
</code></pre>

This did not work, throwing a NotSupported exception.

[Looking closer with fiddler](https://blogs.msdn.com/b/fiddler/archive/2010/10/15/fiddler-and-the-windows-phone-emulator.aspx?Redirected=true) but that shows me not much.

[Oh bummer](http://stackoverflow.com/questions/4588372/is-there-any-way-to-download-ftp-files-with-windows-phone-7-net-4) . . .

So, WebClient wraps HttpWebRequest and does not support ftp.

One of my requirements is to NOT have to provide hosting.  Also proxying the data could be against the BOM copyright/usage thing.

[Sockets?](http://msdn.microsoft.com/en-us/library/system.net.sockets.socket%28v=VS.96%29.aspx)

Great.  Now implement the FTP protocol.

I am starting to lose faith here. . .

Some rapid learnings later:

 - FTP is a text based protocol, like HTTP
 - You can do it "raw" with TELNET

So between the sockets example and [these raw commands](http://geekswithblogs.net/bigpapa/archive/2007/11/05/C-heart-RAW-FTP.aspx) I'm now getting:

<pre><code>
220-Welcome to the Bureau of Meteorology FTP service.
220-
220-                              Disclaimer
220-
220-You accept all risks and responsibility for losses, damages, costs and
220-other consequences resulting directly or indirectly from using this site and
220-any information or material available from it.
220-
220-To the maximum permitted by law, the Bureau of Meteorology excludes all
220-liability to any person arising directly or indirectly from using this
220-site and any information or material available from it.
220-
220-Always Check the Information
220-
220-Information at this site:
220-
220-. is general information provided as part of the Bureau of Meteorology's
220-  statutory role in the dissemination of information relating to
220-  meteorology.
220-. is subject to the uncertainties of scientific and technical research
220-. may not be accurate, current or complete
220-. is subject to change without notice
220-. is not a substitute for independent professional advice and users
220-  should obtain any appropriate professional advice relevant to their
220-  particular circumstances
220-. the material on this web site may include the views or recommendations
220-  of third parties, which do not necessarily reflect the views of the
220-  Bureau of Meteorology or indicate its commitment to a particular course of
220-  action.
220 
331 Please specify the password.
230 Login successful.
250 Directory successfully changed.
200 Switching to Binary mode.
227 Entering Passive Mode (134,178,63,130,233,247)
</code></pre>

I have some refactoring to do...  Hopefully I will end up with a really simple FTP client using Silverlight sockets.  This is a bit more than I was expecting, but making something like this will probably be quite useful. For someone.  [Maybe this guy.](http://stackoverflow.com/questions/4588372/is-there-any-way-to-download-ftp-files-with-windows-phone-7-net-4)

Looks like there will be a Part 4. And Part 5.  It's beer time now, but I hope tomorrow I will actually have something running on my real live phone and not just the emulator.