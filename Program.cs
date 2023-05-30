//给某一个人点赞的需求也可以做一下。



using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Anthropic.Net;
using Anthropic.Net.Constants;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

class Program
{
    //文章截取前多少个字
    const int contextLength = 1000;

    //访问chatGPT的URL
    //const string GPTURL = "https://api.openai.com/v1/completions";

    //chatGPT模型不同地址也不同
    const string GPTURL = "https://api.openai.com/v1/chat/completions";

    //每篇文章间隔随机秒数
    const int RandomMinSecond = 20000;

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

    //函数，参数content,返回值为布尔函数,只返回钻大于1的。

    public static bool GetURL1(string content)
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
            //规则是所有标签为class=title 而且 a标签class=title中所有的href内容和获得<span><i class="iconfont ic-list-like"></i> 7</span>中的7赋给like

            Regex regex = new Regex(@"<a\s+class=""title""\s+target=""_blank""\s+href=""(?<href>[^""]+)"".*?<span><i\s+class=""iconfont ic-list-like""></i>\s*(?<number>\d+)\s*</span>");


            //规则是所有标签为class=title 而且 a标签class=title中所有的href内容，并将成存在d:\temp\jianshu2_URL.txt 文件中

            //Regex 需要补什么库，才能使用
            //Regex regex = new Regex((@"<span><i class=""iconfont ic-list-like""></i>(?<like>\d+)</span>.*?<a class=""title"" target=""_blank"" href=""(?<href>.+)"">(?<title>[^公告理事会]+)</a>"));
            MatchCollection matches = regex.Matches(content);

