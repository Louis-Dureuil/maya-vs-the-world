using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Threading;

namespace FLBS14.Data
{
    public static class DataManager
    {
        private static object loadLock = new Object();

        public static void Save<T>(T src, string filename)
        {
            lock (loadLock)
            {
                using (FileStream stream = File.Open(filename, FileMode.OpenOrCreate))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stream, src);
                }
            }
        }

        public static T Load<T>(string filename)
        {
            lock (loadLock)
            {
                using (FileStream stream = File.Open(filename, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stream);
                }
            }
        }

        public static void SaveAsynchronously<T>(T asset, string filename) 
        {
            ThreadPool.QueueUserWorkItem((s) =>
                {
                    Save<T>(asset, filename);
                });
        }

        public static void LoadAsynchronously<T>(string filename, Action<T> action)
        {
            ThreadPool.QueueUserWorkItem((s) =>
                {
                    T asset = Load<T>(filename);
                    if (action != null)
                    {
                        action.BeginInvoke(asset, null, null);
                    }
                });
        }
    }
}
