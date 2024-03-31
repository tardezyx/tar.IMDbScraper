using OpenQA.Selenium;
using System;
using System.Threading;

namespace tar.IMDbScraper.Extensions {
  internal static partial class Extensions {
		#region --- safe click ----------------------------------------------------------------
    internal static void SafeClick(this IWebElement element, int intervalInMilliseconds = 25, int timeoutInMilliseconds = 200) {
      bool success = false;
      int  counter = 0;
      while (!success && counter < timeoutInMilliseconds) {
        try {
          Thread.Sleep(TimeSpan.FromMilliseconds(intervalInMilliseconds));
          element.Click();
          success = true;
          return;
        } catch {
          counter += intervalInMilliseconds;
        }
      }
    }
    #endregion
  }
}