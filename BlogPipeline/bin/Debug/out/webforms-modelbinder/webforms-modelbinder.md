**UPDATE: Well I've fleshed this out a little.  [Source is available][1], with model binding and DataAnnotation validations.**

I was a bit unhappy with my previous attempt at model binding, so I've had a bit more of a read and play.

Firstly, it's great to see the RepeaterItem has an [ItemType][2] property. Up until now I would do something like:

<pre><code>

       &lt;asp:Repeater runat="server"&gt;
          &lt;ItemTemplate&gt;
            &lt;label&gt;Product&lt;/label&gt;
            &lt;input type="text" value="AsProductViewModel(Container).Name" /&gt;
          &lt;/ItemTemplate&gt;
       &lt;/asp:Repeater&gt;

</code></pre>

<pre><code>

       protected ProductViewModel AsProductViewModel(RepeaterItem item)
       {
           return item.DataItem as ProductViewModel;
       }

</code></pre>

By setting ItemType I can now access the properties directly.

<pre><code>

&lt;asp:Repeater runat="server" ItemType="WebApplication2.ProductViewModel"&gt;
   &lt;ItemTemplate&gt;
      &lt;label&gt;Product&lt;/label&gt;
      &lt;input type="text" value="&lt;%# Item.Name %&gt;" /&gt;
   &lt;/ItemTemplate&gt;
&lt;/asp:Repeater&gt;

</code></pre>

And that makes me pretty happy.

**UPDATE: No, that is just all wrong.  This applies to .NET 4.5.  Prior to that there is no ItemType property available, so back to using the AsModel helper.  Bugger.**

But better (maybe if you think using the built in WebControls is better...) was reading [this fairly old post off MSDN][3] on binding WebControls back to an object.  This was rather similar to what I had made earlier.  The article has the good idea of looking for .Text or .Value property names, rather than testing for a specific control.  I have gone for a possibly less flexible approach and test for the least derived class I can think of, e.g. ListControl will capture DropDownList, CheckBoxList etc.  

On reflection (ha!) I think the original authors idea might be better.  It's fairly conventional to have a .Text property if you have some sort of text editing control, but there is no guarantee you derived from TextBox... ah well, back to the drawing board.   

<pre><code>
    &lt;asp:Repeater ID="TestRepeater" runat="server" ItemType="WebApplication2.Product"&gt;
        &lt;ItemTemplate&gt;
            &lt;asp:TextBox runat="server" ID="name" Text="&lt;%# Item.name %&gt;" /&gt;
            &lt;asp:TextBox ID="id" runat="server" Text="&lt;%# Item.id %&gt;" /&gt;
            &lt;asp:CheckBox runat="Server" ID="isSelected" Checked="&lt;%# Item.isSelected %&gt;" /&gt;
            &lt;asp:DropDownList runat="server" ID="age" name="product[0].age" SelectedValue='&lt;%# Item.age %&gt;'&gt;
                &lt;asp:ListItem Value="21" /&gt;
                &lt;asp:ListItem Value="31" /&gt;
                &lt;asp:ListItem Value="41" /&gt;
            &lt;/asp:DropDownList&gt;
            &lt;asp:Calendar runat="server" ID="MyDate" SelectedDate="&lt;%# Item.MyDate %&gt;" /&gt;
        &lt;/ItemTemplate&gt;
    &lt;/asp:Repeater&gt;

</code></pre>


