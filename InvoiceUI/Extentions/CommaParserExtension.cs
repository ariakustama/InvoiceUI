using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceUI.Extentions
{
    public static class CommaParserExtension
    {

        public static string ToComma(this decimal inputNumber)
        {
            try
            {
                return inputNumber.ToString("N", CultureInfo.CreateSpecificCulture("en-US"));
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static string ToComma(this int inputNumber)
        {
            try
            {
                return inputNumber.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
