﻿namespace KoboRack.Utility.Pagination
{
    public class PagePagination<T>
    {
        public static async Task<PageResult<IEnumerable<T>>> GetPager(IEnumerable<T> data,
              int PerPage, int Page, Func<T, string> nameSelector, Func<T, string> idSelector)
        {
            PerPage = PerPage <= 0 ? 10 : PerPage;
            Page = Page <= 0 ? 1 : Page;

            data = data.OrderBy(item => nameSelector(item)).ThenBy(item => idSelector(item));
            int totalData = data.Count();
            int totalPagedCount = (int)Math.Ceiling((double)totalData / PerPage);
            var pagedData = data.Skip((Page - 1) * PerPage).Take(PerPage);
            await Task.Delay(1);
            return new PageResult<IEnumerable<T>>
            {
                Data = pagedData,
                TotalPageCount = totalPagedCount,
                CurrentPage = Page,
                PerPage = PerPage,
                TotalCount = totalData
            };
        }
    }
}
