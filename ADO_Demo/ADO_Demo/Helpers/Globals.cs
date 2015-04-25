using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Helpers
{
    public static class Globals
    {
        //public const string gNsSupportEmail = "support@NotarySoftware.com";
        //public const string gNcSupportEmail = "support@NotaryCafe.com";
        private static string _gNsSupportEmail = ""; // empty so property calculates
        public static string gNsSupportEmail
        {
            get
            {
                if (_gNsSupportEmail.IsEmptyX())
                {
                    if (ConfigurationManager.AppSettings["nsDefaultSupportEmail"] != null)
                    {
                        _gNsSupportEmail = Convert.ToString(ConfigurationManager.AppSettings["nsDefaultSupportEmail"]);
                    }

                    if (_gNsSupportEmail.IsEmptyX())
                    {
                        _gNsSupportEmail = "support@NotarySoftware.com";
                    }
                }

                return _gNsSupportEmail;
            }
        }
        private static string _gNcSupportEmail = ""; // empty so property calculates
        public static string gNcSupportEmail
        {
            get
            {
                if (_gNcSupportEmail.IsEmptyX())
                {
                    if (ConfigurationManager.AppSettings["ncDefaultSupportEmail"] != null)
                    {
                        _gNcSupportEmail = Convert.ToString(ConfigurationManager.AppSettings["ncDefaultSupportEmail"]);
                    }

                    if (_gNcSupportEmail.IsEmptyX())
                    {
                        _gNcSupportEmail = "support@NotaryCafe.com";
                    }
                }

                return _gNcSupportEmail;
            }
        }

        private static DateTime _minDbValue = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //   01/01/1753 
        public static DateTime MinSqlDate { get { return _minDbValue; } } //readonly


        // according to this site, the DAtaAnnotations Regex for email is this... good luck        private static Regex _regex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);          https://github.com/mono/aspnetwebstack/blob/master/src/Microsoft.Web.Mvc/EmailAddressAttribute.cs
        // Wikipedia claims a wide variety of valid emails... http://en.wikipedia.org/wiki/Email_address#Valid_email_addresses
        // This is a bit more restrictive than I want to support...     public const string gRegexOfEmail = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){1,3})+)$"; // from http://geekswithblogs.net/MainaD/archive/2007/12/03/117321.aspx
        //
        // !!!!!!!!!! see the use of the nsExtensionMethods.IsEmail() extension method.
        // We will look for a leading letter, and @ sign, and a period. Though technically more is valid than this.     
        public const string gRegexOfEmail = @"^\w\S*@\S+\.\S+$"; // \w starts with a word character ... \S* is any non-whitespace character(0 or more) ... the '@' ... \S+ 1 or more non-whitespace ... a period .... \S+ 1 or more non-whitespace
        public const string gRegexOfEmailMessage = "The Email Address is not recognized. valid sample: 'your.name1@domain.com'.  Recognized Email addresses must start with a letter and contain only 1 '@' follow by domain, period and TLD (eg.'com'). There should be no spaces.";

        // This regex should match US Postal Codes short or long form with this. (^\d{5}(-\d{4})?$) 
        // It should also match any Canadian postal codes with or without spaces with this. (^[ABCEGHJKLMNPRSTVXY]{1}\d{1}[A-Z]{1} *\d{1}[A-Z]{1}\d{1}$)
        public const string gRegexOfZip = @"(^\d{5}(-\d{4})?$)|(^[ABCEGHJKLMNPRSTVXYabceghjklmnprstvxy]{1}\d{1}[a-zA-Z]{1}\s?\d{1}[a-zA-Z]{1}\d{1}$)"; // from http://geekswithblogs.net/MainaD/archive/2007/12/03/117321.aspx
        public const string gRegexOfZipMessage = "Zip must be in US or Canadian format: '12345', '12345-1234', 'T2X 1V4'";

        public const string gRegexOfPhoneNumber = @"^([0-9]( |-)?)?(\(?[0-9]{3}\)?|[0-9]{3})( |-)?([0-9]{3}( |-)?[0-9]{4}|[a-zA-Z0-9]{7})$"; // from http://www.regexlib.com/Search.aspx?k=phone
        public const string gRegexOfPhoneNumberMessage = "Phone number must be 10 digits";
        //          ^
        //          ([0-9]( |-)?)?
        //          (
        //              \(?[0-9]{3}\)?
        //              |
        //              [0-9]{3}
        //          )
        //          ( |-)?
        //          (
        //              [0-9]{3}( |-)?
        //              [0-9]{4}
        //              |
        //              [a-zA-Z0-9]{7}
        //          )
        //          $

        //NOTE! update         public static string CleanNoHtml(string original)     when changing gRegexOfHtml
        public const string gRegexOfHtml = @"^[^<>\{\}]*$"; // Matches an entire string where no character matches the inner 'NOT' clause [^notThese] ... eg not <, not >, not {, not }
        public const string gRegexOfHtmlMessage = @"'{0}' may not contain these web code characters: > <  {{ }} "; //data annotations cannot have curly braces, unless escaped by doubling them. otherwise they are used like string.format to show value.

        /// <summary> This regex identifies a NS V2 Public Id (8 characters of numbers and letters</summary>
        public const string gRegexOfPublicId = @"(?=.*?[a-zA-Z])(?=.*?[\d]){8}"; //@"^[\w\d]{8}$"; // 8 characters or digits  eg. 5CAC714D,  must contain both

        public const string gGuidRequiredMessage = "A system Id is required. This could be caused by a system glitch. Please contact support.";
        public static string gMinSqlDateMessage { get { return "The Date field must be more recent than " + MinSqlDate.ToShortDateString(); } } // needs to be a property to contain calculated text

        public const string gOwnerEditableMessage = "Only the owner of this data is permitted to edit it.";

        public const string gExpiredSessionMessage = "Due to inactivity, you have been logged out. Please login again."; // used in nsCatchAnonymousOrExpiredSessionAttribute and overridable as         [nsCatchAnonymousOrExpiredSession(ExpiredMessage = "My custom message.")] 
        public const string gAnonymousMessage = "Please login to view this page."; // used in nsCatchAnonymousOrExpiredSessionAttribute and overridable as         [nsCatchAnonymousOrExpiredSession(AnonymousMessage = "My custom message.")] 

        public const string GoogleMapApiKey = "AIzaSyBYgdlSrxi9T_e-GyxLRBmoFCmK5LOk_BE"; //  Google Map Javascript Library (for adding points, etc) Api Key created by DavidB@Exects.com  

        /// <summary> The external link to the sister Forum site </summary>
        public const string ForumUrl = "http://Forum.NotaryCafe.com";
        /// <summary> This is the Rijndael key/IV for encrypting the Single Sign On link to the forums </summary>

        public const string PaymentUrl = "/Account/Manage?initial=membershippayment";

    }
}