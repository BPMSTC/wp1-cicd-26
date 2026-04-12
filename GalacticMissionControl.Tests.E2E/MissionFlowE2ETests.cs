using Microsoft.Playwright;

namespace GalacticMissionControl.Tests.E2E;

[Collection(E2ECollection.Name)]
public sealed class MissionFlowE2ETests
{
    private readonly PlaywrightFixture _fixture;

    public MissionFlowE2ETests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HomePage_Loads()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("/");

        await ExpectVisibleHeadingAsync(page, "Mission Operations Dashboard");
        await page.GetByRole(AriaRole.Link, new() { Name = "Missions" }).WaitForAsync();
    }

    [Fact]
    public async Task MissionListPage_Loads()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("/Mission");

        await ExpectVisibleHeadingAsync(page, "Mission Board");
        await page.GetByText("Europa Ice Survey", new() { Exact = false }).WaitForAsync();
    }

    [Fact]
    public async Task MissionList_NavigatesToCreatePage()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("/Mission");
        await page.GetByRole(AriaRole.Link, new() { Name = "+ New Mission" }).ClickAsync();

        await page.WaitForURLAsync("**/Mission/Create");
        await ExpectVisibleHeadingAsync(page, "Launch New Mission");
    }

    [Fact]
    public async Task CreateMission_WithValidData_ShowsCreatedMissionOnMissionBoard()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("/Mission/Create");

        var missionTitle = $"Student Demo Mission {Guid.NewGuid():N}";

        await page.GetByLabel("Title").FillAsync(missionTitle);
        await page.GetByLabel("Assigned Sector").FillAsync("Kuiper Belt");
        await page.GetByLabel("Commander").FillAsync("Cmdr. Rivera");
        await page.GetByLabel("Priority").SelectOptionAsync("Normal");
        await page.GetByLabel("Status").SelectOptionAsync("Planned");
        await page.GetByLabel("Threat Level").SelectOptionAsync("Low");
        await page.GetByLabel("Launch Date").FillAsync("2034-02-01");
        await page.GetByLabel("Description").FillAsync("Student demo mission for end-to-end validation.");

        await page.GetByRole(AriaRole.Button, new() { Name = "Create Mission" }).ClickAsync();

        await page.WaitForURLAsync("**/Mission");
        await page.GetByRole(AriaRole.Heading, new() { Name = "Mission Board" }).WaitForAsync();
        await page.GetByText(missionTitle, new() { Exact = false }).WaitForAsync();
    }

    [Fact]
    public async Task CreateMission_WithMissingRequiredFields_ShowsValidationMessages()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("/Mission/Create");
        await page.GetByRole(AriaRole.Button, new() { Name = "Create Mission" }).ClickAsync();

        await page.WaitForURLAsync("**/Mission/Create");
        await page.GetByText("The Title field is required.", new() { Exact = false }).WaitForAsync();
        await page.GetByText("The Assigned Sector field is required.", new() { Exact = false }).WaitForAsync();
        await page.GetByText("The Priority field is required.", new() { Exact = false }).WaitForAsync();
        await page.GetByText("The Status field is required.", new() { Exact = false }).WaitForAsync();
        await page.GetByText("The Threat Level field is required.", new() { Exact = false }).WaitForAsync();
    }

    private static async Task ExpectVisibleHeadingAsync(IPage page, string heading)
    {
        await page.GetByRole(AriaRole.Heading, new() { Name = heading }).WaitForAsync();
    }
}