            using (StreamWriter sw = new StreamWriter(@"d:\temp\jianshu2_URL.txt"))
            {
                foreach (Match match in matches)
                {
                    int like = int.Parse(match.Groups["like"].Value);
                    string href = match.Groups["href"].Value;
                    string title = match.Groups["title"].Value;
                    if (like > 1 && !title.Contains("公告") && !title.Contains("理事会"))
                    {
                        sw.WriteLine("https://www.jianshu.com" + href);
                    }
                    // sw.WriteLine("https://www.jianshu.com" + match.Groups["url"].Value);
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
        "郭德纲",
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

    public static async Task<string> GetOpenAiCommentByClaude(string content)
    {
        string responseString = string.Empty;

        //将内容中有"号，全进行转义
        content = content.Replace("\"", "\\\"");
        //将内容中有换行\n号，全进行转义
        content = content.Replace("\n", "\\\n");


        //超过1000个字，按1000字来算；
        if (content.Length > contextLength+2000)
            content = content.Substring(1, contextLength+2000);

        //随机生成10到30的整数
        Random random = new Random();
        int randomNumber = random.Next(10, 30);

        //随机获得扮演的角色
        string comedyStar = getRandomComedyStar();
        Console.WriteLine($"你现在扮演：{comedyStar}");

        string addMessage = "\"用【" + comedyStar + "】的风格进行评论,根据这个文章内容【" + content + "】,请用小于" + randomNumber + "字进行评论,不要出现这篇文章;\"";
        var channelId = "D057HV46G04";
        var jsonContent = $"{{\"channel\":\"{channelId}\",\"text\":{addMessage}}}";
        /*
         * OptanonAlertBoxClosed=2023-05-12T05:42:35.351Z; shown_ssb_redirect_page=1; c={"slack_gpt":1}; b=c80ee5b7039707d6950bab1b5911bec2; d=xoxd-VtOFkpkprfqmfoc7qlVhv4DQow5tPM%2BMpHWjd%2BRUff5INyQ3oOytIYymBPJOa%2BWhqHHBzho10QuxfUPUERc5A%2Fv1u2ANkEgcizJF%2BAsMg%2FYbrzU9yRMJA4PlnUeruxkiCAQm6lqFlb7DkP%2BgW5bN5GahbxHzP1SA9hdKqTsjN3GiXw13BepFWqqnAA%3D%3D; lc=1684484210; shown_download_ssb_modal=1; show_download_ssb_banner=1; no_download_ssb_banner=1; d-s=1685240587; utm=%7B%22utm_source%22%3A%22in-prod%22%2C%22utm_medium%22%3A%22inprod-link_app_settings-user_card-click%22%7D; OptanonConsent=isGpcEnabled=0&datestamp=Tue+May+30+2023+10%3A25%3A35+GMT%2B0800+(%E4%B8%AD%E5%9B%BD%E6%A0%87%E5%87%86%E6%97%B6%E9%97%B4)&version=202211.1.0&isIABGlobal=false&hosts=&consentId=bedb186f-a0ba-4a4c-83a7-f89e3e405188&interactionCount=1&landingPath=NotLandingPage&groups=1%3A1%2C2%3A1%2C3%3A1%2C4%3A1&geolocation=SG%3B&AwaitingReconsent=false
         * https://slack.com/api/chat.postMessage?channel=D057HV46G04&text=Hello&pretty=1
         *Bearer Token xoxc-5246885754470-5266130985009-5290999882867-9f0f2f5355b3aa6a70dc40aee98c0ef374a33cd2c7433f0176ab444ce5e1665a
         */
        HttpClient client = new HttpClient();
        //string url = "https://slack.com/api/chat.postMessage?channel=D057HV46G04&text=" + addMessage + "&pretty=1";
        string url = "https://slack.com/api/chat.postMessage";

        client.DefaultRequestHeaders.Add("Authorization", "Bearer xoxc-5246885754470-5266130985009-5290999882867-9f0f2f5355b3aa6a70dc40aee98c0ef374a33cd2c7433f0176ab444ce5e1665a");
        string cookie = "b=c80ee5b7039707d6950bab1b5911bec2; d=xoxd-VtOFkpkprfqmfoc7qlVhv4DQow5tPM%2BMpHWjd%2BRUff5INyQ3oOytIYymBPJOa%2BWhqHHBzho10QuxfUPUERc5A%2Fv1u2ANkEgcizJF%2BAsMg%2FYbrzU9yRMJA4PlnUeruxkiCAQm6lqFlb7DkP%2BgW5bN5GahbxHzP1SA9hdKqTsjN3GiXw13BepFWqqnAA%3D%3D; lc=1684484210; shown_download_ssb_modal=1; show_download_ssb_banner=1; no_download_ssb_banner=1; d-s=1685240587; utm=%7B%22utm_source%22%3A%22in-prod%22%2C%22utm_medium%22%3A%22inprod-link_app_settings-user_card-click%22%7D; OptanonConsent=isGpcEnabled=0&datestamp=Tue+May+30+2023+10%3A25%3A35+GMT%2B0800+(%E4%B8%AD%E5%9B%BD%E6%A0%87%E5%87%86%E6%97%B6%E9%97%B4)&version=202211.1.0&isIABGlobal=false&hosts=&consentId=bedb186f-a0ba-4a4c-83a7-f89e3e405188&interactionCount=1&landingPath=NotLandingPage&groups=1%3A1%2C2%3A1%2C3%3A1%2C4%3A1&geolocation=SG%3B&AwaitingReconsent=false";
        client.DefaultRequestHeaders.Add("Cookie", cookie);

        Console.WriteLine($"请求的内容为：{url}\n{jsonContent}");

        try
        {
            HttpContent postContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, postContent);
            if (response.IsSuccessStatusCode)
            {
                responseString = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseString);


                bool isTyping = true;
                do
                {

                    Console.WriteLine("等待10秒！,等待返回！");
                    System.Threading.Thread.Sleep(10000);

                    string getDataURL = "https://slack.com/api/conversations.history?channel=D057HV46G04&limit=1";
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer xoxc-5246885754470-5266130985009-5290999882867-9f0f2f5355b3aa6a70dc40aee98c0ef374a33cd2c7433f0176ab444ce5e1665a");
                    //string cookieData = "b=c80ee5b7039707d6950bab1b5911bec2; d=xoxd-VtOFkpkprfqmfoc7qlVhv4DQow5tPM%2BMpHWjd%2BRUff5INyQ3oOytIYymBPJOa%2BWhqHHBzho10QuxfUPUERc5A%2Fv1u2ANkEgcizJF%2BAsMg%2FYbrzU9yRMJA4PlnUeruxkiCAQm6lqFlb7DkP%2BgW5bN5GahbxHzP1SA9hdKqTsjN3GiXw13BepFWqqnAA%3D%3D; lc=1684484210; shown_download_ssb_modal=1; show_download_ssb_banner=1; no_download_ssb_banner=1; d-s=1685240587; utm=%7B%22utm_source%22%3A%22in-prod%22%2C%22utm_medium%22%3A%22inprod-link_app_settings-user_card-click%22%7D; OptanonConsent=isGpcEnabled=0&datestamp=Tue+May+30+2023+10%3A25%3A35+GMT%2B0800+(%E4%B8%AD%E5%9B%BD%E6%A0%87%E5%87%86%E6%97%B6%E9%97%B4)&version=202211.1.0&isIABGlobal=false&hosts=&consentId=bedb186f-a0ba-4a4c-83a7-f89e3e405188&interactionCount=1&landingPath=NotLandingPage&groups=1%3A1%2C2%3A1%2C3%3A1%2C4%3A1&geolocation=SG%3B&AwaitingReconsent=false";
                    //client.DefaultRequestHeaders.Add("Cookie", cookieData);
                    HttpResponseMessage response_Data = await client.GetAsync(getDataURL);
                    if (response_Data.IsSuccessStatusCode)
                    {
                        responseString = await response_Data.Content.ReadAsStringAsync();


                        dynamic json = JsonConvert.DeserializeObject(responseString);

                        responseString = json.messages[0].text;

                        isTyping = responseString.Contains("Typing…");

                        Console.WriteLine($"获得AI评论内容为：{responseString}");


                    }
                    else
                    {
                        Console.WriteLine($"Error: {response_Data.StatusCode} - {response_Data.ReasonPhrase}");
                    }
                } while (isTyping);

            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        } 
        catch (Exception ex)
        {
            Console.WriteLine($"AI评论获取失败！！！！");
            responseString = "手工点赞！";
        }

        //因为会产出提示，就会不成功。
        if (responseString.Contains("Please note:"))
        {
            Console.WriteLine($"AI评论获取失败！！！！");
            responseString = "手工点赞！";
        }

        return responseString;


    }



    //函数，参数content,然后打开https://api.openai.com/v1/chat/completions地址，将conent的内容给openAiapi，让他返回评论的内容。
    public static string GetOpenAiComment(string content)
    {
        string responseString = string.Empty;

        //随机获得扮演的角色
        string comedyStar = "";

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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GPTURL);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer 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");
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                //随机生成10到30的整数
                Random random = new Random();
                int randomNumber = random.Next(10, 30);

                //随机获得扮演的角色
                comedyStar = getRandomComedyStar();
                Console.WriteLine($"你现在扮演：{comedyStar}");

                string json = "";

                if (GPTURL.Contains("chat"))
                {
                   
                    json = "{"
                            + "\"max_tokens\": " + (contextLength + 200) + ","
                            + "\"messages\": ["
                            + "{"
                                + "\"content\": \"用【" + comedyStar + "】的风格进行评论,根据这个文章内容【" + content + "】,请用小于" + randomNumber + "字进行评论,不要出现这篇文章;\","
                                + "\"role\": \"assistant\""
                            + "}"
                        + "],"
                        + "\"model\": \"gpt-3.5-turbo\","
                        + "\"stream\": false"
                    + "}";
                }
                else
                    json = "{\"prompt\":\"请您扮演【" + comedyStar + "】的风格，对这篇文章【" + content + "】，进行积极方面的评论，字数控制在" + randomNumber + "字以内。不要出现【这篇文章】【我学到】【你扮演角色的名字】的字样；\",\"model\": \"text-davinci-003\",\"max_tokens\": " + (contextLength + 200) + "}";
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

                if (GPTURL.Contains("chat"))
                {
                    responseString = json.choices[0].message.content;
                }
                else
                {
                    responseString = json.choices[0].text;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            //comedyStar = "";
            Console.WriteLine("获取文章点评！！！！不成功！！！");
            responseString = "手工点赞！";
        }

        try
        {
            if (responseString is not null)
            {
                //将可能存在的换行符去掉；
                responseString = responseString.Replace("\n", "");
                responseString = responseString.Replace("{", "");
                responseString = responseString.Replace("}", "");
                //如果在评论中，出现演员名称，给他替换成空的内容；
                responseString = responseString.Replace($"{comedyStar}", "");
            }
        }
        catch
        {
            Console.WriteLine($"演员名称{comedyStar}" + "！！！替换不成功！！！");
            responseString = "手工点赞！";
        }

        return responseString;
    }


