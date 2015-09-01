[![Build status](https://ci.appveyor.com/api/projects/status/2nqf6d330qf09n27?svg=true)](https://ci.appveyor.com/project/marcoippel/selenium-storage-provider)
## What is Selenium storage provider?

Save your selenium screenshots including the stacktrace and page url to one of the providers.
There are currently 2 providers:
* Azure blob storage provider
* A Slack provider.

"A picture is worth a thousand words" so it is much more easy to debug your selenmiumtests with a screenshot if your test fails.

## How to get started?
* Create a selenium test project for example with the [SeleniumTemplate](https://github.com/marcoippel/SeleniumTemplate) project.
* Install the seleniumstorageprovider nuget package on the test project. Install-Package SeleniumStorageProvider
* Initialise a storage object
* Add the TakeScreenshot method in a baseclass.
* Add a try catch block around your test and in the catch block call the TakeSCreenshot method.
* In the app.config add the configuration for the provider.

## Create your own providers
* Create a new class and inhert from the IStorageProvider interface
* Implement the "void Save(byte[] screenshot, string pageSource, string url, string message, string methodName, EventType eventType);" method and save your data.

## Examples 
```

namespace SeleniumTestTemplate.Tests
{
    [TestFixture(Devices.Ipad)]
    [TestFixture(Devices.Nexus5)]
    [TestFixture]
    public class Demo : SeleniumBase
    {
		private Storage _storage;
        private const string Url = "/";
        private Devices _device;

        public Demo()
        {
            this._device = Devices.Desktop;
        }

        public Demo(Devices device)
        {
            this._device = device;
        }

        [SetUp]
        public void Initialize()
        {
            SetupDriver(_device);
			_storage = new Storage();
        }

        [TearDown]
        public void Cleanup()
        {
            Driver.Quit();
        }

        [Test]
        public void SearchOnGoogle()
        {
			try
            {
				Goto(Url);
				Driver.GetElementByAttribute(ElementType.Input, AttributeType.Class, "gsfi").SendKeys("Selenium tests");
				Driver.GetElementByAttribute(ElementType.Button, AttributeType.Class, "lsb").Click();

				const int expectedResult = 5;
				int actualResult = Driver.GetElementsByAttribute(ElementType.Li, AttributeType.Class, "g").Count;

				actualResult.Should().BeGreaterOrEqualTo(expectedResult);
			}
            catch (Exception all)
            {
                TakeScreenshot(_driver, all.ToString(), EventType.Error);
                throw;
            }
        }
		
		private void TakeScreenshot(IWebDriver driver, string message, EventType eventType)
        {
            var methodName = new StackFrame(1, true).GetMethod().Name;
            byte[] screenshot = driver.TakeScreenshot().AsByteArray;
            _storage.Save(screenshot, driver.PageSource, Driver.Url, message, methodName, eventType);
        }
    }
}

```



## Screenshot
![Image of Yaktocat](https://raw.githubusercontent.com/marcoippel/Selenium-storage-provider/develop/Screenshot/screenshot.PNG)
