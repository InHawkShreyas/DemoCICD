using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightPipelineDemo;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SauceDemoTests : PageTest
{
    [Test]
    public async Task SuccessfulLogin_ShouldShowProductsPage()
    {
        // Go to the demo site
        await Page.GotoAsync("https://www.saucedemo.com/");

        // Fill out the login form
        await Page.Locator("[data-test='username']").FillAsync("standard_user");
        await Page.Locator("[data-test='password']").FillAsync("secret_sauce");
        await Page.Locator("[data-test='login-button']").ClickAsync();

        // Verify login worked by checking the title
        var title = Page.Locator(".title");
        await Expect(title).ToHaveTextAsync("Products");
    }
}