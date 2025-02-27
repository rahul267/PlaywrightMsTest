using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [TestClass]
    public class BlazeDemoTests
    {
        private IBrowser browser;
        private IBrowserContext context;
        private IPage page;

        [TestInitialize]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            context = await browser.NewContextAsync();
            page = await context.NewPageAsync();
        }

        [TestCleanup]
        public async Task Teardown()
        {
            await page.CloseAsync();
            await context.CloseAsync();
            await browser.CloseAsync();
        }

        [TestMethod]
        public async Task TestFlightSearchFunctionality()
        {
            await page.GotoAsync("http://blazedemo.com");
            await page.SelectOptionAsync("select[name='fromPort']", "Boston");
            await page.SelectOptionAsync("select[name='toPort']", "New York");
            await page.ClickAsync("input[type='submit']");
            Assert.IsTrue(await page.Locator("table").IsVisibleAsync(), "Flight search results are not visible.");
        }

        [TestMethod]
        public async Task TestFlightBookingProcess()
        {
            await page.GotoAsync("http://blazedemo.com");
            await page.SelectOptionAsync("select[name='fromPort']", "Boston");
            await page.SelectOptionAsync("select[name='toPort']", "New York");
            await page.ClickAsync("input[type='submit']");
            await page.ClickAsync("table tr:nth-child(1) input");
            await page.FillAsync("input[name='inputName']", "John Doe");
            await page.FillAsync("input[name='address']", "123 Main St");
            await page.FillAsync("input[name='city']", "Anytown");
            await page.FillAsync("input[name='state']", "State");
            await page.FillAsync("input[name='zipCode']", "12345");
            await page.ClickAsync("input[type='submit']");
            Assert.IsTrue(await page.Locator("h1").InnerTextAsync() == "Thank you for your purchase today!");
        }

        [TestMethod]
        public async Task TestFieldValidationOnBookingPage()
        {
            await page.GotoAsync("http://blazedemo.com");
            await page.SelectOptionAsync("select[name='fromPort']", "Boston");
            await page.SelectOptionAsync("select[name='toPort']", "New York");
            await page.ClickAsync("input[type='submit']");
            await page.ClickAsync("table tr:nth-child(1) input");
            await page.ClickAsync("input[type='submit']");
            Assert.IsTrue(await page.Locator(".alert-danger").IsVisibleAsync(), "Validation error message is not visible.");
        }

        [TestMethod]
        public async Task TestNavigationToHomepage()
        {
            await page.GotoAsync("http://blazedemo.com");
            await page.SelectOptionAsync("select[name='fromPort']", "Boston");
            await page.SelectOptionAsync("select[name='toPort']", "New York");
            await page.ClickAsync("input[type='submit']");
            await page.ClickAsync("a[href='/']");
            Assert.IsTrue(await page.Locator("h1").InnerTextAsync() == "Welcome to the Simple Travel Agency!");
        }

        [TestMethod]
        public async Task TestPriceSortingFunctionality()
        {
            await page.GotoAsync("http://blazedemo.com");
            await page.SelectOptionAsync("select[name='fromPort']", "Boston");