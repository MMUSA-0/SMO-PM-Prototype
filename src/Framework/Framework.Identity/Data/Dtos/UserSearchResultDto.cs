using Framework.Core.Data;
using PagedList.Core;
using System.Collections.Generic;

namespace Framework.Identity.Data.Dtos
{
    public class UserSearchResultDto : PagingDto
    {        
        public new StaticPagedList<UsersListDto> Items { get; set; }
        public List<UsersListExcelDto> ExportedItems { get; set; }
    }
}