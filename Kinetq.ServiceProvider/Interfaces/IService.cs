﻿using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Models;
using Kinetq.ServiceProvider.ResultModels;

namespace Kinetq.ServiceProvider.Interfaces
{
    public interface IService<TDto, TId> : IKinetqService
    {
        Task<TDto> CreateAsync(TDto dto);
        Task<IList<TDto>> UpsertAsync(IList<TDto> dtos);
        Task<DeleteResult<TDto>> DeleteAsync(TId id);
        Task<TDto> UpdateAsync(TDto dto);
        Task<List<TDto>> GetAllAsync();
        Task<TDto> GetAsync(TId id);
        Task<ListResult<TDto>> GetAllPagedAsync(Int32 page, Int32 pageSize);
        Task<Int32> GetCountAsync();
        Task<ListResult<TDto>> GetForIdsAsync(TId[] ids, Int32? page = null, Int32? pageSize = null);
        Task<bool> DeleteForIdsAsync(TId[] ids);
        Task<ListResult<TDto>> ExceptIdsAsync(TId[] ids, Int32? page = null, Int32? pageSize = null);
        Task<ListResult<TDto>> GetByFilters(IList<Filter> filters, int? page = null, int? pageSize = null);
        FilterBuilder<TDto, TId> GetByFilters();
    }
}