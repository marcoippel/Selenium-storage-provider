using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using SeleniumTest.Enums;
using SeleniumTest.Models;

namespace SeleniumTest.Helpers
{
    public class SeleniumDriverHelper
    {
        public static IWebDriver GetDriver(Devices device)
        {
            return GetDriver(string.Empty, device);
        }

        public static IWebDriver GetDriver(string driver, Devices device)
        {
            DeviceModel model = Device.Get(device);
            IWebDriver webDriver;
            switch (driver.ToLower())
            {
                case "safari":
                    webDriver = new SafariDriver();
                    break;
                case "chrome":
                    webDriver = new ChromeDriver();
                    break;
                case "ie":
                    webDriver = new InternetExplorerDriver();
                    break;
                //case "firefox":
                default:
                    var profile = new FirefoxProfile();
                    profile.SetPreference("general.useragent.override", model.UserAgent);
                    webDriver = new FirefoxDriver(profile);
                    webDriver.Manage().Window.Size = model.ScreenSize;
                    break;
            }
            return webDriver;
        }
    }
}
