namespace FlashcardsApp.BLL.Implementations;


public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public List<string> Errors { get; } = new();

    private ServiceResult() { }

    private ServiceResult(T data)
    {
        IsSuccess = true;
        Data = data;
    }

    private ServiceResult(IEnumerable<string> errors)
    {
        IsSuccess = false;
        Errors.AddRange(errors);
    }

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>(data);
    }

    public static ServiceResult<T> Failure(params string[] errors)
    {
        return new ServiceResult<T>(errors);
    }
}