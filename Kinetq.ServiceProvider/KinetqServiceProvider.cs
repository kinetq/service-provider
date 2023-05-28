﻿using System.Reflection;
using AutoMapper;
using Kinetq.ServiceProvider.Attributes;
using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Helpers;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.ResultModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kinetq.ServiceProvider
{
    public abstract class KinetqServiceProvider<TDto, TEntity, TId> : IService<TDto, TId>
        where TDto : class
        where TEntity : class, IEntityWithTypedId<TId>, new()
    {
        private readonly ISessionManager _sessionManager;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMapper _mapper;

        protected KinetqServiceProvider(
            ISessionManager sessionManager,
            ILoggerFactory loggerFactory,
            IMapper mapper)
        {
            _sessionManager = sessionManager;
            _loggerFactory = loggerFactory;
            _mapper = mapper;
        }

        private IList<Func<IQueryable<TEntity>, Filter, IQueryable<TEntity>>> FilterProviders
        {
            get
            {
                var @type = GetType();
                var assembly = Assembly.GetAssembly(@type);
                var filters = assembly.ExportedTypes.Where(x =>
                {
                    var attribute = x.GetCustomAttribute<ServiceProviderAttribute>();
                    if (attribute == null) return false;

                    return attribute.EntityType == typeof(TEntity);
                });

                var methods =
                    filters.SelectMany(x =>
                        x.GetMethodsBySig(typeof(IQueryable<TEntity>), typeof(IQueryable<TEntity>), typeof(Filter)));

                return methods.Select(x => (Func<IQueryable<TEntity>, Filter, IQueryable<TEntity>>)x.CreateDelegate(typeof(Func<IQueryable<TEntity>, Filter, IQueryable<TEntity>>))).ToList();
            }
        }

        private Func<IQueryable<TEntity>, IList<Filter>, KinetqContext, IEnumerable<TEntity>> ProjectionProvider
        {
            get
            {
                var @type = GetType();
                var assembly = Assembly.GetAssembly(@type);
                var filters = assembly.ExportedTypes.Where(x =>
                {
                    var attribute = x.GetCustomAttribute<ServiceProviderAttribute>();
                    if (attribute == null) return false;

                    return attribute.EntityType == typeof(TEntity);
                });

                var methods =
                    filters.SelectMany(x =>
                        x.GetMethodsBySig(typeof(IEnumerable<TEntity>), typeof(IQueryable<TEntity>), typeof(IList<Filter>), typeof(KinetqContext)));

                return methods.Select(x => (Func<IQueryable<TEntity>, IList<Filter>, KinetqContext, IEnumerable<TEntity>>)x.CreateDelegate(typeof(Func<IQueryable<TEntity>, IList<Filter>, KinetqContext, IEnumerable<TEntity>>))).SingleOrDefault();
            }
        }        
        
        private Func<IQueryable<TEntity>, IQueryable<TEntity>> IncludeProvider
        {
            get
            {
                var @type = GetType();
                var assembly = Assembly.GetAssembly(@type);
                var filters = assembly.ExportedTypes.Where(x =>
                {
                    var attribute = x.GetCustomAttribute<ServiceProviderAttribute>();
                    if (attribute == null) return false;

                    return attribute.EntityType == typeof(TEntity);
                });

                var methods =
                    filters.SelectMany(x =>
                        x.GetMethodsBySig(typeof(IQueryable<TEntity>), typeof(IQueryable<TEntity>)));

                return methods.Select(x => (Func<IQueryable<TEntity>, IQueryable<TEntity>>)x.CreateDelegate(typeof(Func<IQueryable<TEntity>, IQueryable<TEntity>>))).SingleOrDefault();
            }
        }

        protected abstract string SessionKey { get; }
        protected KinetqContext Session => _sessionManager.GetSessionFrom(SessionKey).Result; 
        protected ILogger Logger => CreateLogger();
        protected virtual ILogger CreateLogger()
        {
            return _loggerFactory.CreateLogger<KinetqServiceProvider<TDto, TEntity, TId>>();
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            try
            {
                TEntity entity = _mapper.Map<TDto, TEntity>(dto, opt => opt.Items["SessionKey"] = SessionKey);

                await Session.Set<TEntity>().AddAsync(entity);

                if (await Session.SaveChangesAsync() > -1)
                {
                    return _mapper.Map<TEntity, TDto>(await GetEntityAsync(entity.Id), opt => opt.Items["SessionKey"] = SessionKey);
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Could not create dto type {0}. Error: {1}", typeof(TEntity).Name, ex);
                return null;
            }
        }

        public async Task<IList<TDto>> UpsertAsync(IList<TDto> dtos)
        {
            IList<TDto> result = new List<TDto>();
            foreach (var dto in dtos)
            {
                TId id = (TId)typeof(TDto).GetProperty("Id").GetValue(dto);

                if (id.EqualsDefaultValue())
                {
                    result.Add(await CreateAsync(dto));
                }
                else
                {
                    result.Add(await UpdateAsync(dto));
                }
            }

            return result;
        }

        public virtual async Task<bool> DeleteAsync(TId id)
        {
            try
            {
                var entity = await Session.Set<TEntity>().FindAsync(id);
                Session.Set<TEntity>().Remove(entity);

                return await Session.SaveChangesAsync() > -1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Could not delete dto");
                return false;
            }
        }

        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            try
            {
                TId id = (TId)typeof(TDto).GetProperty("Id").GetValue(dto);
                TEntity entity = await GetEntityAsync(id);

                _mapper.Map(dto, entity, opt => opt.Items["SessionKey"] = SessionKey);

                Session.Set<TEntity>().Update(entity);
                if (await Session.SaveChangesAsync() > -1)
                {
                    return _mapper.Map<TEntity, TDto>(await GetEntityAsync(entity.Id), opt => opt.Items["SessionKey"] = SessionKey);
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Could not update dto {typeof(TEntity).Name}");
                return null;
            }
        }

        public virtual async Task<List<TDto>> GetAllAsync()
        {
            try
            {
                IQueryable<TEntity> entities = Session.Set<TEntity>();

                entities = IncludeProvider(entities);

                return _mapper.Map<List<TEntity>, List<TDto>>(await entities.ToListAsync(), opt => opt.Items["SessionKey"] = SessionKey);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Could not get all for {typeof(TEntity).Name}");
                return new List<TDto>();
            }
        }

        public virtual async Task<TDto> GetAsync(TId id)
        {
            try
            {
                return _mapper.Map<TEntity, TDto>(await GetEntityAsync(id), opt => opt.Items["SessionKey"] = SessionKey);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Could not find item for id {id}");
                return null;
            }
        }

        private async Task<TEntity> GetEntityAsync(TId id)
        {
            IQueryable<TEntity> entities = Session.Set<TEntity>();
            entities = IncludeProvider(entities);

            var entity = await entities.FirstAsync(x => x.Id.Equals(id));
            return entity;
        }

        public virtual async Task<ListResult<TDto>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = Session.Set<TEntity>()
                .AsQueryable()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            int count = await Session.Set<TEntity>().AsQueryable().CountAsync();

            return new ListResult<TDto>
            {
                Entities = _mapper.Map<List<TEntity>, List<TDto>>(query.ToList(), opt => opt.Items["SessionKey"] = SessionKey),
                Count = count
            };
        }

        public virtual async Task<int> GetCountAsync()
        {
            return await Session.Set<TEntity>().AsQueryable().CountAsync();
        }

        public virtual async Task<ListResult<TDto>> GetForIdsAsync(TId[] ids, int? page = null, int? pageSize = null)
        {
            var query = Session.Set<TEntity>().AsQueryable().Where(x => ids.Contains(x.Id));

            int count = await query.CountAsync();
            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return new ListResult<TDto>
            {
                Entities = _mapper.Map<List<TEntity>, List<TDto>>(query.ToList(), opt => opt.Items["SessionKey"] = SessionKey),
                Count = count
            };
        }

        public virtual async Task<bool> DeleteForIdsAsync(TId[] ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    bool deleted = await DeleteAsync(id);
                    if (!deleted)
                    {
                        throw new Exception($"Could not delete dto for id {id}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return false;
            }

            return true;
        }

        public virtual async Task<ListResult<TDto>> ExceptIdsAsync(TId[] ids, int? page = null, int? pageSize = null)
        {
            var query = Session.Set<TEntity>().AsQueryable().Where(x => !ids.Contains(x.Id));
            int count = await query.CountAsync();

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return new ListResult<TDto>
            {
                Entities = _mapper.Map<List<TEntity>, List<TDto>>(query.ToList(), opt => opt.Items["SessionKey"] = SessionKey),
                Count = count
            };
        }

        public virtual async Task<ListResult<TDto>> GetByFilters(IList<Filter> filters, int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = Session.Set<TEntity>();

            foreach (var filter in filters)
            {
                var provider =
                    FilterProviders.FirstOrDefault(x => x.Method.Name.Equals(filter.Name));

                if (provider != null)
                {
                    query = provider(query, filter);
                }
            }

            query = IncludeProvider != null ? IncludeProvider(query) : query;

            try
            {
                int count = await query.CountAsync();
                if (page.HasValue && pageSize.HasValue)
                {
                    query = query.Skip((page.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value);
                }

                return new ListResult<TDto>
                {
                    Entities = _mapper.Map<List<TEntity>, List<TDto>>(ProjectionProvider == null ? query.ToList() : ProjectionProvider(query, filters, Session).ToList(),
                        opt => opt.Items["SessionKey"] = SessionKey),
                    Count = count
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Could not apply filters for {typeof(TEntity).Name}");
                return new ListResult<TDto>
                {
                    Entities = new List<TDto>(),
                    Count = 0
                };
            }
        }

        public virtual FilterBuilder<TDto, TId> GetByFilters()
        {
            return new FilterBuilder<TDto, TId>(this);
        }
    }
}