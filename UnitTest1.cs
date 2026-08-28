using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace PlaywrightPipelineDemo;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SauceDemoTests : PageTest
{
    [SetUp]
    public async Task StartTracingAsync()
    {
        // Start Playwright tracing before each test
        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task CaptureFailureEvidenceAsync()
    {
        // Only capture evidence when the test fails
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            var testName = TestContext.CurrentContext.Test.Name;

            var outputDirectory = Path.Combine(
                "test-results",
                "playwright",
                testName);

            Directory.CreateDirectory(outputDirectory);

            // Capture screenshot
            await Page.ScreenshotAsync(new()
            {
                Path = Path.Combine(outputDirectory, "screenshot.png"),
                FullPage = true
            });

            // Save Playwright trace
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(outputDirectory, "trace.zip")
            });
        }
        else
        {
            // Stop tracing without saving it for successful tests
            await Context.Tracing.StopAsync();
        }
    }

    [Test]
    public async Task SuccessfulLogin_ShouldShowProductsPage()
    {
        // Go to the demo site
        await Page.GotoAsync("https://www.saucedemo.com/");

        // Fill out the login form
        await Page.Locator("[data-test='username']")
            .FillAsync("standard_user");

        await Page.Locator("[data-test='password']")
            .FillAsync("secret_sauce");

        await Page.Locator("[data-test='login-button']")
            .ClickAsync();

        // Verify login worked by checking the title
        var title = Page.Locator(".title");

        await Expect(title)
            .ToHaveTextAsync("WRONG_TEXT");
    }
}