﻿using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    //文章截取前多少个字
    const int contextLength= 1000;

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

    //扮演随机演员进行评论
    public static string getRandomComedyStar()
    {
        string[] comedyStars = {
        "埃迪·墨菲(Eddie Murphy)",
        "詹姆斯·柯登(James Corden)",
        "凯文·哈特(Kevin Hart)",
        "路易·安德森(Louis Anderson)",
        "岳云鹏",
        "德云社相声演员郭德纲",
        "小沈阳",
        "贾玲",
        "赵本山",
        "苏东坡",
        "周星驰",
        "阿米尔·汗(Aamir Khan)",
        "阿克谢·库玛尔(Akshay Kumar)",
        "莫妮克·韦尔什(Monique Welsh)",
        "詹姆斯·柯登(James Corden)",
        "罗恩·阿特金森(Ron Atkinson)",
        "阿列克谢·斯米尔诺夫(Aleksei Smirnov)",
        "阿列克谢·波尔日诺夫(Aleksei Borysenko)",
        "阿列克谢·涅斯特罗夫(Aleksei Nestorov)"
        };

        Random random = new Random();
        string comedyStar = comedyStars[random.Next(0, comedyStars.Length)];

        return comedyStar;
    }


    //函数，参数content,然后打开https://api.openai.com/v1/chat/completions地址，将conent的内容给openAiapi，让他返回评论的内容。
    public static string GetOpenAiComment(string content)
    {
        string responseString = string.Empty;
        try
        {
            //将内容中有"号，全进行转义
            content = content.Replace("\"", "\\\"");
            //将内容中有换行\n号，全进行转义
            content = content.Replace("\n", "\\\n");

            //超过1000个字，按1000字来算；
            if (content.Length > contextLength)
                content = content.Substring(1, contextLength);


            //openAi 访问地址
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.openai.com/v1/completions");

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer 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");
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                //随机生成10到30的整数
                Random random = new Random();
                int randomNumber = random.Next(10, 30);

                //随机获得扮演的角色
                string comedyStar = getRandomComedyStar();
                Console.WriteLine($"你现在扮演：{comedyStar}");

                string json = "{\"prompt\":\"你现在用【" + comedyStar + "】风格，对这篇文章【" + content + "】，用小于" + randomNumber + "字进行评论。不要出现这篇文章的字样；\",\"model\": \"text-davinci-003\",\"max_tokens\": " + (contextLength+200)+"}";
                //获得当前时间，格式输出为20230515104045
                DateTime currentTime = DateTime.Now;
                Console.WriteLine(currentTime.ToString("yyyyMMddHHmmss"));
                string currentTimeStr = currentTime.ToString("yyyyMMddHHmmss");
                File.WriteAllText($"d:/temp/{currentTimeStr}req.json", json);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    responseString = streamReader.ReadToEnd();
                }
                //将responseString转成josn并获得choices节点下的text的值

                dynamic json = JsonConvert.DeserializeObject(responseString);

                responseString = json.choices[0].text;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("获取文章点评！！！！不成功！！！");
        }

        //将可能存在的换行符去掉；
        responseString = responseString.Replace("\n", "");
        return responseString;
    }


    public static void autoInputTextarea(string ReplyMessage, IWebDriver driver)
    {
        // 找到 class="W2TSX_" 的 textarea 元素
        IWebElement textarea = driver.FindElement(By.CssSelector("textarea.W2TSX_"));
        // 在 textarea 内输入内容
        textarea.SendKeys($"{ReplyMessage}");
        Console.WriteLine("文章内容--填充评论内容！");


        //if ReplyMessage的长度小于等于10停10秒，小于等于20大于10，停20秒，大于20小于30停30秒，大于30停40秒，
        if (ReplyMessage.Length <= 10)
        {
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("等待10秒！");
        }
        else if (ReplyMessage.Length > 10 && ReplyMessage.Length <= 20)
        {
            System.Threading.Thread.Sleep(20000);
            Console.WriteLine("等待20秒！");
        }
        else if (ReplyMessage.Length > 20 && ReplyMessage.Length <= 30)
        {
            System.Threading.Thread.Sleep(30000);
            Console.WriteLine("等待30秒！");
        }
        else
        {
            System.Threading.Thread.Sleep(40000);
            Console.WriteLine("等待40秒！");
        }


        /*<div class="_1Jdfvb ufcbR-"><div class="TDvCqd"><textarea class="W2TSX_" placeholder="写下你的评论...">sdfadfsadfsa</textarea><i aria-label="icon: smile" tabindex="-1" class="anticon anticon-smile _2qhU6p"><svg viewBox="64 64 896 896" focusable="false" class="" data-icon="smile" width="1em" height="1em" fill="currentColor" aria-hidden="true"><path d="M288 421a48 48 0 1 0 96 0 48 48 0 1 0-96 0zm352 0a48 48 0 1 0 96 0 48 48 0 1 0-96 0zM512 64C264.6 64 64 264.6 64 512s200.6 448 448 448 448-200.6 448-448S759.4 64 512 64zm263 711c-34.2 34.2-74 61-118.3 79.8C611 874.2 562.3 884 512 884c-50.3 0-99-9.8-144.8-29.2A370.4 370.4 0 0 1 248.9 775c-34.2-34.2-61-74-79.8-118.3C149.8 611 140 562.3 140 512s9.8-99 29.2-144.8A370.4 370.4 0 0 1 249 248.9c34.2-34.2 74-61 118.3-79.8C413 149.8 461.7 140 512 140c50.3 0 99 9.8 144.8 29.2A370.4 370.4 0 0 1 775.1 249c34.2 34.2 61 74 79.8 118.3C874.2 413 884 461.7 884 512s-9.8 99-29.2 144.8A368.89 368.89 0 0 1 775 775zM664 533h-48.1c-4.2 0-7.8 3.2-8.1 7.4C604 589.9 562.5 629 512 629s-92.1-39.1-95.8-88.6c-.3-4.2-3.9-7.4-8.1-7.4H360a8 8 0 0 0-8 8.4c4.4 84.3 74.5 151.6 160 151.6s155.6-67.3 160-151.6a8 8 0 0 0-8-8.4z"></path></svg></i></div><button type="button" class="_1OyPqC _3Mi9q9 _1YbC5u"><span>发布</span></button><button type="button" class="_1OyPqC"><span>取消</span></button></div> 
         * 要求以上脚本，进行模拟点击发布按钮，需要找到_1OyPqC _3Mi9q9 _1YbC5u 的button且 <span>发布</span>地方进行点击操作。使用iwebdriver
         */
        IWebElement publishButton = driver.FindElement(By.XPath("//button[contains(span,'发布')]"));

        //模拟mouse 移动操作
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", publishButton);

        bool enabled = publishButton.Enabled;
        if (enabled)
        {
            //针对下面的代码进行异常保护
            try
            {
                // Code that may cause an exception
                publishButton.Click();
                Console.WriteLine("文章内容--评论成功并发布！");
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine("文章内容--评论成功并发布不成功！");
            }
            finally
            {
                // Clean up code
            }
        }
        else
        {

            Console.WriteLine("文章内容--评论成功并发布不成功！");
        }

    }

    public static void getURLContent(string getALkieURL, IWebDriver driver)
    {
        string html = driver.PageSource;
        string content = "";
        Regex regex = new Regex("<p>(.*?)</p>");
        MatchCollection matches = regex.Matches(html);
        foreach (Match match in matches)
        {
            content += match.Groups[1].Value;
        }
        Console.WriteLine(content);
        Console.WriteLine("文章内容获得成功！");

        string ReplyMessage = GetOpenAiComment(content);
        Console.WriteLine($"获得AI评论内容：{ReplyMessage}");

        autoInputTextarea(ReplyMessage, driver);


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
        Console.WriteLine($"延迟{randomNumber / 1000} 秒！");
        System.Threading.Thread.Sleep(randomNumber);

        //自动评论；
        getURLContent(getALkieURL, driver);

        //IWebElement element = driver.FindElement(By.ClassName("_2VdqdF")).FindElement(By.Role("button"));
        
        //自动点赞  ，窗体大小要注意，有时小了 没有这个button按钮，会出错。
        IWebElement element = driver.FindElement(By.CssSelector("div._2VdqdF[role='button']"));

        if (element != null)
        {
            try
            {
                element.Click();
                Console.WriteLine($"{getALkieURL} 点赞成功！");
            }
            catch
            {
                Console.WriteLine($"{getALkieURL} 点赞不成功！");
            }
        }
        else
            System.Console.WriteLine($"{getALkieURL}没有找到");


    }

    static void Main(string[] args)
    {

        //设置浏览器窗口大小为 1424x1024
        var options = new EdgeOptions();
        options.AddArguments("start-maximized"); // 最大化窗口
        options.AddArguments("window-size=1424,1024"); // 设置窗口大小,把边上点赞按钮显示出来。
        IWebDriver driver = new EdgeDriver(options);

        // 创建ChromeDriver实例
        /*IWebDriver driver = new EdgeDriver();
        *IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        *js.ExecuteScript("window.resizeTo(1624, 1200);");
        */

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

                    giveALike(getALkieURL, driver);//执行点赞和评论！
                }
                sr.Close();
            }
        }

        driver.Quit();
    }




}