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
    public async void PostLoginUser_UserExistsLogin_Ok()
    {
        string endpoint = "/Login";
        string requestBody = $"{{\"username\": \"{userName}\",  \"password\": \"{password}\"}}";
        await RunHazeTest(endpoint, WebRequestMethods.Http.Post, true, HttpStatusCode.OK, requestBody);
    }
    
    [Fact]
    public async void PutProfile_ValidRequest_Ok()
    {
        string endpoint = "/UserProfile";
        string requestBody = "{    \"email\": \"\",    \"password\": \"\",    \"roleName\": \"\",    \"userName\": \"\",    \"firstName\": \"unit\",    \"lastName\": \"test\",    \"gender\": \"Prefer not to say\",    \"birthDate\": \"2022-01-01T00:00:00\",    \"newsletter\": true  }";
        await RunHazeTest(endpoint, WebRequestMethods.Http.Put, true, HttpStatusCode.OK, requestBody);
    }
    
    [Fact]
    public async void PatchPreferenceSetting_ValidRequest_Ok()
    {
        string endpoint = "/UserPreferences";
        string requestBody = "{    \"platformIds\": [        1    ],    \"categoryIds\": [        1,        2    ]}";
        await RunHazeTest(endpoint, "PATCH", true, HttpStatusCode.OK, requestBody);
    }
    
    [Fact]
    public async void GetMemberAddresses_ValidRequest_Ok()
    {
        string endpoint = "/Addresses";
        await RunHazeTest(endpoint, WebRequestMethods.Http.Get, true, HttpStatusCode.OK);
    }
    
    [Fact]
    public async void GetMemberPaymentInfos_ValidRequest_Ok()
    {
        string endpoint = "/PaymentInfo";
        await RunHazeTest(endpoint, WebRequestMethods.Http.Get, true, HttpStatusCode.OK);
    }

    [Fact]
    public async void PostLoginAdmin_AdminExistsLogin_Ok()
    {
        string endpoint = "/Login";
        string requestBody = $"{{\"username\": \"{adminName}\",  \"password\": \"{password}\"}}";
        await RunHazeTest(endpoint, WebRequestMethods.Http.Post, true, HttpStatusCode.OK, requestBody, true);
    }
    
    [Fact]
    public async void GetEvent_ValidRequest_Ok()
    {
        string endpoint = "/Events";
        await RunHazeTest(endpoint, WebRequestMethods.Http.Get, true, HttpStatusCode.OK, "", true);
    }
    
    [Fact]
    public async void GetWishlistReport_ValidRequest_Ok()
    {
        string endpoint = "/Reports/Wishlist";
        await RunHazeTest(endpoint, WebRequestMethods.Http.Get, true, HttpStatusCode.OK, "", true);
    }
    
    [Fact]
    public async void GetUserWishlist_ValidRequest_Ok()
    {
        string endpoint = "/Wishlist";
        await RunHazeTest(endpoint, WebRequestMethods.Http.Get, true, HttpStatusCode.OK);
    }
    
    private async Task RunHazeTest(string endpoint, string method, bool useToken, HttpStatusCode expectedStatusCode, string requestBody = "", bool isAdmin = false)
    {
        try
        {
            HttpResponseMessage response = null;
            StringContent? content = null;
            HttpClient client = null;
            if (useToken && !isAdmin)
                UseUserToken();
            else if (useToken && isAdmin)
                UseAdminToken();
            output.WriteLine(method + " " + endpoint);
            if (!string.IsNullOrEmpty(requestBody))
            {
                content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                output.WriteLine("Request Body: " + requestBody);
            }

            client = isAdmin ? adminClient : userClient;
            
            switch (method)
            {
                case WebRequestMethods.Http.Get:
                    response = await client.GetAsync(endpoint);
                    break;
                case WebRequestMethods.Http.Post:
                    response = await client.PostAsync(endpoint, content);
                    break;
                case WebRequestMethods.Http.Put:
                    response = await client.PutAsync(endpoint, content);
                    break;
                case "PATCH":
                    response = await client.PatchAsync(endpoint, content);
                    break;
            }
  
            LogResponse(response);
            
            Assert.True(response.StatusCode == expectedStatusCode);
            output.WriteLine("Test passed!");
        }
        catch (Exception e)
        {
            Assert.True(false, e.Message);
        }
        
    }

    private async void LogResponse(HttpResponseMessage response)
    {
        output.WriteLine("Status: " + response.StatusCode);
        output.WriteLine("Response body: " + await response.Content.ReadAsStringAsync());
    }

    private void UseUserToken()
    {
        userClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + userToken);
        output.WriteLine("Token: " + userToken);
    }
    
    private void UseAdminToken()
    {
        adminClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + adminToken);
        output.WriteLine("Token: " + adminToken);
    }
}