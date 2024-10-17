namespace RestaurantBooking.API.Models.Pagination
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageCount { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        
        public PagedList(List<T> items, int totalCount, int currentPage, int pageCount, int pageSize)
        {
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageCount = pageCount;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            AddRange(items);
        }
        
        public static PagedList<T> ToPagedList(ICollection<T> source, int currentPage, int pageSize)
        {
            var totalCount = source.Count;
            var items = source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, totalCount, currentPage, items.Count, pageSize);
        }
    }
}
