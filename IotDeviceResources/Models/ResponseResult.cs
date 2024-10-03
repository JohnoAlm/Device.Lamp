namespace IotDeviceResources.Models;

public class ResponseResult
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
}

public class ResponseResult<T> : ResponseResult
{
    public T? Content { get; set; }
}
