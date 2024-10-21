namespace RestaurantBooking.API.Helpers.Pagination
{
    public class PaginationParams
    {
        private const int MaxPageSize = 100;

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => _currentPage = value < _currentPage ? _currentPage : value;
        }

        private int _pageSize = 25;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
