namespace SyriaNews.Abstractions;

public class PaginatedList<T>
{
    public PaginatedList(List<T> items, int pageNumber, int count, int totalPages, int pageSize)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Count = count;
        TotalPages = totalPages;
    }

    public List<T> Items { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public int Count { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync
        (IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, pageNumber, count, totalPages, pageSize);
    }
    
    public static PaginatedList<T> Create
        (List<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count;
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, pageNumber, count, totalPages, pageSize);
    }
    
}
