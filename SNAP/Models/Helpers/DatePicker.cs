using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SNAP.Models.Helpers
{
    public class DatePicker
    {
        public static MvcHtmlString Label(string modelName)
        {
            List<string> statements = new List<string>();

            statements.Add(String.Format("<label class='input-group-btn' for='{0}'>", modelName));
            statements.Add("<span class='btn btn-default'>");
            statements.Add("<span class='glyphicon glyphicon-calendar'>");
            statements.Add("</span></span></label>");

            return MvcHtmlString.Create(String.Join("\n", statements));
        }

        internal static List<string> GetTimeRange(int startHour, int count, bool halfHours)
        {
            List<string> timeRange = new List<string>();

            foreach (int item in Enumerable.Range(startHour, count))
            {
                timeRange.Add(DateTime.MinValue.AddHours(item).ToString("hh:mm tt"));

                if (halfHours == true)
                {
                    timeRange.Add(DateTime.MinValue.AddHours(item).AddMinutes(30).ToString("hh:mm tt"));
                }
            }

            return timeRange;
        }
    }
}