using Framework.Core.Globalization;

namespace Framework.Core.Angular
{
    public class SelectListItem
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Name { get => CultureHelper.IsArabic ? this.NameAr : this.NameEn; }
        public int Value { get; set; }
    }

    public class SelectListItem<T>
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Name { get => CultureHelper.IsArabic ? this.NameAr : this.NameEn; }

        public T Value { get; set; }
    }
}
