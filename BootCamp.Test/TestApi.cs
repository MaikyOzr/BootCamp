using Microsoft.AspNetCore.Mvc.Testing;

namespace BootCamp.Test;

public class TestApi : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public TestApi(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    //Add test for full app
}

