using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.Base {
  internal static class WebBrowser {
    #region --- get chrome driver -----------------------------------------------------------------
    private static ChromeDriver GetChromeDriver(ChromeDriverService driverService) {
      ChromeOptions chromeOptions = new ChromeOptions();
      chromeOptions.AddArguments(
        "--blink-settings=imagesEnabled=false",
        "--disable-infobars",
        "--window-size=400,400"
      );
      chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
      chromeOptions.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);

      return new ChromeDriver(driverService, chromeOptions);
    }
    #endregion
    #region --- get dev tools session ------------------------------------------------- (async) ---
    private static async Task<DevToolsSession> GetDevToolsSessionAsync(ChromeDriver chromeDriver, OperationHashes operationHashes, DateTime compareTime) {
			DevToolsSession result = chromeDriver.GetDevToolsSession();
			await result.Domains.Network.EnableNetwork();

			result.DevToolsEventReceived += (s, e) => {
        if (
				  e.EventName.Equals("requestWillBeSentExtraInfo")
				  && e.EventData["headers"]?[":path"] is JToken path
				  && Uri.UnescapeDataString(path.ToString()) is string parameters
				  && parameters.Contains("\"sha256Hash\":\"")
			  ) {
          UpdateOperationHashes(operationHashes, compareTime, parameters);
        }
      };

      return result;
    }
    #endregion
    #region --- request all operation hashes ------------------------------------------ (async) ---
    internal static async Task<OperationHashes> RequestAllOperationHashesAsync(OperationHashes operationHashes) {
      /* for each operation
       * - the last update time is checked (as one page can contain hashes for multiple operations)
       * - the corresponding page is loaded (if not already open)
       * - the last update is used as exit condition (as the update is triggered in the background by the DevToolsSession)
       * - the page is parsed and necessary buttons are clicked (popups and "more")
       * - to find and click the buttons unhandled try-catch-blocks are needed in order to ignore DOM-exceptions
       */

      DateTime startOfUpdate = DateTime.Now;
      string page = string.Empty;

      using ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
      driverService.HideCommandPromptWindow = true;
			using ChromeDriver chromeDriver = GetChromeDriver(driverService);
			using DevToolsSession session = await GetDevToolsSessionAsync(chromeDriver, operationHashes, startOfUpdate);

      DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(chromeDriver) {
        Timeout = TimeSpan.FromMilliseconds(500),
        PollingInterval = TimeSpan.FromMilliseconds(250)
      };

      foreach (OperationHash operationHash in operationHashes.OrderBy(x => x.Page)) {
        if (operationHash.LastUpdate > startOfUpdate && operationHash.Hash.HasText()) {
          continue;
        }

        bool newPage = false;
        if (!page.Equals(operationHash.Page)) {
          page = operationHash.Page;
          newPage = true;

			    chromeDriver.Navigate().GoToUrl(operationHash.Page);
          IWait<IWebDriver> wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30.00));
          wait.Until(driver => chromeDriver.ExecuteScript("return document.readyState").Equals("complete"));
          await Task.Delay(500);
        }

        while (operationHash.LastUpdate < startOfUpdate || operationHash.Hash.IsNullOrEmpty()) {
          // --- find and click on cookies reject button ------------------------------------------
          if (newPage) {
            try {
              while (fluentWait.Until(x => x.FindElement(By.XPath("//button[@data-testid=\"reject-button\"]"))) is IWebElement rejectButton) {
                if (rejectButton.Location.Y > 100) {
                  chromeDriver.ExecuteScript($"window.scrollTo(0, {rejectButton.Location.Y - 200})");
                  await Task.Delay(500);
                }
                rejectButton.SafeClick();
              }
            } catch { }
          }

          // --- find and click on all "more" buttons to trigger get-request for most hashes ------
          try {
            while (fluentWait.Until(x => x.FindElements(By.ClassName("ipc-see-more__text"))).FirstOrDefault() is IWebElement moreButton) {
              bool promptAppeared = false;

              if (moreButton.Location.Y > 100) {
                chromeDriver.ExecuteScript($"window.scrollTo(0, {moreButton.Location.Y - 200})");
                await Task.Delay(500);
              }

              // --- find and click on close prompt button (if it appears when scrolled) ----------
              try {
                while (fluentWait.Until(x => x.FindElement(By.XPath("//div[@data-testid=\"promptable__x\"]"))) is IWebElement closePromptButton) {
                  promptAppeared = true;
                  if (closePromptButton.Location.Y > 100) {
                    chromeDriver.ExecuteScript($"window.scrollTo(0, {closePromptButton.Location.Y - 200})");
                    await Task.Delay(500);
                  }
                  closePromptButton.SafeClick();
                }
              } catch { }

              if (!promptAppeared) {
                moreButton.SafeClick();
              }
            }
          } catch { }

          // --- scroll down to trigger get-requests for particular hashes ------------------------
          chromeDriver.ExecuteScript($"window.scrollTo(0, document.body.scrollHeight)");
          await Task.Delay(500);
        }
			}

      return operationHashes;
    }
    #endregion
    #region --- update operation hashes -----------------------------------------------------------
    private static void UpdateOperationHashes(OperationHashes operationHashes, DateTime compareTime, string parameters) {
			Operation operation = parameters.GetSubstringBetweenStrings("/?operationName=", "&").GetEnumByDescription<Operation>();

      if (operationHashes.FirstOrDefault(x => x.Operation == operation && x.LastUpdate < compareTime) is OperationHash operationHash) {
        operationHash.Hash       = parameters.GetSubstringBetweenStrings("\"sha256Hash\":\"", "\"");
        operationHash.LastUpdate = DateTime.Now;
      }
    }
    #endregion
  }
}