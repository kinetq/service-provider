namespace Kinetq.ServiceProvider.Interfaces
{
    public interface IEntityWithTypedId<TId>
    {
        /// <summary>
        ///     Gets the ID which uniquely identifies the entity instance within its type's bounds.
        /// </summary>
        TId Id { get; set; }
    }
}