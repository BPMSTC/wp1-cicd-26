using Microsoft.Playwright;
using Xunit;

namespace GalacticMissionControl.Tests.E2E;

public sealed class PlaywrightFixture : IAsyncLifetime
{
    private E2ETestWebApplicationFactory? _factory;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public string BaseUrl { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        _factory = new E2ETestWebApplicationFactory();

        using var warmupClient = _factory.CreateClient();
        BaseUrl = warmupClient.BaseAddress?.ToString().TrimEnd('/')
            ?? throw new InvalidOperationException("Unable to determine test server base URL.");

        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = PlaywrightSettings.ChromiumArgs
        });
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
        _factory?.Dispose();
    }

    public async Task<IBrowserContext> CreateContextAsync()
    {
        if (_browser is null)
        {
            throw new InvalidOperationException("Browser is not initialized.");
        }

        await _factory!.ResetDatabaseAsync();

        var context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize
            {
                Width = 1440,
                Height = 900
            }
        });

        context.SetDefaultTimeout(PlaywrightSettings.DefaultTimeoutMs);
        context.SetDefaultNavigationTimeout(PlaywrightSettings.NavigationTimeoutMs);

        return context;
    }
}

[CollectionDefinition("E2E")]
public sealed class E2ECollection : ICollectionFixture<PlaywrightFixture>
{
    public const string Name = "E2E";
}
