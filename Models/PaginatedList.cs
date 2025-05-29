namespace NYC.MobileApp.Model;

public class PaginatedList<T>
{
    public int CurrentPage { get; }
    public int PageSize { get; }
    public int PageCount { get; }
    public List<T> Items { get; }

    public PaginatedList(int currentPage, int pageSize, int pageCount, List<T> items)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        PageCount = (int)Math.Ceiling(pageCount / (double)pageSize);
        Items = items;
    }
}