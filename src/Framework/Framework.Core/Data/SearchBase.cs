namespace Framework.Core.Data
{
    public abstract class SearchBase<T>
    {
        public List<T>? Items { get; set; }
        public int PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int TotalItemsCount { get; set; }
        public bool IsExport { get; set; }
        public OrdeyByEnum OrderBy { get; set; } = OrdeyByEnum.ASC; //1:ASC  2:DESC
        public string? OrderByColumn { get; set; }
        public ExportType ExportTypeId { get; set; }
    }
}
