# GiveALlike_JianShu2
这个给某网站进行文章评论的内容；
可以扮演【XXX角色】进行文章【XXXX】评论，内容不超过【xxx】字；


# 几点使用限制：
- 浏览器，使用Edge;
- 如果需要使用openAi，需要自己在GetOpenAiComment 中换成自己的Key ;
- 如果是使用claude，那就需要自己先在slack中开通Claude,然后获得cookie 和 Authorization，以及channelId 

# claude 主要两个方法
## chat.postMessage
我使用两种模式，一种是直接用url = "https://slack.com/api/chat.postMessage?channel=D057HV46G04&text=" + addMessage + "&pretty=1";
和另一种是：url = "https://slack.com/api/chat.postMessage"; 然后使用json ,采用 jsonContent = $"{{\"channel\":\"{channelId}\",\"text\":{addMessage}}}";

## conversations.history
string getDataURL = "https://slack.com/api/conversations.history?channel=D057HV46G04&limit=1";
就是需要先发一下信息，然后再使用conversations.history 获得。
limit 获得最后一条。但是这个claude经常会有废话。他好意给你提醒点内容。

## 这里有是帮助文档：
https://api.slack.com/methods/conversations.history/code

# 参考了1个视频：
https://www.bilibili.com/video/BV1cd4y1f7XW/?spm_id_from=333.337.search-card.all.click&vd_source=85872358ffbb29af2a85dbe8f49b4e0b

