using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.ResultModels;

namespace Kinetq.ServiceProvider.Builders
{
    public class FilterBuilder<TDto, TId>
    {
        private readonly IService<TDto, TId> _service;
        private readonly List<Filter> _filters = new List<Filter>();
        private int? _page = null;
        private int? _pageSize = null;

        public FilterBuilder(IService<TDto, TId> service)
        {
            _service = service;
        }

        public FilterBuilder<TDto, TId> Where(string name, dynamic value)
        {
            _filters.Add(new Filter { Name = name, Value = value });
            return this;
        }

        public FilterBuilder<TDto, TId> Where(params Filter[] filters)
        {
            _filters.AddRange(filters);
            return this;
        }

        public FilterBuilder<TDto, TId> Paged(int page, int pageSize)
        {
            _page = page;
            _pageSize = pageSize;
            return this;
        }

        public async Task<ListResult<TDto>> ListResult()
        {
            return await _service.GetByFilters(_filters, _page, _pageSize);
        }
        
        public async Task<IList<TDto>> Entities()
        {
            var result = await _service.GetByFilters(_filters, _page, _pageSize);
            return result.Entities;
        }

        public async Task<TDto> FirstOrDefault()
        {
            var result = await _service.GetByFilters(_filters, _page, _pageSize);
            return result.Entities.FirstOrDefault();
        }

        public async Task<bool> Any()
        {
            var result = await _service.GetByFilters(_filters, _page, _pageSize);
            return result != null && result.Entities.Any();
        }
    }
}