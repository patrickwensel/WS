using System.Text;

namespace WS20.Framework
{
    public abstract class WSString
    {
        /// <summary>
        /// Filters a string to make it safe for insert into an Oracle database
        /// </summary>
        /// <param name="theString">The string to be made Oracle-safe</param>
        /// <returns>Returns the Oracle-safe string</returns>
        public static string FilterSQL(string theString)
        {
            return theString.Replace("'", "''");
        }

        /// <summary>
        /// Filters a string to make it safe to post to an HTML page
        /// </summary>
        /// <param name="theString">The string to be made HTML-safe</param>
        /// <returns>Returns the HTML-safe string</returns>
        public static string FilterHTML(string theString)
        {
            string aReturn = theString;
            aReturn = aReturn.Replace("&", "&amp");
            aReturn = aReturn.Replace("<", "&lt");
            aReturn = aReturn.Replace(">", "&gt");

            return aReturn;
        }

        /// <summary>
        /// Strips non-numeric characters from the string
        /// </summary>
        /// <param name="theInput">The string to be edited</param>
        /// <returns>The resulting numeric string</returns>
        public static string StripNonNumeric(string theInput)
        {
            StringBuilder aResult = new StringBuilder();
            theInput = theInput.Trim();
            for (int i = 0; i < theInput.Length; i++)
            {
                if (IsNumeric(theInput[i]))
                    aResult.Append(theInput[i]);
            }

            return aResult.ToString();
        }

        /// <summary>
        /// Tests whether a character is numeric
        /// </summary>
        /// <param name="theChar">The character to be tested</param>
        /// <returns>Returns true if numeric, else false</returns>
        public static bool IsNumeric(char theChar)
        {
            if (theChar >= '0' && theChar <= '9')
                return true;

            return false;
        }

    }
}
