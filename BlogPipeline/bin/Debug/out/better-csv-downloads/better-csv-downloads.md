To often when offering a simple "save as CSV" function I see code that simply makes the database request, strings it up and sends it down the wire.

And more than once I have written code like this.

It's generally not a problem if:

- the requested data is small
- you have very few users
- you do not care about the impact on your servers memory

We can do better, and in a fairly simple way.

**By using an IQueryable<T> and paging we can easily limit the memory usage by only streaming one page at a time back to the user.**

This does have the downside of making multiple database requests, but I believe that to be preferable to simply pulling all the data at once.

Here we simply skip and take over the query.  My ToCsv code is a bit ropey and only does *what I need*, but there are plenty of similar functions to be found around the interwebs.

Whenever you make a database request, be it for downloading results or just populating a grid, you should always considering limiting the results.  There are very few times when the user *really* wants 100,000 records at once.

Usage:

<pre><code>

    using (var context = new DataContext())
            {
                var code = Request.QueryString["code"];

                // query and project to a flat structure for CSV
                var earnings = context.StockEarnings.Where(
                    s => s.StockCode == code)
                    .Select(earning => new
                    {
                        earning.StockCode,
                        earning.Year,
                        earning.Margin,
                        earning.CashFlow,
                        earning.ROE
                    }).AsQueryable();  // note that AsQueryable

                DownloadHelper.DownloadAsCsv(Context, earnings, code + "_earnings.csv");
            }


</code></pre>


<pre><code>
    public static void DownloadAsCsv&lt;T&gt;(HttpContext httpContext, IQueryable&lt;T&gt; query, string fileName, int pageSize = 500) where T : class
        {
            var pageNumber = 0;

            while (true)
            {
                var results = query.Skip(pageSize * pageNumber).Take(pageSize);

                if (!results.Any()) break;

                var csv = results.ToCsv();

                if (pageNumber == 0)
                {
                    var properties = typeof(T).GetProperties();
                    // a bit of reflection to generate the header row
                    var header = properties.Aggregate("", (current, propertyInfo) =&gt; current + string.Format("{0},", propertyInfo.Name));

                    header = header.TrimEnd(',');
                    header += "\r\n";

                    csv = csv.Insert(0, header);

                    httpContext.Response.Clear();
                    httpContext.Response.AddHeader("Content-Disposition",
                                                   "attachment; filename='" + fileName + "'");
                    httpContext.Response.ContentType = "text/comma-separated-values";
                }

                pageNumber++;

                httpContext.Response.Write(csv);
                // flush starts sending the response back to the user
                httpContext.Response.Flush();
            }

            httpContext.Response.End();
        }

    public static string ToCsv&lt;T&gt;(this IEnumerable&lt;T&gt; items)
            where T : class
        {
            var csvBuilder = new StringBuilder();
            var properties = typeof(T).GetProperties();

            foreach (T item in items)
            {
                var line = string.Join(",", properties.Select(p =&gt; p.GetValue(item, null).ToCsvValue()).ToArray());
                csvBuilder.AppendLine(line);
            }

            return csvBuilder.ToString();
        }

        private static string ToCsvValue&lt;T&gt;(this T item)
        {
            if (item == null) return "\"\"";

            if (item is string)
            {
                return string.Format("\"{0}\"", item.ToString().Replace("\"", "\\\""));
            }

            double dummy;

            if (double.TryParse(item.ToString(), out dummy))
            {
                return string.Format("{0}", item);
            }

            return string.Format("\"{0}\"", item);
        }


</code></pre>





