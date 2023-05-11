using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Text.RegularExpressions;

class Program
{

    //生成一个函数表示登录成功，参数为IWebDriver类型，返回一个布尔值
    public static bool LoginSuccess(IWebDriver driver)
    {
        // 打开简书的微信登录页面
        driver.Navigate().GoToUrl("https://www.jianshu.com/users/auth/wechat");

        //给我一个循环判断，driver.PageSource中是否有class=user 和 class=avatar的标签。
        int i = 0;
        do
            if (driver.PageSource.Contains("class=\"user\"") && driver.PageSource.Contains("class=\"avatar\""))
            {


                //暂停5秒

                Console.WriteLine("登录成功！");
                System.Threading.Thread.Sleep(1000);
                break;
            }
            else
            {
                i++;
                System.Threading.Thread.Sleep(5000);
            }
        while (i < 10);

        if (i >= 10)
        {
            Console.WriteLine("登录失败！");
            return false;
        }
        else
            return true;

    }

    //函数，参数content,返回值为布尔函数
    public static bool GetURL(string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            //写一个正则表达式，获取content中,如下
            /*
             * <div class="content">
                <a class="title" target="_blank" href="/p/490ae52f4989">三月最冷的一天</a>
                <p class="abstract">
                  今天气温陡降，冬天说到就到，春姑娘又躲起来了，最低气温零度，最高气温2度。那叫一个冷，风吹在身上，冰冷透心凉。本来今天不想跑步，想偷懒。但儿子要...
                </p>
                <div class="meta">
                    <span class="jsd-meta">
                      <i class="iconfont ic-paid1"></i> 1.4
                    </span>
                  <a class="nickname" target="_blank" href="/u/8723a6e215e9">勇七</a>
                    <a target="_blank" href="/p/490ae52f4989#comments">
                      <i class="iconfont ic-list-comments"></i> 3
            </a>      <span><i class="iconfont ic-list-like"></i> 7</span>
                </div>
              </div>
             */
            //规则是所有标签为class=title 而且 a标签class=title中所有的href内容，并将成存在d:\temp\jianshu2_URL.txt 文件中

            //Regex 需要补什么库，才能使用
            Regex regex = new Regex(@"<a class=""title"" target=""_blank"" href=""(?<url>.*?)"">.*?</a>");
            MatchCollection matches = regex.Matches(content);

            using (StreamWriter sw = new StreamWriter(@"d:\temp\jianshu2_URL.txt"))
            {
                foreach (Match match in matches)
                {
                    sw.WriteLine("https://www.jianshu.com" + match.Groups["url"].Value);
                }
            }
            if (matches.Count > 0)
            {
                Console.WriteLine("d:\\temp\\jianshu2_URL.txt 文件生成成功！");

                return true;
           
            }

            else
                return false;
        }
        else
        {
            return false;
        }
    }



    public static void giveALike(string getALkieURL, IWebDriver driver)
    {
        //使用driver.FindElement方法，pagasource的内容如下
        /*
         *<div class="_2VdqdF" role="button" tabindex="-1" aria-label="给文章点赞"><i aria-label="ic-like" class="anticon"><svg width="1em" height="1em" fill="currentColor" aria-hidden="true" focusable="false" class=""><use xlink:href="#ic-like"></use></svg></i></div>
         */
        //找到 class="_2VdqdF" and role="button" 元素，使用使用他的ic-click事件来操作；

        //随机生成5000到100000的整数
        Random random = new Random();
        int randomNumber = random.Next(50000, 100000);
        //延迟{randomNumber/1000}秒
        Console.WriteLine($"延迟{randomNumber/1000} 秒！");
        System.Threading.Thread.Sleep(randomNumber);
        //IWebElement element = driver.FindElement(By.ClassName("_2VdqdF")).FindElement(By.Role("button"));
        IWebElement element = driver.FindElement(By.CssSelector("div._2VdqdF[role='button']"));
        //判断上文element是否为空
        if (element != null)
        {
            element.Click();
            Console.WriteLine($"{getALkieURL} 点赞成功！");
        }
        else
            System.Console.WriteLine($"{getALkieURL}没有找到");


        //IWebElement element = driver.FindElement(By.ClassName("_2VdqdF")).FindElement(By.AriaLabel("给文章点赞"));
        //element.Click();

    }

    static void Main(string[] args)
    {
        // 创建ChromeDriver实例
        IWebDriver driver = new EdgeDriver();
        if (LoginSuccess(driver)) //登录
        {
            string content = driver.PageSource;
            if (GetURL(content))  //成功获得文件列表；
            {
                //打开d:\temp\jianshu2_URL.txt，并循环读出每一行数据赋给getALkieURL
                string getALkieURL;
                StreamReader sr = new StreamReader("d:\\temp\\jianshu2_URL.txt");
                while ((getALkieURL = sr.ReadLine()) != null)
                {
                    driver.Navigate().GoToUrl(getALkieURL); //打开需要点赞文章；

                    giveALike(getALkieURL, driver);//执行点赞！
                }
                sr.Close();
            }
        }

        driver.Quit();
    }




}