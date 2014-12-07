I’ve just been adding a few features to my [timesheet logger/parser](http://benmcevoy.com.au/blog/unstructured-data), just to make my life a little easier come Monday morning.  The timesheet logger is basically a WPF window with a TextBox in it, not very sophisticated. I had originally thought to give up on adding any text editor features to this and just have the parsing part as a separate app that used a FileWatcher to track a text file.  When the text file was saved the parser would kick in and update it’s stats.

Unfortunately I like playing too much, I guess I like seeing the hours of the day burn up and and knocking off time approach, or something.  At any rate I threw in a few more Text Editor type features.

Firstly – KeyBindings:

<pre><code>
    &lt;Window.InputBindings&gt;
        &lt;KeyBinding Command="{Binding NextDayCommand}" Gesture="ALT+RIGHT"/&gt;
        &lt;KeyBinding Command="{Binding PreviousDayCommand}" Gesture="ALT+LEFT"/&gt;
        &lt;KeyBinding Command="{Binding SaveCommand}" Gesture="CTRL+S"/&gt;
        &lt;KeyBinding Command="{Binding SyncCommand}" Gesture="F5"/&gt;
        &lt;KeyBinding Command="{Binding FindNextCommand}" Gesture="F3"/&gt;
        &lt;KeyBinding Command="{Binding FindCommand}" Gesture="CTRL+F"/&gt;
    &lt;/Window.InputBindings&gt;
</code></pre>

So you can quickly see what I’ve been upto.  InputBindings are sweet.  WPF allows gestures, which can include key presses, mouse movements and events and touch events.  Very sci-fi.

When it came time to implement the Find text feature I almost gave up. It’s 10pm and I’ve had a few wines.  I had a little google and saw this [universal find and replace dialog for wpf]( http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=173509&zep=FindReplaceTest%2fFindReplace%2fFindReplace.cs&rzp=%2fKB%2fmiscctrl%2fWPFFindReplace%2fFindReplaceTest.zip)

The Select method jumped out at me.  That was enough to kick off a very budget implementation that will do for me.  I want to find the text and then select it as a highlight.  TextBox has a CaretIndex property.  This was interesting – if the text is an array of bytes, some of which are letters, some of which are carriage returns and other control characters, then the Caret Index is the index in the array where the caret is and not where it is "on screen".  Makes sense :)  Which is a good idea, otherwise we might have to worry about line wrapping, font size and a bunch of presentation issues to work out where we were in the textbox.

I give you the FindTextBox control:

<pre><code>
using System;
using System.Windows;
using System.Windows.Controls;

namespace FindTextBox
{
    public class FindTextBox : TextBox
    {
        public string FindText
        {
            get { return (string)GetValue(FindTextProperty); }
            set { SetValue(FindTextProperty, value); }
        }

        public static readonly DependencyProperty FindTextProperty =
            DependencyProperty.Register("FindText", typeof(string), typeof(FindTextBox), new UIPropertyMetadata(""));

        public void Find(string findText)
        {
            this.FindText = findText;
            Find();
        }

        public void Find()
        {
            var start = this.CaretIndex;

            if (this.SelectedText.Length > 0)
            {
                start = start + this.SelectedText.Length;
            }

            this.Select(start, this.Text.Length);

            var st = this.SelectedText;
            var startindex = st.IndexOf(this.FindText, StringComparison.CurrentCultureIgnoreCase);

            if (startindex > 0)
            {
                this.Select(start + startindex, this.FindText.Length);
                this.ScrollToLine(this.GetLineIndexFromCharacterIndex(start + startindex));
            }
            else
            {
                this.Select(start, 0);
            }
        }        
    }
}

</code></pre>




