namespace HomelessAnimals.Shared.Models
{
    public class PagedResult<T>
    {
        public PagedResult()
        {

        }

        public PagedResult(IEnumerable<T> items, int count, int pageSize)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        public int TotalPages { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
