# Selenium-storage-provider

Save your selenium screenshots to a azure blob storage account. Including the url, stacktrace and a screenshot.

Example: 
```
private void TakeScreenshot(IWebDriver driver, string message, EventType eventType)
{
    string screenshot = driver.TakeScreenshot().AsBase64EncodedString;
    _storage.Save(screenshot, driver.PageSource, _driver.Url, message, eventType);
}
```
