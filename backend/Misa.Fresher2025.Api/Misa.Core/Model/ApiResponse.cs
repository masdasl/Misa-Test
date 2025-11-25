namespace Misa.Core.Model
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public Pagination? Meta { get; set; }
        public string Error { get; set; }

        public class Pagination
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int Total { get; set; }
        }
    }
}
