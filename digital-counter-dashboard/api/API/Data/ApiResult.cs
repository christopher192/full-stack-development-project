namespace API.Data
{
    public class MSSQLCount
    {
        public int Total { get; set; }
    }

    public class ApiResult<T>
    {
        public List<T>? Result { get; set; } = new List<T>();

        public int Total { get; set; }
    }
}
