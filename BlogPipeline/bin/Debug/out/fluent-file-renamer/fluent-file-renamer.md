For some reason I seem to have a large collection of files that are named similarly, but not consistently. A fluent file renamer interface can fix that for me.

A fluent API is one where we always return the same instance, allowing chaining of method calls.  (Or prehaps the same type if we are a value object, think string.Replace("foo", "bar").Replace("fee", "fii") etc).

e.g.:

<pre><code>
    var renamer = new Renamer("my dODgy file Name.leet.720p.s01E02.mkv")
                .ChangeExtension("avi")
                .MakeTitle()
                .Replace("leet", "");
</code></pre>

The esssence of it is, always return **this** or an object of the same type.

<pre><code>
    public class Renamer
    {
        public FileName FileName { get; set; }

        public Renamer(string fileName)
        {
            FileName = new FileName(fileName);
        }

        public Renamer ChangeExtension(string newExtension) 
        {
            FileName.Extension = newExtension;
            return this; 
        }

        public Renamer Clean()
        {
            Replace(@"\.\.", @".")
                .Replace("  ", " ")
                .Replace("--", "-")
                .Replace("__", "_");

            return this;
        }

        public Renamer MakeTitle()
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            FileName.Name = textInfo.ToTitleCase(FileName.Name);
            
            Replace(" And ", " and ")
                .Replace(" Of ", " of ")
                .Replace(" In ", " in ")
                .Replace(" The ", " the ")
                .Replace(" Is ", " is ")
                .Replace(" For ", " for ")
                .Clean();
            return this;
        }

        public Renamer Replace(string match, string replace)
        {
            FileName.Name = Regex.Replace(FileName.Name, match, replace, RegexOptions.IgnoreCase);
            return this;
        }
    }
</code></pre>

I had intentions of providing a little WPF app over the top of this to open a directory and clean up all the file names therein.  The user would be able to add multiple rules, provide any arguments required and chain them all up for processing.

As it turns out that sort of scenario is better served by a Rules Engine or something similar.  As enamoured as I am with the fluent interface, I think i will end up refactoring it away into a class per rule.  And for same reason there is a voice in the back of my head muttering **expression tree**. C'est la vie.