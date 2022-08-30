using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.PagingHelper
{
    public abstract class QueryStringParameters
    {
        //updated abhishek 22aug
        const int maxPageSize = 10000000;

        [Required(ErrorMessage = "Provide PageNumber value.")]
        public int PageNumber { get; set; } = 1;
        public string SearchParam { get; set; }
        private int _pageSize = 10;
        [Required(ErrorMessage = "Provide PageSize value.")]
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string OrderBy { get; set; }
    }

}
