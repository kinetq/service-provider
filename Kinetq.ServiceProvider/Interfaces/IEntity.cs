namespace Kinetq.EntityFrameworkService.Interfaces
{
    public interface IEntity<TId> : IEntityWithTypedId<TId>
    {
        /// <summary>
        /// Every entity should have a name that uniquely identifies it
        /// </summary>
        string Name { get; set; }
    }
}