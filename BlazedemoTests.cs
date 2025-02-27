using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace BlazedemoTests
{
    [TestClass]
    public class BlazedemoFlightTests
    {
        private IBrowser browser;
        private IBrowserContext context;
        private IPage page;

        [TestInitialize]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            context = await browser.NewContextAsync();
            page = await context.NewPageAsync();
            await page.GotoAsync("http://blazedemo.com");
        }

        [TestCleanup]
        public async Task Teardown()
        {
            await context.CloseAsync();
            await browser.CloseAsync();
        }

        [TestMethod]
        public async Task TestFlightSearchFunctionality()
        {
            await page.SelectOptionAsync("select[name='fromPort']", "Boston");
            await page.SelectOptionAsync("select[name='toPort']", "New York");
            await page.ClickAsync("input[type='submit']");
            var flights = await page.QuerySelectorAllAsync("table tbody tr");
            Assert.IsTrue(flights.Length > 0, "Flight search did not return any results.");
        }

        [TestMethod]
        public async Task TestFlightBookingProcess()
        {
            await TestFlightSearchFunctionality();
            await page.ClickAsync("table tbody tr:first-child input[type='submit']");
            await page.FillAsync("#inputName", "John Doe");
            await page.FillAsync("#address", "123 Elm Street");
            await page.FillAsync("#city", "Anytown");
            await page.FillAsync("#state", "CA");
            await page.FillAsync("#zipCode", "12345");
            await page.FillAsync("#creditCardNumber", "4111111111111111");
            await page.ClickAsync("input[type='submit']");
            var confirmation = await page.TextContentAsync(".hero-unit");
            Assert.IsTrue(confirmation.Contains("Thank you for your purchase!"), "Flight booking was not successful.");
        }

        [TestMethod]
        public async Task ValidateErrorMessageForMissingPassengerDetails()
        {
            await TestFlightSearchFunctionality();
            await page.ClickAsync("table tbody tr:first-child input[type='submit']");
            await page.ClickAsync("input[type='submit']");
            var error = await page.TextContentAsync("#alert");
            Assert.IsTrue(error.Contains("Please enter your name"), "Error message for missing details not displayed.");
        }

        [TestMethod]
        public async Task VerifyNavigationToHomePage()
        {
            await TestFlightSearchFunctionality();
            await page.ClickAsync("a[href='/']");
            var pageTitle = await page.TitleAsync();
            Assert.AreEqual("BlazeDemo", pageTitle, "Navigation to home page failed.");
        }

        [TestMethod]
        public async Task ValidateCurrencyConversionOnBookingPage()
        {
            await TestFlightSearchFunctionality();
            await page.ClickAsync("table tbody tr:first-child input[type='submit']");
            var price = await page.TextContentAsync(".price");
            Assert.IsTrue(price.Contains("USD"), "Currency is not in USD.");
            // Additional logic to change and verify currency conversion can be added here.
        }
    }
}