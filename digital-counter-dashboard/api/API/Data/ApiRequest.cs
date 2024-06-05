namespace API.Data
{
    public class Search
    {
        public int ColumnIndex { get; set; }

        public string? ColumnValue { get; set; }
    }

    public class ApiRequest
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public string SortCol { get; set; } = string.Empty;

        public string SortDir { get; set; } = string.Empty;

        public List<Search>? Searches { get; set; }
    }
}
