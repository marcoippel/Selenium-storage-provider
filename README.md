# Selenium-storage-provider

Save your selenium screenshots to a azure blob storage account. Including the url, stacktrace and a screenshot.

Example: 
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
			string screenshot = driver.TakeScreenshot().AsBase64EncodedString;
			_storage.Save(screenshot, driver.PageSource, _driver.Url, message, methodName, eventType);
		}
    }
}

```



## Screenshot
![Image of Yaktocat](https://raw.githubusercontent.com/marcoippel/Selenium-storage-provider/develop/Screenshot/screenshot.PNG)
