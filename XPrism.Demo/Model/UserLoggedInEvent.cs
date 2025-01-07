using XPrism.Core.Events;

namespace XPrism.Demo.Model;

// 定义事件的数据类型
public class UserInfo
{
    public string Username { get; set; }
    public string Role { get; set; }
}

public class UserLoggedInEvent : PubSubEvent<UserInfo> { }