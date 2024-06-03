using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestUi
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class EditorsTests : PageTest
    {
        [Test]
        public async Task DropdownListSelectValue()
        {
            await Page.GotoAsync("https://localhost:44356/Editors");

            await Page.Locator("#maincont div").Filter(new() { HasText = "DropdownList please select" }).GetByLabel("open").ClickAsync();
            await Page.Locator("#awep1").GetByText("Banana").ClickAsync();

            await Expect(Page.Locator("button").Filter(new() { HasText = "Banana" })).ToBeVisibleAsync();
        }
    }
}
