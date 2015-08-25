using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTest.Extensions
{
    public static class WebElement
    {
        public static void CustomSendKeys(this IWebElement webelement, string key)
        {
            webelement.Clear();
            if (!string.IsNullOrWhiteSpace(key))
            {
                webelement.SendKeys(key);
            }
        }

        public static void SelectByValue(this IWebElement webelement, string value)
        {
            webelement.FindElement(By.XPath(string.Format(".//option[@value = '{0}']", value))).Click();
        }

        public static string GetSelectedValue(this IWebElement webelement)
        {
            var select = new SelectElement(webelement);
            var selected = select.AllSelectedOptions.First();

            return selected != null ? selected.GetAttribute("value") : string.Empty;
        }

        public static string GetValue(this IWebElement webelement)
        {
            return webelement == null ? string.Empty : webelement.GetAttribute("value");
        }
    }
}