<pre><code>

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Bind();
        }

        protected void SomeButton_Click(object sender, EventArgs e)
        {
            var pp = RepeaterToModel&lt;Product&gt;(TestRepeater);
        }

        private void Bind()
        {
            var products = new List&lt;Product&gt;();

            products.Add(new Product() { name = "test1", id = 1, age = 31 });
            products.Add(new Product() { name = "test2", id = 2, age = 31 });
            products.Add(new Product() { name = "test3", id = 3, age = 21 });

            TestRepeater.DataSource = products;
            TestRepeater.DataBind();
        }

        private IEnumerable&lt;T&gt; RepeaterToModel&lt;T&gt;(Repeater repeater) where T : new()
        {
            var results = new List&lt;T&gt;();

            foreach (RepeaterItem item in repeater.Items)
            {
                var result = ControlToModel&lt;T&gt;(item);

                results.Add(result);
            }

            return results;
        }

        private T ControlToModel&lt;T&gt;(Control source) where T : new()
        {
            var result = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                foreach (Control control in source.Controls)
                {
                    if (control.ID == property.Name)
                    {
                        if (control is TextBox)
                        {
                            property.SetValue(result, AutoMapper.Mapper.Map((control as TextBox).Text, typeof(string), property.PropertyType), null);
                            break;
                        }

                        if (control is HiddenField)
                        {
                            property.SetValue(result, AutoMapper.Mapper.Map((control as HiddenField).Value, typeof(string), property.PropertyType), null);
                            break;
                        }

                        if (control is ListControl)
                        {
                            property.SetValue(result, AutoMapper.Mapper.Map((control as ListControl).SelectedValue, typeof(string), property.PropertyType), null);
                            break;
                        }

                        if (control is CheckBox)
                        {
                            property.SetValue(result, AutoMapper.Mapper.Map((control as CheckBox).Checked, typeof(bool), property.PropertyType), null);
                            break;
                        }

                        if (control is Calendar)
                        {
                            property.SetValue(result, AutoMapper.Mapper.Map((control as Calendar).SelectedDate, typeof(DateTime), property.PropertyType), null);
                            break;
                        }
                        break;
                    }
                }
            }

            return result;
        }
    }
}
</code></pre>

After a little more wine and a little benchmarking I now have this:

<pre><code>
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public class WebFormModelBinder
    {
        private readonly Dictionary&lt;string, PropertyInfo&gt; _controlPropertyCache = new Dictionary&lt;string, PropertyInfo&gt;();

        public IEnumerable&lt;T&gt; RepeaterToObjects&lt;T&gt;(Repeater repeater) where T : new()
        {
            var results = new List&lt;T&gt;();

            foreach (RepeaterItem item in repeater.Items)
            {
                results.Add(ControlToObject&lt;T&gt;(item));
            }

            return results;
        }

        public T ControlToObject&lt;T&gt;(Control source) where T : new()
        {
            var result = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                foreach (Control control in source.Controls)
                {
                    if (control.ID == property.Name)
                    {
                        if (TryFindAndSetObjectProperty&lt;T&gt;(control, "Checked", typeof(bool), result, property))
                        {
                            break;
                        }

                        if (TryFindAndSetObjectProperty&lt;T&gt;(control, "Text", typeof(string), result, property))
                        {
                            break;
                        }

                        if (TryFindAndSetObjectProperty&lt;T&gt;(control, "Value", typeof(string), result, property))
                        {
                            break;
                        }

                        if (TryFindAndSetObjectProperty&lt;T&gt;(control, "SelectedValue", typeof(string), result, property))
                        {
                            break;
                        }

                        if (TryFindAndSetObjectProperty&lt;T&gt;(control, "SelectedDate", typeof(DateTime), result, property))
                        {
                            break;
                        }
                        break;
                    }
                }
            }

            return result;
        }

        private bool TryFindAndSetObjectProperty&lt;T&gt;(Control source, string sourcePropertyName, Type sourcePropertyType, T destination, PropertyInfo destinationPropertyInfo)
        {
            if (_controlPropertyCache.ContainsKey(source.ID))
            {
                destinationPropertyInfo.SetValue(destination, Convert.ChangeType(_controlPropertyCache[source.ID].GetValue(source, null), destinationPropertyInfo.PropertyType), null);
                return true;
            }

            var properties = source.GetType().GetProperties();

            foreach (var pi in properties)
            {
                if (pi.Name == sourcePropertyName && pi.PropertyType == sourcePropertyType)
                {
                    try
                    {
                        destinationPropertyInfo.SetValue(destination, Convert.ChangeType(pi.GetValue(source, null), destinationPropertyInfo.PropertyType), null);
                        _controlPropertyCache[source.ID] = pi;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
</code></pre>


  [1]: https://bitbucket.org/benmcevoy/webforms.framework
  [2]: http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.repeateritem.itemtype%28v=vs.90%29.aspx
  [3]: http://msdn.microsoft.com/en-us/library/aa478957.aspx#aspformbinding_topic2