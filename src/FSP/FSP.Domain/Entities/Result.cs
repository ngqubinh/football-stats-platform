namespace FSP.Domain.Entities;

public class Result<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static Result<T> Ok(T data) => new Result<T> { Success = true, Data = data };
    public static Result<T> OkString(string message) => new Result<T> { Success = true, Message = message};
    public static Result<T> Fail(string message) => new Result<T> { Success = false, Message = message };
    
}