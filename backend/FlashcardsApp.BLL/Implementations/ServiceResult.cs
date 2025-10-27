namespace FlashcardsApp.BLL.Implementations;


public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public List<string> Errors { get; } = new();

    // 1. Приватный конструктор без параметров
    private ServiceResult() { }

    // 2. Приватный конструктор для успешного результата
    private ServiceResult(T data)
    {
        IsSuccess = true;
        Data = data;
    }

    // 3. Приватный конструктор для неуспешного результата
    private ServiceResult(IEnumerable<string> errors)
    {
        IsSuccess = false;
        Errors.AddRange(errors);
    }

    // Статический фабричный метод для успеха
    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>(data);
    }

    // Статический фабричный метод для ошибки
    public static ServiceResult<T> Failure(params string[] errors)
    {
        return new ServiceResult<T>(errors);
    }
}