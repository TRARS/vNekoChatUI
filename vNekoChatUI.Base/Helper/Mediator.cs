using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace vNekoChatUI.Base.Helper
{
    //在单个解决方案中使用，可以随时增加类型
    //在多个解决方案中使用，以插件的形式引用该Mediator类，则最好只往尾部增加类型而不是往中间插入新类型
    public enum MessageType
    {
        WindowClose = 0,
        WindowMinimize,
        WindowMaximize,
        WindowPosReset,           //窗体位置恢复至左上角
    }

    //私有字段/属性/方法
    public sealed partial class Mediator
    {
        private Dictionary<string, List<Action<object?>>> internalListEx = new();
        private Dictionary<MessageType, List<Action<object?>>> internalList = new();

        private static string GetEnumDescription(Enum enumVal)
        {
            System.Reflection.MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
            DescriptionAttribute attribute = CustomAttributeExtensions.GetCustomAttribute<DescriptionAttribute>(memInfo[0]);
            return attribute.Description;
        }
    }

    //限制为单例
    public sealed partial class Mediator
    {
        private static readonly Lazy<Mediator> lazyObject = new(() => new Mediator());
        public static Mediator Instance => lazyObject.Value;
    }

    //使用内置消息类型
    public sealed partial class Mediator
    {
        public void Register(MessageType type, Action<object> callback)
        {
            if (!internalList.ContainsKey(type))
            {
                internalList.Add(type, new List<Action<object?>>() { callback });
            }
            else
            {
                internalList[type].Add(callback);
            }
        }

        public void NotifyColleagues(MessageType type, object? args)
        {
            if (internalList.ContainsKey(type))
            {
                foreach (Action<object?> item in internalList[type])
                {
                    item?.Invoke(args);
                }
            }
        }
    }

    //使用自定义消息类型
    public sealed partial class Mediator
    {
        public void Register(string type, Action<object> callback)
        {
            if (!internalListEx.ContainsKey(type))
            {
                internalListEx.Add(type, new List<Action<object?>>() { callback });
            }
            else
            {
                internalListEx[type].Add(callback);
            }
        }
        public void NotifyColleagues(string type, object? args)
        {
            if (internalListEx.ContainsKey(type))
            {
                foreach (Action<object?> item in internalListEx[type])
                {
                    item?.Invoke(args);
                }
            }
        }
    }
}
