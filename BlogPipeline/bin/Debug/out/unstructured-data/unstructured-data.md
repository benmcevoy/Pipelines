As I am a GOOD BOY I have always kept a log of my professional work, spanning back from the late ‘90’s until now.  See I told you I was a good boy :)
The log comes in handy for reviewing any changes I made have made in the distant past, any notes or conversations and generally as a personal record that supplements change control, timesheeting and so on.

So what! I hear you say.  Well the log file was always plain text and  as such not too handy when it came to searching or reporting.

So what! OK, ok.  Log entries look like this:

<pre>
Tuesday 13 November 2007
------------------------------------------------------------------------------------------
7:30-8:00	Talking to Matt about resourcing.
8:30-10:30	ProjectName. Resolving deployment issues with click once. deployed new protoptype with PMP button/image in it  and calendar removed (as it's trial period has expired).

10:30-12:45	ProjectName. Setting up new solution.  Got the latest versions of CSLA and the WPF Ribbon bar and added these.
12:45-1:30	Lunch
1:30-5:30	ProjectName. Review requirements
5:30-8:30	Microsoft presentation on TabletPC/Ink and Linq/SQL
</pre>

And so on.

There are four interesting elements here:

- A date line
- A time period
- A project name
- Some comments

So with some judicious use of regular expressions we are able to build a parser and lo! – bring structure to the unstructured.

I warn you - this code ain’t that pretty, and the main parse routine relies heavily on assumptions and continues out of the loop early and things being null. And it crashes a lot if things ain’t just so. But it works for me so screw you.

<pre>
<code>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TimeLog
{
    public class LogParser
    {
        public static string LogDateFormat = "dddd dd MMMM yyyy";
        public static string TheIdealLine = "---------------------------------------------------------------------";

        private readonly Regex _dateLine = new Regex(
@"^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)
\s
(0?[1-9]|[12][0-9]|3[01])
\s
(January|February|March|April|May|June|July|August|September|October|November|December)
\s
(20[0-1][0-9])$", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

        private readonly Regex _timePeriod = new Regex(
@"^([012]?[0-9]:[0-5][0-9])
-
([012]?[0-9]:[0-5][0-9])
\s
.*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

        private readonly Regex _project = new Regex(
@"^[012]?[0-9]:[0-5][0-9]
-
[012]?[0-9]:[0-5][0-9]
[\s|\t]*
(.*?)\.(.*)$", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

        private readonly Regex _comments = new Regex(@"",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

        public Log Parse(IEnumerable<string> lines)
        {
            if (!lines.Any())
                return null;

            var log = new Log();

            var lineNumber = -1;
            Day day = null;
            TimeEntry timeEntry = null;

            foreach (var line in lines)
            {
                lineNumber++;

                if (string.IsNullOrEmpty(line))
                    continue;

                Debug.WriteLine(string.Format("Parsing Line {0}: {1}", lineNumber, line));

                var date = MatchDateLineFirstLine(line);

                if (date != null)
                {
                    if (day != null)
                    {
                        if (timeEntry != null)
                        {
                            day.TimeEntries.Add(timeEntry);
                            timeEntry = null;
                        }
                        log.Days.Add(day);
                    }

                    day = new Day() { Date = date.Value, LineNumber = lineNumber };
                    continue;
                }

                var secondDateLine = MatchDateLineSecondLine(line);
                if (!string.IsNullOrEmpty(secondDateLine))
                {
                    // could do something with it? nah...
                    continue;
                }

                var timePeriod = MatchTimePeriod(line, day == null ? DateTime.Today : day.Date);
                if (timePeriod != null)
                {
                    if (timeEntry != null)
                        day.TimeEntries.Add(timeEntry);

                    timeEntry = new TimeEntry() { StartDateTime = timePeriod.StartTime, EndDateTime = timePeriod.EndTime };
                    Debug.WriteLine("TimeEntry.Duration: " + timeEntry.Duration);
                }

                var project = MatchProject(line);
                if (!string.IsNullOrEmpty(project))
                {
                    timeEntry.ProjectName = project;
                }

                var comments = MatchComments(line);
                if (!string.IsNullOrEmpty(comments))
                {
                      if (timeEntry != null)
                    {
                        timeEntry.AddComment(comments);
                    }
                    continue;
                }
            }

            if (day != null)
            {
                if (timeEntry != null)
                {
                    day.TimeEntries.Add(timeEntry);
                }
                log.Days.Add(day);
            }

            return log;
        }

        private DateTime? MatchDateLineFirstLine(string line)
        {
            var l = line.Trim();
            DateTime result;

            var match = _dateLine.Match(l);

            if (match.Success)
            {
                if (DateTime.TryParse(l, out result))
                {
                    return result;
                }
            }

            return null;
        }

        private string MatchDateLineSecondLine(string line)
        {
            if (line.All(l => l == '-') && line.Length > 5)
            {
                // return the ideal line!
                return TheIdealLine;
            }

            return string.Empty;
        }

        public Period MatchTimePeriod(string line, DateTime date)
        {
            var l = line.Trim();
            Period period = null;

            var match = _timePeriod.Match(l);

            if (match.Success)
            {
                DateTime startTime;
                if (DateTime.TryParse(match.Groups[1].Value, out startTime))
                {
                    DateTime endTime;
                    if (DateTime.TryParse(match.Groups[2].Value, out endTime))
                    {
                        if (endTime < startTime)
                            endTime = endTime.AddHours(12);

                        startTime = date.Date + startTime.TimeOfDay;
                        endTime = date.Date + endTime.TimeOfDay;

                        period = new Period() { StartTime = startTime, EndTime = endTime };
                    }
                }
            }

            return period;
        }

        public string MatchProject(string line)
        {
            var l = line.Trim();
            var match = _project.Match(l);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        public string MatchComments(string line)
        {
            if (!string.IsNullOrEmpty(MatchProject(line)))
            {
                var l = line.Trim();
                var match = _project.Match(l);

                if (match.Success)
                {
                    return match.Groups[2].Value.Trim();
                }

                return string.Empty;
            }

            return line.Trim();
        }

    }
}


</code>
</pre>

So all that is hydrating a domain model that represents the elements we identified earlier. Plus a few calculated properties such as total hours per day and so on.

<pre><code>
    public class TimeEntry
    {
        public TimeEntry()
        {
            Comments = new List<string>();
        }

        public TimeSpan Duration { get { return EndDateTime.Subtract(StartDateTime); } }

        public double Minutes { get { return Duration.TotalMinutes; } }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string ProjectName { get; set; }

        public List<string> Comments { get; set; }

</code></pre>

So frickin what!  Well…. Now I can browse my text file by date.  Now I can get a report of hours spent on a project.  Now I have strong data I can work with.  Now I can find out just how long I spent at lunch over the last ten years.

![Alt text](http://benmcevoy.com.au/blog/get/TimeLog.jpg "from chaos comes less chaos")

I think that is sweet.  And all from a .txt file.
Hopefully by the time you read this (if anyone ever reads this) I’ll have shoved the code up to bitbucket.



