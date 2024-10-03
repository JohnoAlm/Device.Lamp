using IotDeviceResources.Models;

namespace IotDeviceResources.Factories;

public static class ResponseResultFactory
{
    public static ResponseResult Succeeded(string? message)
    {
        return new ResponseResult
        {
            Succeeded = true,
            Message = message
        };
    }

    public static ResponseResult Failed(string? message)
    {
        return new ResponseResult
        {
            Succeeded = false,
            Message = message
        };
    }
}

public static class ResponseResultFactory<T>
{
    public static ResponseResult<T> Succeeded(string? message, T? content)
    {
        return new ResponseResult<T>
        {
            Succeeded = true,
            Message = message,
            Content = content
        };
    }

    public static ResponseResult<T> Failed(string? message, T? content)
    {
        return new ResponseResult<T>
        {
            Succeeded = false,
            Message = message,
            Content = content
        };
    }
}
