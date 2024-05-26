using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClient.Services
{
    public class NavigationParams : INavigationParams
    {
        private Dictionary<string, object> _parameters = new();

        public NavigationParams() {}

        public void Set(string key, object value)
        {
            if (_parameters.ContainsKey(key))
            {
                _parameters[key] = value;
            }
            else
            {
                _parameters.Add(key, value);
            }
        }

        public T Get<T>(string key)
        {
            return (T)_parameters[key];
        }
    }
}
