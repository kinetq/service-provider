namespace Kinetq.ServiceProvider.Models;

public class ServiceResult
{
    public Func<Task> OperationFunc { get; set; }
    public bool HasRun { get; set; }
    public Task Value { get; set; }
    public Type ReturnType { get; set; }

    public void Unwrap()
    {
        Value = OperationFunc();
        HasRun = true;
    }
}