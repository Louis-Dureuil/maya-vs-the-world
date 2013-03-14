using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14
{
    public class DecisionTree : IDictionary<string, int>
    {
        #region private
        private Dictionary<string, int> weights = new Dictionary<string,int>();
        private string command;
        private int sum;
        #endregion

        public String Command { get { return command; } }

        public void SelectNextCommand()
        {
            command = null;
            int choice = GameCommon.Rand.Next(0, sum);
            foreach (KeyValuePair<string, int> item in weights)
            {
                if (choice < item.Value)
                {
                    command = item.Key;
                    return;
                }
                else
                {
                    choice -= item.Value;
                }
            }
        }

        #region IDictionary
        public void Add(string key, int value)
        {
            weights.Add(key, value);
            sum += value;
        }

        public bool ContainsKey(string key)
        {
            return weights.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return weights.Keys; }
        }

        public bool Remove(string key)
        {
            sum -= weights[key];
            return weights.Remove(key);
        }

        public bool TryGetValue(string key, out int value)
        {
            return weights.TryGetValue(key, out value);
        }

        public ICollection<int> Values
        {
            get { return weights.Values; }
        }

        public int this[string key]
        {
            get
            {
                return weights[key];
            }
            set
            {
                sum -= weights[key];
                weights[key] = value;
                sum += value;
            }
        }

        public void Add(KeyValuePair<string, int> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            weights.Clear();
            sum = 0;
        }

        public bool Contains(KeyValuePair<string, int> item)
        {
            return weights.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return weights.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, int> item)
        {
            sum -= item.Value;
            return weights.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return weights.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return weights.GetEnumerator();
        }
        #endregion
    }
}
