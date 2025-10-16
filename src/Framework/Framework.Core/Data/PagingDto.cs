using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace Framework.Core.Data
{
    public abstract class PagingDto
    {
        public StaticPagedList<object>? Items { get; set; }

        [HiddenInput]
        public int PageNumber { get; set; } = 1;

        public int? PageSize { get; set; } = 20;

        public int? TotalItemsCount { get; set; }

        public bool? IsExport { get; set; } = false;
        [HiddenInput]
        public bool? IsDescending { get; set; } = true;
        [HiddenInput]
        public bool? IsSearchOpen { get; set; } = false;
        [HiddenInput]
        public string? ReturnUrl { get; set; }

        [HiddenInput]
        public bool? IsActive { get; set; }

    }
}
