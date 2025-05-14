using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace NUnitToDo
{
    [TestFixture("chrome", "latest", "Windows 10")]
    [TestFixture("firefox", "latest", "Windows 10")]
    // [TestFixture("chrome", "latest", "Linux")]
    // [TestFixture("firefox", "latest", "Linux")]
    [Parallelizable(ParallelScope.Self)]

    [Category("ToDoTest")]
    public class NUnitSeleniumSample
    {
        public static string LT_USERNAME = "username";
        public static string LT_ACCESS_KEY = "key";
        public static string gridURL = "@hub.lambdatest.com/wd/hub";

        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        private string browser;
        private string version;
        private string os;

        public static ExtentReports _extent;
        public ExtentTest _test;
        public string TC_Name;
        public static string dirPath = "Reports//ToDoTest";

        [OneTimeSetUp]
        protected void ExtentStart()
        {
            var path = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var actualPath = path.Substring(0, path.LastIndexOf("bin"));

            var projectPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(projectPath.ToString() + dirPath);

            var reportPath = projectPath + dirPath + "//ToDoTestReport.html";

            /* For Version 4 --> Creates Index.html */
            var htmlReporter = new ExtentHtmlReporter(reportPath);
            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);
            _extent.AddSystemInfo("Host Name", "ToDo Testing on HyperTest Grid");
            _extent.AddSystemInfo("Environment", "Windows Platform");
            _extent.AddSystemInfo("UserName", "User");
            htmlReporter.LoadConfig(projectPath + "Configurations//report-config.xml");
        }

        public NUnitSeleniumSample(string browser, string version, string os)
        {
            this.browser = browser;
            this.version = version;
            this.os = os;
        }

        [SetUp]
        public void Init()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");  // Example: Add other arguments here

            // LambdaTest specific options
            Dictionary<string, object> ltOptions = new Dictionary<string, object>();
            ltOptions.Add("username", LT_USERNAME);
            ltOptions.Add("accessKey", LT_ACCESS_KEY);
            ltOptions.Add("project", "Untitled");
            ltOptions.Add("w3c", true);

            // Adding LambdaTest options to ChromeOptions
            options.AddAdditionalCapability("LT:Options", ltOptions, true);
            driver.Value = new RemoteWebDriver(new Uri("https://" + LT_USERNAME + ":" + LT_ACCESS_KEY + gridURL),
                            options.ToCapabilities(), TimeSpan.FromSeconds(600));

            Console.Out.WriteLine(driver);
        }

        [Test]
        public void Todotest()
        {
            string context_name = TestContext.CurrentContext.Test.Name + " on " + browser + " " + version + " " + os;
            TC_Name = context_name;

            _test = _extent.CreateTest(context_name);

            Console.WriteLine("Navigating to todos app.");
            driver.Value.Navigate().GoToUrl("https://lambdatest.github.io/sample-todo-app/");

            driver.Value.FindElement(By.Name("li4")).Click();
            Console.WriteLine("Clicking Checkbox");
            driver.Value.FindElement(By.Name("li5")).Click();

            /* If both clicks worked, then the following List should have length 2 */
            IList<IWebElement> elems = driver.Value.FindElements(By.ClassName("done-true"));

            /* so we'll assert that this is correct. */
            Assert.AreEqual(2, elems.Count);

            Console.WriteLine("Entering Text");
            driver.Value.FindElement(By.Id("sampletodotext")).SendKeys("Yey, Let's add it to list");
            driver.Value.FindElement(By.Id("addbutton")).Click();

            /* lets also assert that the new todo we added is in the list */
            string spanText = driver.Value.FindElement(By.XPath("/html/body/div/div/div/ul/li[6]/span")).Text;
            Assert.AreEqual("Yey, Let's add it to list", spanText);
        }

        [OneTimeTearDown]
        protected void ExtentClose()
        {
            Console.WriteLine("OneTimeTearDown");
            _extent.Flush();
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            var exec_status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace) ? ""
            : string.Format("{0}", TestContext.CurrentContext.Result.StackTrace);
            Status logstatus = Status.Pass;
            string screenShotPath, fileName;

            DateTime time = DateTime.Now;
            fileName = "Screenshot_" + time.ToString("h_mm_ss") + TC_Name + ".png";

            switch (exec_status)
            {
                case TestStatus.Failed:
                    logstatus = Status.Fail;
                    /* The older way of capturing screenshots */
                    screenShotPath = Capture(driver.Value, fileName);
                    /* Capturing Screenshots using built-in methods in ExtentReports 4 */
                    var mediaEntity = CaptureScreenShot(driver.Value, fileName);
                    _test.Log(Status.Fail, "Fail");
                    /* Usage of MediaEntityBuilder for capturing screenshots */
                    _test.Fail("ExtentReport 4 Capture: Test Failed", mediaEntity);
                    /* Usage of traditional approach for capturing screenshots */
                    _test.Log(Status.Fail, "Traditional Snapshot below: " + _test.AddScreenCaptureFromPath("Screenshots//" + fileName));
                    break;
                case TestStatus.Passed:
                    logstatus = Status.Pass;
                    /* The older way of capturing screenshots */
                    screenShotPath = Capture(driver.Value, fileName);
                    /* Capturing Screenshots using built-in methods in ExtentReports 4 */
                    mediaEntity = CaptureScreenShot(driver.Value, fileName);
                    _test.Log(Status.Pass, "Pass");
                    /* Usage of MediaEntityBuilder for capturing screenshots */
                    _test.Pass("ExtentReport 4 Capture: Test Passed", mediaEntity);
                    /* Usage of traditional approach for capturing screenshots */
                    _test.Log(Status.Pass, "Traditional Snapshot below: " + _test.AddScreenCaptureFromPath("Screenshots//" + fileName));
                    break;
                case TestStatus.Inconclusive:
                    logstatus = Status.Warning;
                    break;
                case TestStatus.Skipped:
                    logstatus = Status.Skip;
                    break;
                default:
                    break;
            }
            _test.Log(logstatus, "Test: " + TC_Name + " Status:" + logstatus + stacktrace);

            try
            {
                /* Logs the result to LambdaTest */
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                /* Terminates the remote webdriver session */
                driver.Value.Quit();
            }
        }

        public static string Capture(IWebDriver driver, string screenShotName)
        {
            ITakesScreenshot ts = (ITakesScreenshot)driver;
            Screenshot screenshot = ts.GetScreenshot();
            var pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var actualPath = pth.Substring(0, pth.LastIndexOf("bin"));
            var reportPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(reportPath + dirPath + "//Screenshots");
            var finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + dirPath + "//Screenshots//" + screenShotName;
            var localpath = new Uri(finalpth).LocalPath;
            screenshot.SaveAsFile(localpath, ScreenshotImageFormat.Png);
            return reportPath;
        }

        public MediaEntityModelProvider CaptureScreenShot(IWebDriver driver, string screenShotName)
        {
            ITakesScreenshot ts = (ITakesScreenshot)driver;
            var screenshot = ts.GetScreenshot().AsBase64EncodedString;

            return MediaEntityBuilder.CreateScreenCaptureFromBase64String(screenshot, screenShotName).Build();
        }
    }
}
