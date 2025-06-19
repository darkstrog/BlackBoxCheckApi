namespace BlackBoxCheckApi.Services
{
    public interface IQRService
    {
        string GetBase64PngQRCode(string text);
    }
}
