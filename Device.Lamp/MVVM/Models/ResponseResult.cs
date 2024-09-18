namespace Device.Lamp.MVVM.Models;

public class ResponseResult<T>
{
    public bool Succeeded { get; set; }
    public T? Result { get; set; }
    public string? ErrorMessage { get; set; }

}
