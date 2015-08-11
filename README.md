# Selenium-storage-provider

Save your selenium screenshots to a azure blob storage account. Including the url, stacktrace and a screenshot.

Example: 
```
private Storage _storage;
[SetUp]
public void Initialize()
{
	_storage = new Storage();
}

private void TakeScreenshot(IWebDriver driver, string message, EventType eventType)
{
	var methodName = new StackFrame(1, true).GetMethod().Name;
	string screenshot = driver.TakeScreenshot().AsBase64EncodedString;
	_storage.Save(screenshot, driver.PageSource, _driver.Url, message, methodName, eventType);
}
```

## Screenshot
![Image of Yaktocat](https://raw.githubusercontent.com/marcoippel/Selenium-storage-provider/develop/Screenshot/screenshot.PNG)
