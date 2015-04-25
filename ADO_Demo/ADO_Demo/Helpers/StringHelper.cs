using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Helpers
{
    public static class StringHelper
    {
        public static string CleanPhone(string phoneNumber)
        {
            //samples
            //      after hrs 555 555-5555                            555-555-5555
            //      555*555*5555                                      555-555-5555
            //      555 555  5555                                     555-555-5555
            //      (555) 555-5555 (bus)                              (555)555-5555
            //      555-555-5555 phone/fax                            555-555-5555
            //      555-555-5555 * Primary                            555-555-5555
            //      (555) 555-5555                                    (555)555-5555
            //      555 / 555-5555                                    555-555-5555
            //      Call Bob Notary at 555.555.5555 !                 555-555-5555
            //      555 555-5555 after 5p.m                           555-555-5555
            //      555-555-5555 try 1st if late                      555-555-5555
            //      555.555-5555 after 5:00 p.m.                      555-555-5555
            //      555-555-5555  (2nd)                               555-555-5555
            //      same as cell                                      
            //      open after 6:00 pm                                


            string p = phoneNumber; // shortening variable name for local-only repetitive code. DavB

            p = Regex.Replace(p, @"~", "-");

            //remove useless numbers
            p = Regex.Replace(p, @"\dst", "", RegexOptions.IgnoreCase); //remove 1st
            p = Regex.Replace(p, @"\dnd", "", RegexOptions.IgnoreCase); //remove 2nd
            p = Regex.Replace(p, @"\drd", "", RegexOptions.IgnoreCase); //remove 3rd
            p = Regex.Replace(p, @"\dth", "", RegexOptions.IgnoreCase); //remove 4th 5th 6th
            p = Regex.Replace(p, @"\D\d\s?:\d\d?\D", "", RegexOptions.IgnoreCase); //remove time numbers 5 :55   55:55  5:5 surrounded by non-numerals
            p = Regex.Replace(p, @"\D\d\D?[ap]\.?m", "", RegexOptions.IgnoreCase); //remove time numbers 5pm 5 a.m.   non-numberal  then numeral   then optional nonnumeral    then either A or P  then optional period    then M

            // setup for extensions
            p = Regex.Replace(p, @"\wext", "", RegexOptions.IgnoreCase); // remove the 'ext' characters within other words (eg "Text") ... any character followed by 'ext'
            p = Regex.Replace(p, @"ext", "~~~", RegexOptions.IgnoreCase);
            //            p = Regex.Replace(p, @"\sx\s", "~~~", RegexOptions.IgnoreCase); // space x space
            p = Regex.Replace(p, @"\sx\.?\s", "~~~", RegexOptions.IgnoreCase); // space x (optional period). space
            //            p = Regex.Replace(p, @"\sx(?<num>\d+)", "~~~${num} ", RegexOptions.IgnoreCase); // space x number   
            p = Regex.Replace(p, @"\sx\.?(?<num>\d+)", "~~~${num}", RegexOptions.IgnoreCase); // space x (optional period). number  ... (?<num>\d+) catches a group of numbers as a group named 'num' which is persisted by name in the replacement by using ${num}
            p = Regex.Replace(p, @"(?<num1>\d)x(?<num2>\d+)", "${num1}~~~${num2}", RegexOptions.IgnoreCase); // A-number x Many-numbers   ... (?<num2>\d+) and catches a group of numbers as a group named 'num2' which is persisted by name in the replacement by using ${num2}

            //remove all but digits, dashes, parenthesis
            p = Regex.Replace(p, @"[^\d\(\)\-~]", "-"); // carrot means opposite of what's in group[] ...keep digits or space or parenthesis or dashes or squiggle ... replace others with single dash

            //remove all duplicate dashes
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash

            //remove parenthesis and dash combo
            p = Regex.Replace(p, @"\(\)", ""); //   remove empty parenthesis
            p = Regex.Replace(p, @"\(\-\)", ""); // remove empty parenthesis with just a dash inbetween (-)
            p = Regex.Replace(p, @"\-\(", "("); //   remove parenthesis with just a dash just before -(
            p = Regex.Replace(p, @"\-\)", ")"); //   remove parenthesis with just a dash just before -)
            p = Regex.Replace(p, @"\(\-", "("); //   remove parenthesis with just a dash just after  (-
            p = Regex.Replace(p, @"\)\-", ")"); //   remove parenthesis with just a dash just after  )-

            //remove solitary parenthesis
            if (p.Contains("(") && !p.Contains(")"))
            {
                p = Regex.Replace(p, @"\(", "-"); // remove solitary parenthesis , replace with single dash                     
            }
            if (p.Contains(")") && !p.Contains("("))
            {
                p = Regex.Replace(p, @"\)", "-"); // remove solitary parenthesis , replace with single dash                     
            }

            //final  cleanup of duplicate dashes
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash
            p = Regex.Replace(p, @"\-\-", "-"); // two dashes ... replace with single dash
            p = Regex.Replace(p, @"^\-", ""); // remove leading dash
            p = Regex.Replace(p, @"\-$", ""); // remove trailing dash

            // Re-Add extension
            p = Regex.Replace(p, @"\-~", "~"); // remove dash before extension
            p = Regex.Replace(p, @"~\-", "~"); // remove dash trailing extension
            p = Regex.Replace(p, @"~~~", " ext."); // remove trailing dash

            if (Regex.IsMatch(p, @"^\d{10}$")) // exaclty 10 digits 
            {
                p = p.Insert(3, "-").Insert(7, "-"); // turns 8005556666 into 800-555-6666
            }
            if (Regex.IsMatch(p, @"^\d{11}$")) // exaclty 10 digits 
            {
                p = p.Insert(1, "-").Insert(5, "-").Insert(9, "-"); // turns 18005556666 into 1-800-555-6666
            }
            p = Regex.Replace(p, @"(?<num1>\d)x(?<num2>\d+)", "${num1}~~~${num2}", RegexOptions.IgnoreCase); // A-number x Many-numbers   ... (?<num2>\d+) and catches a group of numbers as a group named 'num2' which is persisted by name in the replacement by using ${num2}
            p = Regex.Replace(p, @"(?<first>\(?\d{3}\)?\-?\d{3}\-\d{4})\-(?<second>\(?\d{3}\)?\-?\d{3}\-\d{4})", "${first} ${second}"); // repalces "222-333-4444-555-666-7777" with "222-333-4444 555-666-7777" (space in the middle) when they entered two numbers on one line.  ..uses  (?<first> **expression** ) and catches as a group named 'first' which is persisted by name in the replacement by using ${first}

            return p;


        }


        public static string CleanZip(string theZip)
        {
            //is the dash missing in a two part zip?
            if (Regex.IsMatch(theZip, @"^\d{9}$"))
            {
                theZip = theZip.Insert(5, "-");
            }
            // return it if it's a match, or else just clear it.
            if (Regex.IsMatch(theZip, Globals.gRegexOfZip))
            {
                return theZip;
            }
            return ""; //No way to fix a bad zip, just return empty string
        }

        public static string CleanNoHtml(string original)
        {
            if (!Regex.IsMatch(original, Globals.gRegexOfHtml))
            {
                original = Regex.Replace(original, "[<>]", "-"); // intended to remove html markup, can't determine whether to replace with 'greater than' or 'arrow' so using a dash
                original = Regex.Replace(original, @"\{", "("); // intended to invalidate a javascript call, but retain a similar meaning to the user
                original = Regex.Replace(original, @"\}", ")"); // intended to invalidate a javascript call, but retain a similar meaning to the user
            }

            return original;
        }

        public static string CleanOnlyAlphaNumeric(string original)
        {
            return Regex.Replace(original, @"[\W]", "").Trim(); // \W finds anything not alphanumeric, this removes it.  \W is equivalent to [^a-zA-Z_0-9]  (^ means NOT) 
        }

        public static string GetRegexMatches(string targetText, string regexPattern, string delimiter)
        {
            StringBuilder resultBuilder = new StringBuilder();

            //get all matches i the string
            Match matches = new Regex(@"(?<found>" + regexPattern + ")", RegexOptions.IgnoreCase).Match(targetText); // http://msdn.microsoft.com/en-us/library/twcw2f1c.aspx  (\[\w*\]) with parenthesis places the matchj into a group (?<found>  gives that group the name 'found'     (?< name > subexpression) Captures the matched subexpression into a named group.
            while (matches.Success)
            {
                // get the match result by name
                Group g = matches.Groups["found"];//.Groups[1];
                CaptureCollection cc = g.Captures;

                //NOTE other group names could be implemented

                //place matched text in the result
                foreach (Capture capture in g.Captures)
                {
                    resultBuilder.Append(capture.Value + delimiter);
                }

                //if multiple groups are defined in the Regex, get the next group...not the intention of this method to have multiple groups...untested
                matches = matches.NextMatch();
            }
            return resultBuilder.ToString().Trim();
        }


        public static string GenerateRandomAlphaCharacters(int length)
        {
            string availables = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return GetRandomText(availables, length);
        }

        public static string GenerateRandomAlphaNumerics(int length)
        {
            string availables = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789"; // NOTE! no letter "O"
            return GetRandomText(availables, length);
        }

        private static string GetRandomText(string availableCharacter, int length)
        {
            int endLength = availableCharacter.Length;
            Random random = new Random();
            if (length < 1) { return ""; }

            if (length < 9) // use += Not StringBuilder, this is actually faster for very short strings and few-ish iterations
            {
                string result = "";
                for (int i = 0; i < length; i++)
                {
                    int index = random.Next(0, endLength);
                    result += availableCharacter.Substring(index, 1);
                }
                return result;
            }
            else // use StringBuilder not +=
            {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < length; i++)
                {
                    int index = random.Next(0, endLength);
                    result.Append(availableCharacter.Substring(index, 1));
                }
                return result.ToString();
            }
        }

        /// <summary>
        /// This method ensures that '\n' is returned as '\r\n' without duplicating '\r' accidentally.
        /// targetString = StringHelper.FixLineBreaks(targetString)
        /// </summary>
        public static string FixLineBreaks(string targetText)
        {
            return Regex.Replace(targetText, "(\r)?\n", "\r\n"); // This regex looks for "\r\n" or "\n" and replaces either with "\r\n" which ensures proper spacing in v1
        }

        /// <summary>
        /// This method (from the internet) will get the index of the nth occurance of a string. Be sure the string doesn't have REGEX escapes in it.
        /// </summary>
        /// <param name="target">The string off which this extension method will fire</param>
        /// <param name="value">The value to find within the string</param>
        /// <param name="n">The count to find, and thus return it's index. If not found, then -1 is returned</param>
        /// <returns></returns>
        public static int IndexOfNth(this string target, string value, int n)
        {
            // this Method is adapted from http://stackoverflow.com/questions/186653/c-sharp-indexof-the-nth-occurrence-of-a-string
            string regex = "((" + value + ").*?){" + n + "}"; // concat strings outside of method parameters
            Match m = Regex.Match(target, regex);

            if (m.Success) { return m.Groups[2].Captures[n - 1].Index; }
            else { return -1; }
        }

        /// <summary>
        /// This method will shorten a long string to be smaller than the supplied length AND smaller than the number of line breaks
        /// </summary>
        /// <param name="str">The string to shorten</param>
        /// <param name="maxLength">The largest length the string can be before adding an elipses "..."</param>
        /// <param name="maxLineBreaks">pass 0 or less to ignore this int. If line breaks are restricted, how many lines may exist? if 2, then 2nd linebreak will be removed (if found) so that the total number of lines is 2. This assumes '\n' as the line break</param>
        /// <returns></returns>
        public static string AbbreviateText(string str, int maxLength, int maxLineBreaks)
        {
            if (str.IsEmptyX()) { return ""; }
            str = str.Trim();

            int startLength = str.Length;
            // Jira task... the summary description should not be longer than 200 character and not occupy more than 2 lines. https://interject.atlassian.net/browse/NSN-87
            // the full description is only available on the profile page.
            if (str.Length > maxLength)
            {
                str = str.Substring(0, maxLength);
                // did we cut off a word in the middle? is there a space near the end?
                int lastSpace = str.LastIndexOf("");
                if (maxLength - lastSpace < 12 && lastSpace > 0) { str = str.Substring(0, str.LastIndexOf(" ")); }// remove the partial of the last word
            }

            if (maxLineBreaks > 0)
            {
                // are there line breaks?
                int secondIndexOfLineBreak = str.IndexOfNth("\n", maxLineBreaks);
                if (secondIndexOfLineBreak > 0) { str = str.Substring(0, secondIndexOfLineBreak); }
            }


            //did we cut it off with line breaks at the end?
            while (str.EndsWith("\r") || str.EndsWith("\n"))
            {
                str = str.Substring(0, str.Length - 2);
            }

            // add elipses if needed
            if (startLength > str.Length) { str = str + "..."; }

            return str;
        }

    }
}
