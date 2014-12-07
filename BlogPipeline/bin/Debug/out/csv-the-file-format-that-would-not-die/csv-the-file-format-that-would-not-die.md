# CSV the file format that would not die #

The comma seperated value format is ubiquitous, simple and yet at the same time quite a massive pain to work with.  The lack of a standard, even to the point of "comma" meaning a comma, tab or any old delimiter, or how a record can have varying columns, or the use of the *reserved* delimiter as data makes parsing CSV fun fun fun! FUN!

Given it's age and ubiquity it's surprising that there isn't a native, or .NET framework, library available to us...

Sure there's [Microsoft.VisualBasic.FileIO.TextFieldParser](http://msdn.microsoft.com/en-us/library/ms128079.aspx), but that namespace is nasty uh-huh; or using the OLEDB JET driver to treat it as a datasource, which sounds even worse; or [this](http://www.codeproject.com/Articles/9258/A-Fast-CSV-Reader) or [that](http://filehelpers.sourceforge.net/) (which are probably the right choices).  Oh hey, and let's not forget [LINQ to CSV](http://www.codeproject.com/Articles/25133/LINQ-to-CSV-library)!

Convential wisdom states

 - someone else has already solved this problem, better than you can
 - don't use regex when the data isn't regular
 - the "right" answer for this problem is probably a lexer/parser

To hell with convential wisdom I say, here is my crack at it!

Given a CSV as a big old string we can parse it and map it onto a DTO class, thereby getting an uncool CSV back as a very nice enumerable of T.  And everyone likes an enumerable of T.

The map is a dictionary mapping a DTO property name to a CSV column ordinal.  As the CSV is processed we reflect over the DTO properties to find the type, and as long as it a string, integer or double, you're gold!  As you may be able to tell, I did not need any more types than that.  In fact, no-one should ever need more than three types. Right?

Now... this code has a number of issues, and I really wouldn't use it where performance counted.  Ideally we would stream the CSV source in, avoiding memory pressure, increasing speed and so on.  And we'd handle the many, varied and colourful deviations that CSV files can throw at you.  And avoid reflection. And split this out so the CSV parsing was it's own concern. And so on.

But... it was kinda fun to write.  It's fine for a small file, and it has the added goodness of automatically mapping a column from the CSV to a property on a class.

The actual "parser" as such I nicked from [here](http://www.blackbeltcoder.com/Articles/files/reading-and-writing-csv-files-in-c), it's not great either, but better than my first five minute attempt at trying to just use string.Split(',')

I'm pretty pleased to say I managed to ignore all convential wisdom, squeezing some RegEx, string manipulation and even a little reflection into this home brewed monstrosity.  And I certainly didn't write a lexer or parser.

In some ways I've replicated [LINQ to CSV](http://www.codeproject.com/Articles/25133/LINQ-to-CSV-library), but without the tight coupling and attribute decoration.  And probably also without the functionality, testing and performance, but that's a different issue.

<pre>
<code>
public static class CsvMapper
    {
        /// &lt;summary&gt;
        /// Map CSV records to an object
        /// &lt;/summary&gt;
        /// &lt;typeparam name="T"&gt;The type to map&lt;/typeparam&gt;
        /// &lt;param name="mapping"&gt;A map of Property names to the CSV column ordinal&lt;/param&gt;
        /// &lt;param name="csv"&gt;The raw CSV&lt;/param&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        public static IEnumerable&lt;T&gt; MapCsvTo&lt;T&gt;(Dictionary&lt;string, int&gt; mapping, string csv, bool skipFirstRow)
            where T : new()
        {
            var properties = new T().GetType().GetProperties();
            var propertyMap = GetPropertyMap(properties);
            var results = new List&lt;T&gt;();
            var records = Regex.Split(csv, "\r\n");

            foreach (var row in records)
            {
                if (skipFirstRow)
                {
                    skipFirstRow = false;
                    continue;
                }

                if (string.IsNullOrEmpty(row.Trim()))
                {
                    continue;
                }

                var columns = ParseRow(row);

                if (columns.Length == 0)
                {
                    continue;
                }

                var item = new T();

                foreach (var map in mapping)
                {
                    var property = propertyMap[map.Key];
                    property.SetValue(item, ConvertToType(property.PropertyType, columns[map.Value]), null);
                }

                results.Add(item);
            }

            return results;
        }

        private static Dictionary&lt;string, PropertyInfo&gt; GetPropertyMap(PropertyInfo[] properties)
        {
            var propertyMap = new Dictionary&lt;string, PropertyInfo&gt;();

            foreach (var property in properties)
            {
                propertyMap.Add(property.Name, property);
            }

            return propertyMap;
        }

        private static object ConvertToType(Type type, string value)
        {
            switch (type.Name)
            {
                case "String":
                    return value;

                case "Int32":
                    return Convert.ToInt32(value);

                case "Double":
                    return Convert.ToDouble(value);

                default:
                    break;
            }

            return value;
        }

        // TODO: from http://www.blackbeltcoder.com/Articles/files/reading-and-writing-csv-files-in-c
        // kind of unattractive as we look at every character
        // might work out OK if combined with a stream
        private static string[] ParseRow(string row)
        {
            var results = new List&lt;string&gt;();

            int position = 0;

            while (position &lt; row.Length)
            {
                string value;

                // Special handling for quoted field
                if (row[position] == '"')
                {
                    // Skip initial quote
                    position++;

                    // Parse quoted value
                    int start = position;
                    while (position &lt; row.Length)
                    {
                        // Test for quote character
                        if (row[position] == '"')
                        {
                            // Found one
                            position++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (position &gt;= row.Length || row[position] != '"')
                            {
                                position--;
                                break;
                            }
                        }
                        position++;
                    }
                    value = row.Substring(start, position - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = position;

                    while (position &lt; row.Length && row[position] != ',')
                    {
                        position++;
                    }

                    value = row.Substring(start, position - start);
                }

                results.Add(value);

                // Eat up to and including next comma
                while (position &lt; row.Length && row[position] != ',')
                {
                    position++;
                }
                if (position &lt; row.Length)
                {
                    position++;
                }
            }

            return results.ToArray();
        }
    }
</code>
</pre>