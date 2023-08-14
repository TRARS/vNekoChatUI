using System;
using System.Collections.Generic;

namespace Common.WPF.Services
{
    public interface IStreamService
    {
        public void StartStreamingText(string name, string text);
        public void RegisterStremingText(string name, Action<string> act);

        public void StartSuggestion(string name, List<string> text);
        public void RegisterSuggestion(string name, Action<List<string>> act);
    }

    public partial class StreamService : IStreamService
    {
        private Dictionary<string, Action<string>> _botList = new();
        public void StartStreamingText(string name, string text)
        {
            if (_botList.TryGetValue(name, out Action<string>? act))
            {
                act?.Invoke(text);
            }
        }
        public void RegisterStremingText(string name, Action<string> act)
        {
            if (_botList.ContainsKey(name))
            {
                _botList[name] = act;
            }
            else
            {
                _botList.TryAdd(name, act);
            }
        }

        //
        private Dictionary<string, Action<List<string>>> _botSugList = new();
        public void StartSuggestion(string name, List<string> text)
        {
            if (_botSugList.TryGetValue(name, out Action<List<string>>? act))
            {
                act?.Invoke(text);
            }
        }
        public void RegisterSuggestion(string name, Action<List<string>> act)
        {
            if (_botSugList.ContainsKey(name))
            {
                _botSugList[name] = act;
            }
            else
            {
                _botSugList.TryAdd(name, act);
            }
        }
    }
}
