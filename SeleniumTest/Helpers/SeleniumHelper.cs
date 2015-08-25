using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace SeleniumTest.Helpers
{
    public static class SeleniumHelper
    {
        public static IWebElement GetElementByAttribute(this IWebDriver driver, string element, string attribute, string id, int index = 0)
        {
            var query = string.Format(".//{0}[contains(@{1},'{2}')]", element, attribute, id);
            return GetElements(driver, query, index);
        }

        public static IWebElement GetElement(this IWebDriver driver, string element, int index = 0)
        {
            var query = string.Format(".//{0}", element);
            return GetElements(driver, query, index);
        }

        private static IWebElement GetElements(IWebDriver driver, string query, int index)
        {
            var elements = driver.FindElements(By.XPath(query));
            if (!elements.Any())
            {
                throw new Exception(string.Format("No elements returned with query {0} looking for index {1}", query, index));
            }

            return elements[index];
        }

        public static IWebElement GetElementByAttribute(this IWebElement driver, string element, string attribute, string id, int index = 0)
        {
            var query = string.Format(".//{0}[contains(@{1},'{2}')]", element, attribute, id);
            return GetElements(driver, query, index);
        }

        public static ReadOnlyCollection<IWebElement> GetElementsByAttribute(this IWebDriver driver, string element, string attribute, string id)
        {
            var query = string.Format(".//{0}[contains(@{1},'{2}')]", element, attribute, id);
            var elements = driver.FindElements(By.XPath(query));
            if (!element.Any())
            {
                throw new Exception(string.Format("No elements returned with query {0}", query));
            }
            return elements;
        }

        public static IWebElement GetElement(this IWebElement driver, string element, int index = 0)
        {
            var query = string.Format(".//{0}", element);
            return GetElements(driver, query, index);
        }

        private static IWebElement GetElements(IWebElement driver, string query, int index)
        {
            var elements = driver.FindElements(By.XPath(query));
            if (!elements.Any())
            {
                throw new Exception(string.Format("No elements returned with query {0} looking for index {1}", query, index));
            }

            return elements[index];
        }

        public static int GetElementCount(this IWebElement webelement, string element)
        {
            var elements = webelement.FindElements(By.XPath(string.Format(".//{0}", element)));
            return elements.Count;
        }
    }
}
