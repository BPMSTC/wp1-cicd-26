using System.Net;
using System.Text.RegularExpressions;
using GalacticMissionControl.Web.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GalacticMissionControl.Tests.Integration;

public class MissionEndpointsIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>, IAsyncLifetime
{
    private static readonly Regex RequestVerificationTokenRegex =
        new("name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(?<token>[^\"]+)\"", RegexOptions.Compiled);

    private readonly IntegrationTestWebApplicationFactory _factory;

    public MissionEndpointsIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetMissionIndex_ReturnsSuccessAndRendersMissionBoard()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/Mission");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Mission Board", html);
        Assert.Contains("Europa Ice Survey", html);
    }

    [Fact]
    public async Task GetMissionDetails_WithExistingId_ReturnsSuccessAndMissionContent()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/Mission/Details/1");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Europa Ice Survey", html);
        Assert.Contains("Jupiter Frontier", html);
    }

    [Fact]
    public async Task GetMissionCreate_ReturnsSuccessAndForm()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/Mission/Create");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Launch New Mission", html);
        Assert.Contains("__RequestVerificationToken", html);
    }

    [Fact]
    public async Task PostMissionCreate_WithValidData_RedirectsAndPersistsMission()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var token = await GetAntiForgeryTokenAsync(client);

        var form = new Dictionary<string, string>
        {
            ["Mission.Title"] = "Mars Relay Deployment",
            ["Mission.AssignedSector"] = "Mars Orbit",
            ["Mission.Commander"] = "Cmdr. Sato",
            ["Mission.Priority"] = "Normal",
            ["Mission.Status"] = "Planned",
            ["Mission.ThreatLevel"] = "Low",
            ["Mission.LaunchDate"] = "2033-10-20",
            ["Mission.Description"] = "Deploy long-range relay satellites.",
            ["__RequestVerificationToken"] = token
        };

        var response = await client.PostAsync("/Mission/Create", new FormUrlEncodedContent(form));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Mission", response.Headers.Location?.OriginalString);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Contains(dbContext.Missions, m => m.Title == "Mars Relay Deployment" && m.AssignedSector == "Mars Orbit");
    }

    [Fact]
    public async Task PostMissionCreate_WithInvalidData_ReturnsFormWithValidationErrors()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var token = await GetAntiForgeryTokenAsync(client);

        var form = new Dictionary<string, string>
        {
            ["Mission.Title"] = "x",
            ["Mission.AssignedSector"] = string.Empty,
            ["Mission.Commander"] = "",
            ["Mission.Priority"] = string.Empty,
            ["Mission.Status"] = string.Empty,
            ["Mission.ThreatLevel"] = string.Empty,
            ["Mission.LaunchDate"] = string.Empty,
            ["Mission.Description"] = "",
            ["__RequestVerificationToken"] = token
        };

        var response = await client.PostAsync("/Mission/Create", new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("validation-summary", html);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(2, dbContext.Missions.Count());
    }

    private static async Task<string> GetAntiForgeryTokenAsync(HttpClient client)
    {
        var createResponse = await client.GetAsync("/Mission/Create");
        createResponse.EnsureSuccessStatusCode();
        var createHtml = await createResponse.Content.ReadAsStringAsync();

        var match = RequestVerificationTokenRegex.Match(createHtml);
        if (!match.Success)
        {
            throw new InvalidOperationException("Could not find anti-forgery token in Create Mission form.");
        }

        return match.Groups["token"].Value;
    }
}
