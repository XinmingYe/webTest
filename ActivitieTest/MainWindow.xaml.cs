using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;


namespace ActivitieTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
       
        //徽章检测事件
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                badagePurchase2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        //为他人购买戒指测试事件
        private void button_RingTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ringForOther();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //送自己戒指事件
        private void button_RingTest_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ringForSelf();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
        /********徽章测试变量***********/
        string userName = null;
        string password = null;
        string URLExten = null;
        string RewardID = null;
        string cost;
        string startmb, quitmb, state;
        int startMB, quitMB;    
        /*******************************/
        
        //徽章检查输入函数
        public bool infoCheck()
        {
            userName = textBox_ID.Text;
            if (userName == "")
            {
                MessageBox.Show("请输入登录ID");
                return false;
            }
            password = textBox_Password.Text;
            if (password == "")
            {
                MessageBox.Show("请输入登录密码");
                return false;
            }          
            URLExten = textBox_Url.Text;
            if (URLExten == "")
            {
                MessageBox.Show("请输入活动ID");
                return false;
            }
            cost = textBox_Cost.Text;
            if (cost == "")
            {
                MessageBox.Show("请输入消费金额");
                return false;
            }
            RewardID = textBox_RewardId.Text;
            if (RewardID == "")
            {
                MessageBox.Show("请输入徽章ID");
                return false;
            }
            return true;
        }
        
        //徽章检测流程1
        public string badagePurchase()
        {
            //检查是否输入完整
            bool c = infoCheck();
            if(!c)
            {
                return null;
            }          

            IWebDriver driver_FF = new FirefoxDriver();
            //进入劲舞团页面商城//
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com/");                          
            driver_FF.Manage().Window.Maximize();
            ////// 空领 /////
            driverConduct.getConsBadge(driver_FF, userName, password, RewardID, URLExten);   //获取徽章
            Thread.Sleep(500);
            state = driver_FF.FindElement(By.XPath("//p[contains(@class,'t_center')]")).Text;   //获取上一步操作的返回信息
            logText.AppendText("空领状态：" + state + "\n");
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            //////第一次/////
            driverConduct.betaShopLogin(userName, password, driver_FF);                       //登录账号
            Thread.Sleep(500);
            driverConduct.GetCurrenWindowHandle(driver_FF);                                      //切换至当前句柄
            startmb = driver_FF.FindElement(By.CssSelector("div.loginMoney>span>b.f_red2")).Text;//获取页面上的MB余额
            startMB = Convert.ToInt32(driverConduct.selectIntNum(startmb));                      //将余额转化成整数型
            driverConduct.buy1200(userName, driver_FF);                                       //为自己购买一个售价为1200的道具
            driver_FF.Navigate().Refresh();                                                      //刷新页面
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            /////第二次购买/////
            driverConduct.betaShopLogin(userName, password, driver_FF);
            Thread.Sleep(500);
            driverConduct.GetCurrenWindowHandle(driver_FF);
            driverConduct.buySpeaker(userName, (Convert.ToInt32(cost) - 1300).ToString(), driver_FF);//为自己购买售价为(cost-1300)的喇叭
            driver_FF.Navigate().Refresh();
            quitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h2>b.f_red2")).GetAttribute("innerText");//获取页面上MB余额
            quitMB = Convert.ToInt32(quitmb);                                                      //转化为整数型
            logText.AppendText("实际消费：" + (startMB - quitMB) + "\n");                                  //输出前两次购买在页面上实际扣除的金额
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            driverConduct.getConsBadge(driver_FF, userName, password, RewardID, URLExten);                    //第二次获取徽章
            Thread.Sleep(500);
            state = driver_FF.FindElement(By.XPath("//p[contains(@class,'t_center')]")).Text;
            logText.AppendText("领取状态：" + state + "\n");                                                //打印返回信息
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            ////第三次购买/////
            driverConduct.betaShopLogin("lbtest01", password, driver_FF);                                      //登录lbtest01账号购买100返现币的喇叭（用于测试赠送+返现币消费）
            Thread.Sleep(500);
            driverConduct.GetCurrenWindowHandle(driver_FF);
            startmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h3>b.f_red2")).GetAttribute("innerText");
            startMB = Convert.ToInt32(startmb);
            driverConduct.buySpeakerInBack(userName, driver_FF);
            driver_FF.Navigate().Refresh();
            quitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h3>b.f_red2")).GetAttribute("innerText");
            quitMB = Convert.ToInt32(quitmb);
            logText.AppendText("实际消费：" + (startMB - quitMB) + "返现币   有效消费：100" + "\n");
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            driverConduct.getConsBadge(driver_FF, userName, password, RewardID, URLExten);
            Thread.Sleep(500);
            state = driver_FF.FindElement(By.XPath("//p[contains(@class,'t_center')]")).Text;
            logText.AppendText("领取状态：" + state + "\n");
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();

            string str =null;
            return str;
        }
        
        //徽章检测流程2
        public string badagePurchase2()
        {
            //检查是否输入完整
            bool c = infoCheck();
            if (!c)
            {
                return null;
            }

            IWebDriver driver_FF = new FirefoxDriver();
            //点击领取徽章，验证跳转正确//
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com/show/active/name/ConsBadge"+URLExten);
            driver_FF.Manage().Window.Maximize();
            driver_FF.FindElement(By.CssSelector("div.pic_btn>a.a1")).Click();
            driverConduct.switchToAnotherWindow(driver_FF);
            Thread.Sleep(500);

            ////// 空领 /////
            driverConduct.getConsBadge(driver_FF, userName, password, RewardID, URLExten);           
            Thread.Sleep(500);
            state = driver_FF.FindElement(By.XPath("//p[contains(@class,'t_center')]")).Text;   //获取上一步操作的返回信息
            logText.AppendText("空领状态：" + state + "\n");
            //////第一次/////
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com");
            Thread.Sleep(500);
            driverConduct.GetCurrenWindowHandle(driver_FF);                                      //切换至当前句柄
            startmb = driver_FF.FindElement(By.CssSelector("div.loginMoney>span>b.f_red2")).Text;//获取页面上的MB余额
            startMB = Convert.ToInt32(driverConduct.selectIntNum(startmb));                      //将余额转化成整数型
            driverConduct.buy1200(userName, driver_FF);                                       //为自己购买一个售价为1200的道具
            /////第二次购买/////
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com");
            Thread.Sleep(500);
            driverConduct.GetCurrenWindowHandle(driver_FF);
            driverConduct.buySpeaker(userName, (Convert.ToInt32(cost) - 1300).ToString(), driver_FF);//为自己购买售价为(cost-1300)的喇叭
            driver_FF.Navigate().Refresh();
            quitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h2>b.f_red2")).GetAttribute("innerText");//获取页面上MB余额
            quitMB = Convert.ToInt32(quitmb);                                                      //转化为整数型
            logText.AppendText("实际消费：" + (startMB - quitMB) + "\n");                                  //输出前两次购买在页面上实际扣除的金额
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            driverConduct.getConsBadge(driver_FF, userName, password, RewardID, URLExten);                    //第二次获取徽章
            Thread.Sleep(500);
            state = driver_FF.FindElement(By.XPath("//p[contains(@class,'t_center')]")).Text;
            logText.AppendText("领取状态：" + state + "\n");                                                //打印返回信息
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            ////第三次购买/////
            driverConduct.betaShopLogin("lbtest01", password, driver_FF);                                      //登录lbtest01账号购买100返现币的喇叭（用于测试赠送+返现币消费）
            Thread.Sleep(500);
            driverConduct.GetCurrenWindowHandle(driver_FF);
            startmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h3>b.f_red2")).GetAttribute("innerText");
            startMB = Convert.ToInt32(startmb);
            driverConduct.buySpeakerInBack(userName, driver_FF);
            driver_FF.Navigate().Refresh();
            quitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h3>b.f_red2")).GetAttribute("innerText");
            quitMB = Convert.ToInt32(quitmb);
            logText.AppendText("实际消费：" + (startMB - quitMB) + "返现币   有效消费：100" + "\n");
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            driverConduct.getConsBadge(driver_FF, userName, password, RewardID, URLExten);
            Thread.Sleep(500);
            state = driver_FF.FindElement(By.XPath("//p[contains(@class,'t_center')]")).Text;
            logText.AppendText("领取状态：" + state + "\n");
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            driverConduct.getConsBadge(driver_FF, userName, password, RewardID, URLExten);
            Thread.Sleep(500);
            state = driver_FF.FindElement(By.XPath("//p[contains(@class,'t_center')]")).Text;
            logText.AppendText("领取状态：" + state + "\n");
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();

            string str = null;
            return str;
        }

        /**********送他人戒指**********/
        string ringURLExten;
        string ringRecieveUser;
        string ringPassword="123456";
        string ringCost;
        string ringPKey;
        string ringStartmb, ringQuitmb; 
        int ringStartMB, ringQuitMB, accountNum;
        string getRingUser;
        string[] GetRingUs = new string[5];
        /*****************************/
        
        //给他人买戒指，读取输入
        public bool infoRing()
        {
            ringRecieveUser=textBox_ID_Ring.Text;
            if (ringRecieveUser == "")
            {
                MessageBox.Show("请输入登录ID");
                return false;
            }
            ringPassword = textBox_Password_Ring.Text;
            if (ringPassword == "")
            {
                MessageBox.Show("请输入登录密码");
                return false;
            }
            ringURLExten = textBox_Url_Ring.Text;
            if (ringURLExten == "")
            {
                MessageBox.Show("请输入活动ID");
                return false;
            }
            ringCost = textBox_Cost_Ring.Text;
            if (ringCost == "")
            {
                MessageBox.Show("请输入购买金额");
                return false;
            }
            ringPKey = textBox_RewardId_Ring.Text;
            if (ringPKey == "")
            {
                MessageBox.Show("请输入戒指pKey");
                return false;
            }
            string s1 = textBox_RewardId_Num.Text;
            accountNum = int.Parse(s1);
            if (s1 == "")
            {
                MessageBox.Show("请输入接受戒指帐号个数");
                return false;
            }
            string s2 = textBox_Reward_Account.Text;
            int N = textBox_Reward_Account.LineCount;
            if (N != accountNum)
            {
                MessageBox.Show("输入账户数与实际输入不符");
                return false;
            }
            for (int i=0;i< N; i++)
            {
                GetRingUs[i] = textBox_Reward_Account.GetLineText(i);
            }
            getRingUser = GetRingUs[N-1];
            if (s2 == "")
            {
                MessageBox.Show("请输入赠送帐号");
                return false;
            }

            return true;

        }

        //为他人购买戒指流程
        public string ringForOther()
        {
            bool c = infoRing();
            if (!c)
            {
                return null;
            }
            IWebDriver driver_FF = new FirefoxDriver();
            //跳转页面
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com/show/active/name/RingSign"+ ringURLExten);
            driver_FF.Manage().Window.Maximize();
            driver_FF.FindElement(By.XPath("//a[contains(@href,"+ ringPKey + ")]")).Click();//选择某个戒指
            driverConduct.switchToAnotherWindow(driver_FF);//要切换页面
            //切换后登录
            driverConduct.betaShopLogin(ringRecieveUser, ringPassword, driver_FF);
            Thread.Sleep(500);
            //送给最后一账户,并输出信息
            driverConduct.sendRing_2(driver_FF, getRingUser);
            Thread.Sleep(500);
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");//跳到弹出页面窗口
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                logText.AppendText("购买状态：购买成功");
            }
            else
            {
                logText.AppendText("购买状态：" + state + "\n");
            }
            //进入劲舞团页面商城完成不同类型消费,并输出
            driverConduct.GetCurrentWindowHandle(driver_FF);
            ringStartmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h2>b.f_red2")).GetAttribute("innerText");//mb余额
            ringStartMB = Convert.ToInt32(ringStartmb);
            driverConduct.buySelf1200(driver_FF);
            driverConduct.buySpeaker(ringRecieveUser, (Convert.ToInt32(ringCost) - 1300).ToString(), driver_FF);
            driver_FF.Navigate().Refresh();
            ringQuitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h2>b.f_red2")).GetAttribute("innerText");
            ringQuitMB = Convert.ToInt32(ringQuitmb);
            state = "实际消费：" + (ringStartMB - ringQuitMB).ToString();
            logText.AppendText( state + "\n");
            //跳转到pkey页面，还差100去领取
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com/item/show/pkey/" + ringPKey);
            driver_FF.FindElement(By.CssSelector("[data-bind-input='server_0']")).Click();//点击选区
            driver_FF.FindElement(By.CssSelector("[data-value='11']")).Click();//选择体验区
            driver_FF.FindElement(By.LinkText("确认提交")).Click();//提交
            Thread.Sleep(500);
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                state = "购买资格/状态:可以购买";
            }
            else
            {
                state = "购买资格/状态:" + state;
            }
            logText.AppendText(state + "\n" + "账号：" + getRingUser + "\n");
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            //登录lbtest01消费100返现币，送给usrname，完成购满条件
            driverConduct.betaShopLogin("lbtest01", ringPassword, driver_FF);
            Thread.Sleep(500);
            driverConduct.GetCurrentWindowHandle(driver_FF);
            ringStartmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h3>b.f_red2")).GetAttribute("innerText");
            ringStartMB = Convert.ToInt32(ringStartmb);
            driverConduct.buySpeakerInBack(ringRecieveUser, driver_FF);//买喇叭
            driver_FF.Navigate().Refresh();
            ringQuitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h3>b.f_red2")).GetAttribute("innerText");
            ringQuitMB = Convert.ToInt32(ringQuitmb);
            Console.WriteLine("实际消费：" + (ringStartMB - ringQuitMB) + "返现币   有效消费：100"+"\n");
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            //登录帐号，开始赠送测试
            driverConduct.betaShopLogin(ringRecieveUser, ringPassword, driver_FF);
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com/item/show/pkey/" + ringPKey);
            //driver_FF.FindElement(By.XPath("//a[contains(@href," + ringPKey + ")]")).Click();//选择某个戒指
            Thread.Sleep(500);
            for (int i = 0; i < GetRingUs.Length; i++)
            {
                driver_FF.Navigate().Refresh();
                driverConduct.sendRing_2(driver_FF, GetRingUs[i]);
                Thread.Sleep(500);
                driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
                state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
                if (state == "")
                {
                    state = "购买资格/状态:成功";
                }
                else
                {
                    state = "购买资格/状态:" + state;
                }
                logText.AppendText(state + "\n" + "账号：" + GetRingUs[i] + "\n");

            }
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            driver_FF.FindElement(By.LinkText("确认提交")).Click();//确认购买
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                state = "购买资格/状态:可以购买";
            }
            else
            {
                state = "购买资格/状态:" + state;
            }
            logText.AppendText(state + "\n");
            driver_FF.Navigate().Refresh();
            driverConduct.sendRing(driver_FF);//再次提交
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                state = "购买资格/状态:可以购买";
            }
            else
            {
                state = "购买资格/状态:" + state;
            }
            logText.AppendText(state + "\n");

            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();           
            string str = null;
            return str;
        }

        /**********送自己戒指**********/
        string ringForSelfURLExten;
        string ringForSelfRecieveUser;
        string ringForSelfPassword = "123456";
        string ringForSelfCost;
        string ringForSelfPKey;
        string ringForSelfStartmb, ringForSelfQuitmb;
        int ringForSelfStartMB, ringForSelfQuitMB;
        /*****************************/

        //给自己购买戒指，读取输入
        public bool infoRingForSelf()
        {
            ringForSelfRecieveUser = textBox_ID_Ring_Copy.Text;
            if (ringForSelfRecieveUser == "")
            {
                MessageBox.Show("请输入登录ID");
                return false;
            }
            ringForSelfPassword = textBox_Password_Ring_Copy.Text;
            if (ringForSelfPassword == "")
            {
                MessageBox.Show("请输入登录密码");
                return false;
            }
            ringForSelfURLExten = textBox_Url_Ring_Copy.Text;
            if (ringForSelfURLExten == "")
            {
                MessageBox.Show("请输入活动ID");
                return false;
            }
            ringForSelfCost = textBox_Cost_Ring_Copy.Text;
            if (ringForSelfCost == "")
            {
                MessageBox.Show("请输入购买金额");
                return false;
            }
            ringForSelfPKey = textBox_RewardId_Ring_Copy.Text;
            if (ringForSelfPKey == "")
            {
                MessageBox.Show("请输入戒指pKey");
                return false;
            }
            return true;
        }

        //给自己购买戒指流程
        public string ringForSelf()
        {
            
            bool c = infoRingForSelf();
            if (!c)
            {
                return null;
            }
            IWebDriver driver_FF = new FirefoxDriver();
            //跳转页面
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com/show/active/name/RingSign" + ringForSelfURLExten);
            driver_FF.Manage().Window.Maximize();
            driver_FF.FindElement(By.XPath("//a[contains(@href," + ringForSelfPKey + ")]")).Click();//选择某个戒指
            driverConduct.switchToAnotherWindow(driver_FF);//要切换页面
            //切换后登录
            driverConduct.betaShopLogin(ringForSelfRecieveUser, ringForSelfPassword, driver_FF);
            Thread.Sleep(500);
            driverConduct.sendRing(driver_FF);
            Thread.Sleep(500);
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                state = "购买资格/状态:购买成功";
            }
            else
            {
                state = "购买资格/状态:" + state;
            }
            logText.AppendText(state + "\n");
            driver_FF.Navigate().Refresh();
            //进入劲舞团页面商城完成不同类型消费,并输出
            driverConduct.GetCurrentWindowHandle(driver_FF);
            ringForSelfStartmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h2>b.f_red2")).GetAttribute("innerText");//mb余额
            ringForSelfStartMB = Convert.ToInt32(ringForSelfStartmb);
            driverConduct.buySelf1200(driver_FF);
            driverConduct.buySpeaker(ringForSelfRecieveUser, (Convert.ToInt32(ringForSelfCost) - 1300).ToString(), driver_FF);
            driver_FF.Navigate().Refresh();
            ringForSelfQuitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h2>b.f_red2")).GetAttribute("innerText");
            ringForSelfQuitMB = Convert.ToInt32(ringForSelfQuitmb);
            state = "实际消费：" + (ringForSelfStartMB - ringForSelfQuitMB).ToString();
            logText.AppendText(state + "\n");

            driver_FF.Navigate().GoToUrl("http://betashop.9you.com/item/show/pkey/" + ringForSelfPKey);
            driverConduct.sendRing(driver_FF);
            Thread.Sleep(500);
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                state = "购买资格/状态:可以购买";
            }
            else
            {
                state = "购买资格/状态:" + state;
            }
            logText.AppendText(state + "\n");
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            //登录lbtest01消费100返现币，送给usrname，完成购满条件
            driverConduct.betaShopLogin("lbtest01", ringForSelfPassword, driver_FF);
            Thread.Sleep(500);
            driverConduct.GetCurrentWindowHandle(driver_FF);
            ringForSelfQuitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h3>b.f_red2")).GetAttribute("innerText");
            ringForSelfStartMB = Convert.ToInt32(ringForSelfQuitmb);
            driverConduct.buySpeakerInBack(ringForSelfRecieveUser, driver_FF);//买喇叭
            driver_FF.Navigate().Refresh();
            ringForSelfQuitmb = driver_FF.FindElement(By.CssSelector("div.moneyDetailed>h3>b.f_red2")).GetAttribute("innerText");
            ringForSelfQuitMB = Convert.ToInt32(ringForSelfQuitmb);
            Console.WriteLine("实际消费：" + (ringForSelfStartMB - ringForSelfQuitMB) + "返现币   有效消费：100" + "\n");
            driver_FF.FindElement(By.LinkText("退出登录")).Click();
            driver_FF.Navigate().Refresh();
            Thread.Sleep(500);

            driverConduct.betaShopLogin(ringForSelfRecieveUser, ringForSelfPassword, driver_FF);
            driver_FF.Navigate().GoToUrl("http://betashop.9you.com/item/show/pkey/" + ringForSelfPKey);
            driverConduct.sendRing(driver_FF);//购买
            Thread.Sleep(500);
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                state = "购买资格/状态:可以购买";
            }
            else
            {
                state = "购买资格/状态:" + state;
            }
            logText.AppendText(state + "\n");
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            driver_FF.FindElement(By.LinkText("确认提交")).Click();//确认购买
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                state = "购买资格/状态:可以购买";
            }
            else
            {
                state = "购买资格/状态:" + state;
            }
            logText.AppendText(state + "\n");
            driver_FF.Navigate().Refresh();
            driverConduct.sendRing(driver_FF);//再次提交
            driverConduct.SwitchToFrame(driver_FF, "//iframe[contains(@class,'fancybox-iframe')]");
            state = driver_FF.FindElement(By.CssSelector("div.tipBox2>font")).Text;
            if (state == "")
            {
                state = "购买资格/状态:可以购买";
            }
            else
            {
                state = "购买资格/状态:" + state;
            }
            logText.AppendText(state + "\n");
            driver_FF.Navigate().Refresh();
            driver_FF.FindElement(By.LinkText("退出登录")).Click();

            string str = null;
            return str;
        }

    }
}
