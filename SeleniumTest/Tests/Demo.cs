using System;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using SeleniumStorageProvider;
using SeleniumStorageProvider.Enum;
using SeleniumStorageProvider.Provider.AzureBlob;
using SeleniumTest.Business;
using SeleniumTest.Enums;
using SeleniumTest.Helpers;

namespace SeleniumTest.Tests
{
    [TestFixture(Devices.Ipad)]
    [TestFixture(Devices.Nexus5)]
    [TestFixture(Devices.Desktop)]
    public class Demo : SeleniumBase
    {
        private Devices _device;
        private ScreenCaptureStorage _screenCaptureStorage;

        public Demo()
        {
            this._device = Devices.Desktop;
        }

        public Demo(Devices device)
        {
            this._device = device;
        }

        private const string Url = "/";

        [SetUp]
        public void Initialize()
        {
            SetupDriver(_device);
            _screenCaptureStorage = new ScreenCaptureStorage(TestContext.CurrentContext.TestDirectory);
            _screenCaptureStorage.Start();
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

                throw new Exception("Test");

                const int expectedResult = 5;
                int actualResult = Driver.GetElementsByAttribute(ElementType.Li, AttributeType.Class, "g").Count;

                actualResult.Should().BeGreaterOrEqualTo(expectedResult);
            }
            catch (Exception all)
            {
                TakeScreenshot(Driver, all.ToString(), EventType.Error);
                throw;
            }
            
        }

        private void TakeScreenshot(IWebDriver driver, string message, EventType eventType)
        {
            var methodName = new StackFrame(1, true).GetMethod().Name;
            _screenCaptureStorage.Save(driver.PageSource, Driver.Url, message, methodName, eventType);
        }
    }
}
