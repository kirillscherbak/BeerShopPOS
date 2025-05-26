using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CryptoPro.Sharpei;

public class EGAISService
{
    private readonly string _certThumbprint;
    private readonly string _certPassword;
    
    public EGAISService(IConfiguration config)
    {
        _certThumbprint = config["EGAIS:CertThumbprint"];
        _certPassword = config["EGAIS:CertPassword"];
    }

    private X509Certificate2 GetCertificate()
    {
        using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
        {
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            
            var certs = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                _certThumbprint,
                false);
            
            if (certs.Count == 0)
                throw new Exception("Сертификат не найден. Убедитесь, что Рутокен подключен.");
            
            return certs[0];
        }
    }

    public async Task<string> RegisterSaleAsync(Receipt receipt)
    {
        var cert = GetCertificate();
        
        // Создаем подпись для запроса
        var content = new StringContent(JsonConvert.SerializeObject(receipt));
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/sales/register")
        {
            Content = content
        };
        
        // Подписываем запрос
        var signedData = SignData(content.ReadAsStringAsync().Result, cert);
        request.Headers.Add("X-Signature", signedData);
        
        // Отправка запроса
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }

    private string SignData(string data, X509Certificate2 cert)
    {
        using (var csp = (Gost3410CryptoServiceProvider)cert.GetRSAPrivateKey())
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signature = csp.SignData(dataBytes, new Gost3411CryptoServiceProvider());
            return Convert.ToBase64String(signature);
        }
    }
}