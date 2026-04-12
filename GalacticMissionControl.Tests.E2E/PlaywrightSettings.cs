namespace GalacticMissionControl.Tests.E2E;

internal static class PlaywrightSettings
{
    public const int DefaultTimeoutMs = 10_000;
    public const int NavigationTimeoutMs = 15_000;

    public static readonly string[] ChromiumArgs =
    [
        "--no-sandbox",
        "--disable-dev-shm-usage"
    ];
}
