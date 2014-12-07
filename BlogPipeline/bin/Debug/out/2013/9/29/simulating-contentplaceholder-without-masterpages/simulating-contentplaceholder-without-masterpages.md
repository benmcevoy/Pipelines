# Simulating Content PlaceHolder without MasterPages #

Oh back in the deeply wrong world of WebForms...

So somewhere in the misty depths of an ASPX page I wish to render out all the scraps of javascript that any ASCX user controls may require. You could use the ScriptManager and RegisterClientScriptBlock (and LoadScriptsBeforeUI=false), or be very tidy and bundle and minify all the little script snippets together.

Other times it is desirable to just keep all the HTML and script together in a single ASCX.

With a master page we can use ContentPlaceHolder, but in just a normal ASPX page?

Behold.  Add a placeholder to the ASPX, prehaps at the bottom of the page.

<pre><code>
&lt;asp:PlaceHolder runat="server" ID="ScriptsPlaceHolder"&gt;&lt;/asp:PlaceHolder&gt;
</code></pre>

In the user control we have a little javascript snippet, prehaps to hook up a click event. Who knows.

<pre><code>
    html normally goes here

    &lt;my:RenderPlaceHolder runat="server" PlaceHolderId="ScriptsPlaceHolder"&gt;
        &lt;content&gt;
           &lt;script type="text/javascript"&gt;
              console.log('this script is rendered from a user control 
								into it's parents placeholder');
           &lt;/script&gt;        
        &lt;/content&gt;
    &lt;/my:RenderPlaceHolder&gt;
</code></pre>

The RenderPlaceHolder control will look up into the page and find the corresponding PlaceHolder control, dumping it's content into it.

<pre><code>
    using System.Web.UI;

    [ParseChildren(true, "Content")]
    [PersistChildren(false)]
    public class RenderPlaceHolder : UserControl
    {
        protected override void Render(HtmlTextWriter writer)
        {
            // you should probably do some null reference checks here...
            Page.FindControl(PlaceHolderId).Controls.Add(Content);
            base.Render(writer);
        }

        public Control Content { get; set; }

        public string PlaceHolderId { get; set; }
    }

</code></pre>
