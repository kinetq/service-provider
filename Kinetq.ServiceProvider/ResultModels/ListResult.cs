namespace Kinetq.ServiceProvider.ResultModels
{
    public class ListResult<T>
    {
        public IList<T> Entities { get; set; }
        public int Count { get; set; }
    }
}