using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public static class nsExtensionMethods
{
    /// <summary>
    /// This Extension Method will return the object's .ToString() result, but first check for null and if the object is null will return the paramter value as if it's a default.
    /// </summary>
    /// <param name="defaultIfNull">If the Object is null, what default should be provided instead?</param>
    /// <returns></returns>
    public static string ToStringOrX(this object originalObject, string defaultIfNull = "")
    {
        if (originalObject == null)
        {
            return defaultIfNull;
        }
        return originalObject.ToString();
    }

    #region String Extensions

    /// <summary>
    /// This is shorthand for string.IsNullOrEmpty(str)
    /// </summary>
    /// <param name="originalString"></param>
    /// <returns></returns>
    public static bool IsEmptyX(this string originalString)
    {
        return string.IsNullOrEmpty(originalString);
    }

    /// <summary>
    /// This is shorthand for (not) !string.IsNullOrEmpty(str)
    /// </summary>
    /// <param name="originalString"></param>
    /// <returns></returns>
    public static bool IsntEmptyX(this string originalString)
    {
        return !string.IsNullOrEmpty(originalString);
    }

    /// <summary>
    /// This uses a REGEX to verify that this is ina a valid email format/
    /// </summary>
    /// <param name="theEmail"></param>
    /// <returns></returns>
    public static bool IsEmailX(this string theEmail)
    {
        if (theEmail.IsEmptyX()) { return false; }
        return Regex.IsMatch(theEmail, Globals.gRegexOfEmail) //            must match lenient Regular expression
            && theEmail.IndexOf("@") == theEmail.LastIndexOf("@"); //       must not have multiple '@'s        
    }

    public static bool IsGuidX(this string expression)
    {
        // adapted from http://www.geekzilla.co.uk/View8AD536EF-BC0D-427F-9F15-3A1BC663848E.htm

        if (expression.IsntEmptyX())
        {
            Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

            return guidRegEx.IsMatch(expression);
        }
        return false;
    }


    /// <summary>  This returns an empty string if the supplied string is not a valid email. Otherwise it returns example   g*****@yahoo.com  </summary>
    public static string AsMaskedEmailX(this string theEmail)
    {
        theEmail = theEmail.Trim();
        if (!theEmail.IsEmailX()) { return ""; }

        int atIntdex = theEmail.IndexOf("@");
        string domain = theEmail.Substring(theEmail.IndexOf("@")); // eg.  @yahoo.com

        string asterixes = "****"; //default min length
        if (atIntdex > 3 && atIntdex < 15) { asterixes = "********************".Substring(0, atIntdex); } //matches length when easy, within reason

        return string.Format("{0}{1}{2}", theEmail.Substring(0, 2), asterixes, domain); //results in da********@notarysoftware.com or sa****@q.com for "sam@q.com"
    }

    /// <summary> This extension method will return the same string with HtmlEncoding applied </summary>
    public static string AsHtmlEncodedX(this string theString)
    {
        // NOTE That knockout data-bindings sometimes html-encode  strings also. 
        //  'value' IS encoded:        <div ><input type="text" data-bind="value: Company" /></div>
        //  'text'  IS encoded:        <div data-bind="    text: Company"></div>
        //  'html' NOT encoded:        <div data-bind="    html: Company"></div>

        return HttpUtility.HtmlEncode(theString);
    }
    /// <summary> This extension method will remove HtmlEncoding from the string, and return the plain text </summary>
    public static string AsHtmlDecodedX(this string theString)
    {
        return HttpUtility.HtmlDecode(theString);
    }
    /// <summary> This extension method will return the same string with UrlEncoding applied </summary>
    public static string AsUrlEncodedX(this string theString)
    {
        //  Url Encoding:                                      
        //  < > ` ~ ! ^ * - _ | \ [ ] { } ( ) . ' " @   #   $   +   =   /   ,   ;   %   &   :   ?
        //  < > ` ~ ! ^ * - _ | \ [ ] { } ( ) . ' " %40 %23 %24 %2B %3D %2F %2C %3B %25 %26 %3A %3F
        //
        //  NOTICE HTML BRACKETS ARE NOT URL ENCODED: <script> ...resulting in HttpRequestValidationException (when passed in Query String parameter)
        return HttpUtility.UrlEncode(theString);
    }
    /// <summary> This extension method will remove UrlEncoding from the string, and return the plain text </summary>
    public static string AsUrlDecodedX(this string theString)
    {
        return HttpUtility.UrlDecode(theString);
    }

    /// <summary> This Extension Method will check if a string is too long and safely use Substring to cut it to length. Null strings are returned as an empty string. </summary>
    /// <param name="targetText"></param>
    /// <param name="length">What is the Max length allowed? (shorter strings are left unchanged) </param>
    /// <returns></returns>
    public static string CutToLengthX(this string targetText, int length)
    {
        if (targetText == null) { return ""; }

        if (targetText.Length > length)
        {
            targetText = targetText.Substring(0, length);
        }
        return targetText;
    }

    #endregion

    #region DateTime Extensions

    /// <summary> This is shorthand for converting a DateTime to a string of just numbers that represent a datestamp (including milliseconds). 
    /// Convert result to a 'long'...  long stamp = long.Parse(DateTime.UtcNow.ToNumericDatestamp());
    /// </summary>
    public static string ToNumericDatestampX(this DateTime theDate)
    {
        //http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
        return theDate.ToString("yyMMddHHmmssfff"); //!!!!! IT IS CRITICAL that the order of integers is in a sequence that allows timestamp comparrison
        //... this way it can be converted to a C# long and compared against another datestamps for which is earlier
        //      eg... if( long.Parse(DateTime.Now.ToNumericDatestamp()) < long.Parse("130506112304665") ) {}
        /*
         * yy   year
         * MM   month   
         * dd   day
         * HH   hour 24 format
         * mm   minute
         * ss   second
         * fff  millisecond
        */
    }

    public static DateTime ToDateTimeOrX(this object originalObject, DateTime defaultIfNull)
    {
        //Check for null
        if (originalObject == null)
        {
            return defaultIfNull;
        }

        //Check if it's a valid DateTime
        if (originalObject.GetType() == typeof(DateTime))
        {
            return (DateTime)originalObject;
        }

        //Its not a datetime, so try to parse as string
        DateTime result = DateTime.MinValue;
        if (DateTime.TryParse(originalObject.ToString(), out result))
        {
            return result;
        }

        //nothing worked, return default
        return defaultIfNull;
    }

    private static int pacificOffset = (DateTime.UtcNow - DateTime.Now).Hours - 7; // positive hours till UTC, Negative hours back to Pacific.
    /// <summary> This gets the current time in the Pacific time zone </summary>
    public static DateTime PacificX(this DateTime date)
    {
        return date.AddHours(pacificOffset);
    }

    #endregion

    #region Bool Extensions

    /// <summary>
    /// This is a simple conversion from false=>0 and true=>1
    /// </summary>
    public static int AsIntX(this bool originalBool)
    {
        return originalBool ? 1 : 0;
    }

    #endregion

    /// <summary>
    /// This shortcut checks if the string is empty and adds it to the list if it has a value
    /// </summary>
    /// <param name="theList"></param>
    /// <param name="theString"></param>
    public static void AddIfNotEmptyX(this List<string> theList, string theString)
    {
        if (theString.IsntEmptyX())
        {
            theList.Add(theString);
        }
    }

    public static bool ContainsInsensitiveX(this List<string> theList, string theString)
    {
        return theList.FindAll(s => s.Equals(theString, StringComparison.OrdinalIgnoreCase)).Count() > 0;
    }

    //
    // Summary:
    //     Returns the first element of a sequence that satisfies a specified condition
    //     or a default value if no such element is found.
    //
    // Parameters:
    //   source:
    //     An System.Linq.IQueryable<T> to return an element from.
    //
    //   predicate:
    //     A function to test each element for a condition.
    //
    // Type parameters:
    //   TSource:
    //     The type of the elements of source.
    //
    // Returns:
    //     default(TSource) if source is empty or if no element passes the test specified
    //     by predicate; otherwise, the first element in source that passes the test
    //     specified by predicate.
    //
    // Exceptions:
    //   System.ArgumentNullException:
    //     source or predicate is null.
    public static TSource FirstOrNew<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
    {
        var item = source.FirstOrDefault(predicate);

        if (item == null)
        {
            var constructor = source.GetType().GetConstructor(new Type[] { source.ElementType });
            item = (TSource)constructor.Invoke(null);
        }

        return item;

    }

}
