using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace ActivitieTest
{
    public static class driverConduct
    {
        //在徽章活动页面登录
        public static void loginAtActive(IWebDriver driver, string userName, string password)
        {

        }
        //等待
        public static IWebElement webDriverWait(this IWebDriver driver, By by, int timeoutInSeconds)   //FindElement增加等待功能
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }
        //跳转
        public static void getConsBadge(IWebDriver driver, string userName, string password, string consBadgeID, string URLExten)    
        {
            driver.Navigate().GoToUrl("http://betashop.9you.com/active/active/name/ConsBadge" + URLExten);
            betaShopLogin(userName, password, driver);
            driver.webDriverWait(By.XPath("//input[contains(@value,'" + consBadgeID + "')]"), 5).Click();
            driver.FindElement(By.Id("n_input_0")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.LinkText("特别体验区")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.Id("tj")).Click();
        }
        //切到
        public static string GetCurrentWindowHandle(IWebDriver driver)
        {
            string currentWindow = driver.CurrentWindowHandle;
            driver.SwitchTo().Window(currentWindow);
            return currentWindow;
        }
        //劲舞团页面商城用户登录
        public static void betaShopLogin(string userName, string password, IWebDriver driver)
        {
            driver.FindElement(By.LinkText("用户登录")).Click();
            driver.webDriverWait(By.Id("userName"), 5).SendKeys(userName);
            driver.FindElement(By.Id("password")).SendKeys(password);
            driver.FindElement(By.Id("identifyingCode")).SendKeys("1111");
            driver.FindElement(By.Id("submitlog")).Click();
        }
        //切换至当前页面句柄
        public static string GetCurrenWindowHandle(IWebDriver driver)
        {
            string currenWindow = driver.CurrentWindowHandle;
            driver.SwitchTo().Window(currenWindow);
            return currenWindow;
        }
        //切换至另一个窗口 并退出原来窗口
        public static void switchToAnotherWindow(IWebDriver driver)
        {
            IList<string> handlers = driver.WindowHandles;
            foreach (var winHandler in handlers)
            {
                if (winHandler != driver.CurrentWindowHandle)
                {
                    driver.Close();
                    driver.SwitchTo().Window(winHandler);
                }
            }
        }

        //切换至特定iframe
        public static void SwitchToFrame(IWebDriver driver, string _xpath)
        {
            IWebElement oneIframe = driver.webDriverWait(By.XPath(_xpath), 5);
            driver.SwitchTo().Frame(oneIframe);
        }
        //切换至特定iframe，无等待
        public static void SwitchToFrame2(IWebDriver driver, string _xpath)
        {
            IWebElement oneIframe = driver.FindElement(By.XPath(_xpath));
            driver.SwitchTo().Frame(oneIframe);
        }
        //正则表达式获取int型参数
        public static string selectIntNum(string refContent)
        {
            string regex = @"(\d+)\D+";
            Match mstr = Regex.Match(refContent, regex);
            return mstr.Groups[1].Value;
        }
        //购买1200价值的道具
        public static void buy1200(string userName, IWebDriver driver)                                              
        {
            driver.Navigate().GoToUrl("http://betashop.9you.com/item/show/pkey/B7IOSWvt1428477629/eq/0");
            driver.webDriverWait(By.LinkText("送给朋友"), 5).Click();
            driver.FindElement(By.CssSelector("input[name='getusername']")).SendKeys(userName);
            driver.FindElement(By.CssSelector("input[name='getusername_r']")).SendKeys(userName);
            //driver.FindElement(By.CssSelector("form#Sendmode>p>input.n_checkbox")).Click();
            driver.FindElement(By.CssSelector("form[id='Sendmode']>p>input[data-type='server-selector']")).Click();

            Thread.Sleep(500);
            driver.FindElement(By.LinkText("特别体验区")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("form#Sendmode>a.n_qdBtn")).Click();
            Thread.Sleep(500);
            SwitchToFrame(driver, "//iframe[contains(@class,'fancybox-iframe')]");
            driver.FindElement(By.Id("bt1")).Click();
            driver.SwitchTo().DefaultContent();
            driver.FindElement(By.CssSelector("a.fancybox-item.fancybox-close")).Click();
        }
        //给自己买1200道具
        public static void buySelf1200(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("http://betashop.9you.com/item/show/pkey/B7IOSWvt1428477629/eq/0");
            driver.FindElement(By.CssSelector("form[id='Buymode']>p>input[data-type='server-selector']")).Click();
            Thread.Sleep(500);
            driver.webDriverWait(By.LinkText("特别体验区"), 5).Click();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("form#Buymode>a.n_qdBtn")).Click();
            Thread.Sleep(500);
            SwitchToFrame(driver, "//iframe[contains(@class,'fancybox-iframe')]");
            driver.FindElement(By.Id("bt1")).Click();
            driver.SwitchTo().DefaultContent();
            driver.FindElement(By.CssSelector("a.fancybox-item.fancybox-close")).Click();
        }
        //快捷通道购买喇叭
        public static void buySpeaker(string userName, string num, IWebDriver driver)                               
        {
            driver.FindElement(By.CssSelector("div#quickCon_1.quickCon>p>input.inp1")).SendKeys(num);
            driver.FindElement(By.CssSelector("div#quickCon_1.quickCon>p>input.inp2")).SendKeys(userName);
            driver.FindElement(By.CssSelector("div#quickCon_1.quickCon>p>input.inp2.indexDq")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.LinkText("特别体验区")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("div#quickCon_1.quickCon>a.qdBtn2")).Click();
            Thread.Sleep(500);
            SwitchToFrame(driver, "//iframe[contains(@class,'fancybox-iframe')]");
            driver.FindElement(By.Id("bt1")).Click();
            driver.SwitchTo().DefaultContent();
            driver.FindElement(By.CssSelector("a.fancybox-item.fancybox-close")).Click();
        }
        //快捷通道购买经验
        public static void buyExp(string userName, string num, IWebDriver driver)                                       
        {
            driver.FindElement(By.Id("quickNav_2")).Click();
            driver.FindElement(By.CssSelector("div#quickCon_2.quickCon>p>input.inp1")).SendKeys(num);
            driver.FindElement(By.CssSelector("div#quickCon_2.quickCon>p>input.inp2")).SendKeys(userName);
            driver.FindElement(By.CssSelector("div#quickCon_2.quickCon>p>input.inp2.indexDq")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.LinkText("特别体验区")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("div#quickCon_2.quickCon>a.qdBtn2")).Click();
            Thread.Sleep(500);
            SwitchToFrame(driver, "//iframe[contains(@class,'fancybox-iframe')]");
            driver.FindElement(By.Id("bt1")).Click();
            driver.SwitchTo().DefaultContent();
            driver.FindElement(By.CssSelector("a.fancybox-item.fancybox-close")).Click();
        }
        //返现币购买100MB喇叭
        public static void buySpeakerInBack(string userName, IWebDriver driver)                                         
        {
            driver.Navigate().GoToUrl("http://betashop.9you.com/item/show/pkey/OJxwfWsZ1135153983");
            driver.webDriverWait(By.LinkText("送给朋友"), 5).Click();
            driver.FindElement(By.CssSelector("input[name='special']")).SendKeys("100");
            driver.FindElement(By.CssSelector("input[name='getusername']")).SendKeys(userName);
            driver.FindElement(By.CssSelector("input[name='getusername_r']")).SendKeys(userName);
            driver.FindElement(By.CssSelector("form#Sendmode>p>input.n_checkbox")).Click();
            driver.FindElement(By.CssSelector("form[id='Sendmode']>p>input[data-type='server-selector']")).Click();

            Thread.Sleep(500);
            driver.FindElement(By.LinkText("特别体验区")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("form#Sendmode>a.n_qdBtn")).Click();
            Thread.Sleep(500);
            SwitchToFrame(driver, "//iframe[contains(@class,'fancybox-iframe')]");
            driver.FindElement(By.Id("bt1")).Click();
            driver.SwitchTo().DefaultContent();
            driver.FindElement(By.CssSelector("a.fancybox-item.fancybox-close")).Click();
        }
        //返现币购买100MB经验
        public static void buyExpInBack(string userName, IWebDriver driver)                                             
        {
            driver.Navigate().GoToUrl("http://betashop.9you.com/item/show/pkey/kBoubnG61322652566");
            driver.webDriverWait(By.LinkText("送给朋友"), 5).Click();
            driver.FindElement(By.CssSelector("input[name='special']")).SendKeys("100");
            driver.FindElement(By.CssSelector("input[name='getusername']")).SendKeys(userName);
            driver.FindElement(By.CssSelector("input[name='getusername_r']")).SendKeys(userName);
            driver.FindElement(By.CssSelector("form#Sendmode>p>input.n_checkbox")).Click();
            driver.FindElement(By.CssSelector("form[id='Sendmode']>p>input[data-type='server-selector']")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.LinkText("特别体验区")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("form#Sendmode>a.n_qdBtn")).Click();
            Thread.Sleep(500);
            SwitchToFrame(driver, "//iframe[contains(@class,'fancybox-iframe')]");
            driver.FindElement(By.Id("bt1")).Click();
            driver.SwitchTo().DefaultContent();
            driver.FindElement(By.CssSelector("a.fancybox-item.fancybox-close")).Click();
        }

        public static void sendRing(IWebDriver driver)
        {
            switchToAnotherWindow(driver);
            driver.FindElement(By.CssSelector("form[id='Buymode']>p>input[data-type='server-selector']")).Click();
            Thread.Sleep(500);
            driver.webDriverWait(By.LinkText("特别体验区"), 5).Click();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("form#Buymode>a.n_qdBtn")).Click();
        }
        //送个朋友戒指到特别体验区
        public static void sendRing_2(IWebDriver driver, string getRingUser)
        {
            switchToAnotherWindow(driver);
            driver.webDriverWait(By.LinkText("送给朋友"), 5).Click();
            driver.FindElement(By.CssSelector("input[name='getusername']")).SendKeys(getRingUser);
            driver.FindElement(By.CssSelector("input[name='getusername_r']")).SendKeys(getRingUser);
            driver.FindElement(By.CssSelector("form[id='Sendmode']>p>input[data-type='server-selector']")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.LinkText("特别体验区")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("form#Sendmode>a.n_qdBtn")).Click();
        }

        
    }
}
