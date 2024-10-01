namespace Device.Lamp.MVVM.Models;

public class ResponseResult<T>
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; }

}
