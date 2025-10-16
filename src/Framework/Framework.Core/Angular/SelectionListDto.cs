using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Angular
{
    public class SelectionListDto : SelectionListDto<Guid, string> { }

    public class SelectionListDto<T, TN>
    {
        public T Id { get; set; }
        public TN Name { get; set; }
    }

    public class SelectionListWithInfoDto : SelectionListWithInfoDto<Guid, string> { }
    public class SelectionListWithInfoDto<T, TN>
    {
        public T Id { get; set; }
        public TN Name { get; set; }
        public object info { get; set; }
    }

    public class SelectionListEnAndArDto : SelectionListEnAndArDto<Guid, string,string> { }

    public class SelectionListEnAndArDto<T, TNAr, TNEn>
    {
        public T Id { get; set; }
        public TNAr NameAr { get; set; }
        public TNEn NameEn { get; set; }
    }


    public class SelectionListAllNameDto : SelectionListAllNameDto<Guid, string, string,string> { }

    public class SelectionListAllNameDto<T, TN, TNAr, TNEn>
    {
        public T Id { get; set; }
        public TN Name { get; set; }
        public TNAr NameAr { get; set; }
        public TNEn NameEn { get; set; }
    }
}