    public static void autoInputTextarea(string ReplyMessage, IWebDriver driver)
    {
        try
        {
            // 找到 class="W2TSX_" 的 textarea 元素
            IWebElement textarea = driver.FindElement(By.CssSelector("textarea.W2TSX_"));
            // 在 textarea 内输入内容
            textarea.SendKeys($"{ReplyMessage}");
            Console.WriteLine("文章内容--填充评论内容！");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //if ReplyMessage的长度小于等于10停10秒，小于等于20大于10，停20秒，大于20小于30停30秒，大于30停40秒，
        if (ReplyMessage.Length <= 10)
        {
            Console.WriteLine("等待10秒！");
            System.Threading.Thread.Sleep(10000);
        }
        else if (ReplyMessage.Length > 10 && ReplyMessage.Length <= 20)
        {
            Console.WriteLine("等待20秒！");
            System.Threading.Thread.Sleep(20000);
        }
        else if (ReplyMessage.Length > 20 && ReplyMessage.Length <= 30)
        {
            Console.WriteLine("等待30秒！");
            System.Threading.Thread.Sleep(30000);
        }
        else
        {
            Console.WriteLine("等待40秒！");
            System.Threading.Thread.Sleep(40000);
        }


        /*<div class="_1Jdfvb ufcbR-"><div class="TDvCqd"><textarea class="W2TSX_" placeholder="写下你的评论...">sdfadfsadfsa</textarea><i aria-label="icon: smile" tabindex="-1" class="anticon anticon-smile _2qhU6p"><svg viewBox="64 64 896 896" focusable="false" class="" data-icon="smile" width="1em" height="1em" fill="currentColor" aria-hidden="true"><path d="M288 421a48 48 0 1 0 96 0 48 48 0 1 0-96 0zm352 0a48 48 0 1 0 96 0 48 48 0 1 0-96 0zM512 64C264.6 64 64 264.6 64 512s200.6 448 448 448 448-200.6 448-448S759.4 64 512 64zm263 711c-34.2 34.2-74 61-118.3 79.8C611 874.2 562.3 884 512 884c-50.3 0-99-9.8-144.8-29.2A370.4 370.4 0 0 1 248.9 775c-34.2-34.2-61-74-79.8-118.3C149.8 611 140 562.3 140 512s9.8-99 29.2-144.8A370.4 370.4 0 0 1 249 248.9c34.2-34.2 74-61 118.3-79.8C413 149.8 461.7 140 512 140c50.3 0 99 9.8 144.8 29.2A370.4 370.4 0 0 1 775.1 249c34.2 34.2 61 74 79.8 118.3C874.2 413 884 461.7 884 512s-9.8 99-29.2 144.8A368.89 368.89 0 0 1 775 775zM664 533h-48.1c-4.2 0-7.8 3.2-8.1 7.4C604 589.9 562.5 629 512 629s-92.1-39.1-95.8-88.6c-.3-4.2-3.9-7.4-8.1-7.4H360a8 8 0 0 0-8 8.4c4.4 84.3 74.5 151.6 160 151.6s155.6-67.3 160-151.6a8 8 0 0 0-8-8.4z"></path></svg></i></div><button type="button" class="_1OyPqC _3Mi9q9 _1YbC5u"><span>发布</span></button><button type="button" class="_1OyPqC"><span>取消</span></button></div> 
         * 要求以上脚本，进行模拟点击发布按钮，需要找到_1OyPqC _3Mi9q9 _1YbC5u 的button且 <span>发布</span>地方进行点击操作。使用iwebdriver
         */
        try
        {
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
        catch (Exception ex)
        {
            Console.WriteLine("出现异常，文章内容--评论成功并发布不成功！");
        }
        finally
        {

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

        //获得AI评价；
        //string ReplyMessage = GetOpenAiComment(content);
        //string ReplyMessage="";
        //Claude Api形式；

        string ReplyMessage = GetOpenAiCommentByClaude(content).GetAwaiter().GetResult(); 
        Console.WriteLine($"获得AI评论内容：{ReplyMessage}");


        //自动填充评价的内容
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
        int randomNumber = random.Next(RandomMinSecond, RandomMinSecond+50000 );
        //延迟{randomNumber/1000}秒
        Console.WriteLine($"延迟{randomNumber / 1000} 秒！");
        System.Threading.Thread.Sleep(randomNumber);

        //自动点赞  ，窗体大小要注意，有时小了 没有这个button按钮，会出错。【适合无广告及】
        //IWebElement element = driver.FindElement(By.CssSelector("div._2VdqdF[role='button']"));

        //<div class="_3nj4GN" role="button" tabindex="0" aria-label="给文章点赞"><i aria-label="ic-like" class="anticon"><svg width="1em" height="1em" fill="currentColor" aria-hidden="true" focusable="false" class=""><use xlink:href="#ic-like"></use></svg></i><span>赞<!-- -->1</span></div> 
        //使用driver.FindElement方法，获得button,需要加上aria-label="给文章点赞"这个识别

        //改为评论边上的点赞。
        IWebElement element = driver.FindElement(By.CssSelector("div._3nj4GN[role='button'][aria-label='给文章点赞']"));

        if (element != null)
        {
            try
            {
                //  element.Click();
                Console.WriteLine($"{getALkieURL} 点赞成功！");
            }
            catch
            {
                Console.WriteLine($"{getALkieURL} 点赞不成功！");
            }
        }
        else
            System.Console.WriteLine($"{getALkieURL}没有找到");

        //自动评论；
        getURLContent(getALkieURL, driver);




    }

    static void Main(string[] args)
    {

        //设置浏览器窗口大小为 1424x1024
        var options = new EdgeOptions();
        options.AddArguments("start-maximized"); // 最大化窗口
        options.AddArguments("window-size=1424,1024"); // 设置窗口大小,把边上点赞按钮显示出来。
        IWebDriver driver = new EdgeDriver(options);

        if (LoginSuccess(driver)) //登录
        {

            //定义了 IWebDriver driver = new EdgeDriver(options); 后，使用 IJavaScriptExecutor 方法 中进行模仿 mouse向下滚动。
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,750)");

            Console.WriteLine("滚动鼠标后，暂停5秒，等待页面显示！");
            System.Threading.Thread.Sleep(5000);


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
