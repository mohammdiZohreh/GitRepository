using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterIKBCM
{
   
    public static class PersianConverterDate
    {
        public static string ToShamsi(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();

            return pc.GetYear(value) + "/" + pc.GetMonth(value).ToString("00") + "/" + pc.GetDayOfMonth(value).ToString("00");
        }
        public static string YearToShamsi(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(value).ToString();
        }
        public static int DeyNumber(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetDayOfYear(value);
        }
    }
}
