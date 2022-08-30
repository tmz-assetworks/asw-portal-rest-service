using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Helper
{
    public static class CommonHelpers
    {
        /// <summary>
        /// This function returns hours with two digits like 01,02,03,04 format
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static string GetHoursTwoDigitFormat(int hours)
        {
            if (hours < 10) return "0" + hours;
            else return  hours.ToString();
        }
        public static Dictionary<int, string> LocationStaticColorList()
        {
            Dictionary<int, string> locationcolor = new Dictionary<int, string>();
            locationcolor.Add(1, "#27b406");
            locationcolor.Add(2, "#a60000");
            locationcolor.Add(3, "#fb5858");
            locationcolor.Add(4, "#ffa12d");
            locationcolor.Add(5, "#e97300");
            return locationcolor;       
        }

        public static string PagenationValidation(QueryStringParameters queryStringParameters)
        {
            string message="";
            if (queryStringParameters.PageNumber != null && queryStringParameters.PageNumber < 0)
            {
                if (queryStringParameters.PageNumber < 0)
                {
                    message = "Please check Pagination parameter!";
                }

            }
            if(queryStringParameters.PageSize != null && queryStringParameters.PageSize < 0)
            {
                if(queryStringParameters.PageSize < 0)
                {
                    message = "Please check Pagination parameter!";
                }
            }
            return message;
        }
    }
}
