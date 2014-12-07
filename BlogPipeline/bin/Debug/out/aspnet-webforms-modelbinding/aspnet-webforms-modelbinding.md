I'm stuck in WebForms land at the moment.  Using asp:Repeaters to render a collection of objects, repeater is bound to an collection of viewmodels in the code behind (datasource/databind):

<pre><code>
      &lt;asp:Repeater runat="server"&gt;
         &lt;ItemTemplate&gt;
            &lt;label&gt;Product&lt;/label&gt;
            &lt;input type="text" name="product[&lt;%#Container.ItemIndex %&gt;].Name" value="AsProductViewModel(Container).Name" /&gt
         &lt;/ItemTemplate&gt;
      &lt;/asp:Repeater&gt;
</code></pre>

I *really* don't like OnItemBound and all that crap. Nor FindControl or any of that.

Instead I got me a glass of wine and budget Model Binding. Yee har.
And a helper to cast the Container.DataItem as ProductViewModel.

In the Form that is posted back we end up with something like:

 - product[0].Id
 - product[0].Name
 - product[1].Id
 - product[1].Name

And so on.  And on post back we can map it to a corresponding class.

var products = ModelBinder.Bind<Product>("product", this.Request.Form);

Not too sure if this or something like this will see production, but good fun.  Only works on simple types, but adequate for nice, flat view models.

I believe WebForms 4.5 has something like this built in. Did I mention I'm stuck in .NET 2.0?  And surely AutoMapper does this?

<pre><code>
    public static IEnumerable&lt;T&gt; Bind&lt;T&gt;(string prefix, NameValueCollection form)
           where T : new()
        {
            var results = new List&lt;T&gt;();
            var properties = new T().GetType().GetProperties();
            int index = -1;
            var indexRegex = new Regex(string.Format(@"^{0}\[(\d?)\].*$", prefix));

            Array.Sort(form.AllKeys);

            foreach (var key in form.AllKeys)
            {
                // already matched?
                if (key.StartsWith(string.Format("{0}[{1}]", prefix, index)))
                {
                    continue;
                }

                if (key.StartsWith(prefix))
                {
                    var item = new T();
                    var matches = indexRegex.Match(key);

                    if (matches.Success && matches.Groups.Count == 2)
                    {
                        if (Int32.TryParse(matches.Groups[1].Captures[0].Value, out index))
                        {
                            foreach (var property in properties)
                            {
                                var propertyKey = string.Format("{0}[{1}].{2}", prefix, index, property.Name);

                                if (form.AllKeys.Contains(propertyKey))
                                {
                                    property.SetValue(item, ConvertToType(property.PropertyType, form[propertyKey]), null);
                                }
                            }

                            results.Add(item);
                        }
                    }
                }
            }

            return results;
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

                //case "DateTime":
                //    return DateTime.Parse(value);
                //// Monday, July  2, 2012 00:38:41 UTC

                default:
                    break;
            }

            return value;
        }
</code></pre>