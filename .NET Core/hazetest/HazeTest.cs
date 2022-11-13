using System.Net;
using System.Text;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace hazetest;

public class HazeTest : IAsyncLifetime
{
    private readonly ITestOutputHelper output;
    private string adminToken;
    private string userToken;
    private HttpClient adminClient;
    private HttpClient userClient;
    private string apiHost = @"https://localhost:7105";
    private string userName = "unittest";
    private string adminName = "unit.test@cvgs.org";
    private string password = "asdfasdf1";

    public HazeTest(ITestOutputHelper output)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        this.adminClient = new HttpClient(clientHandler);
        this.userClient = new HttpClient(clientHandler);
        this.adminClient.BaseAddress = new Uri(apiHost);
        this.userClient.BaseAddress = new Uri(apiHost);
        this.output = output;
    }
    
    public async Task InitializeAsync()
    {
        string adminPostBody = $"{{\"username\": \"{adminName}\",  \"password\": \"{password}\"}}";
        string userPostBody = $"{{\"username\": \"{userName}\",  \"password\": \"{password}\"}}";
        var adminContent = new StringContent(adminPostBody, Encoding.UTF8, "application/json");
        var userContent = new StringContent(userPostBody, Encoding.UTF8, "application/json");
        var adminLoginRes = await adminClient.PostAsync("/login", adminContent);
        var userLoginRes = await userClient.PostAsync("/login", userContent);
        dynamic? adminJson = JsonConvert.DeserializeObject<dynamic>(await adminLoginRes.Content.ReadAsStringAsync());
        dynamic? userJson = JsonConvert.DeserializeObject<dynamic>(await userLoginRes.Content.ReadAsStringAsync());
        adminToken = adminJson.token;
        userToken = userJson.token;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    
    [Fact]
    public async void LoginUser_UserExists_Login_Ok()
    {
        try
        {
            string userPostBody = $"{{\"username\": \"{userName}\",  \"password\": \"{password}\"}}";
            var userContent = new StringContent(userPostBody, Encoding.UTF8, "application/json");
            var userLoginRes = await userClient.PostAsync("/login", userContent);
            output.WriteLine("Response: " + await userLoginRes.Content.ReadAsStringAsync());
            Assert.True(userLoginRes.StatusCode == HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Assert.True(false, e.Message);
        }
    }
    
    [Fact]
    public async void LoginUser_UserExists_Login_Ok()
    {
        try
        {
            string userPostBody = $"{{\"username\": \"{userName}\",  \"password\": \"{password}\"}}";
            var userContent = new StringContent(userPostBody, Encoding.UTF8, "application/json");
            var userLoginRes = await userClient.PostAsync("/login", userContent);
            output.WriteLine("Response: " + await userLoginRes.Content.ReadAsStringAsync());
            Assert.True(userLoginRes.StatusCode == HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Assert.True(false, e.Message);
        }
    }
}