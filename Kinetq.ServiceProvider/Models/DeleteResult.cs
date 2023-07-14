namespace Kinetq.ServiceProvider.Models;

public class DeleteResult<T>
{
    public T? DeletedEntity { get; set; }
    public bool Result { get; set; }
}