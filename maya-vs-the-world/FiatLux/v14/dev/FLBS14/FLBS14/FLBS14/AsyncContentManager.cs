using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Threading;

namespace FLBS14
{
    public class AsyncContentManager : ContentManager
    {
        private ContentManager manager;
        static object loadLock = new object();

        public AsyncContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public AsyncContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
        }

        public override T Load<T>(string assetName)
        {
            lock (loadLock)
            {
                return base.Load<T>(assetName);
            }
        }

        public void LoadAsynchronously<T>(string assetName, Action<T> action)
        {
            ThreadPool.QueueUserWorkItem((s) =>
                {
                    T asset = this.Load<T>(assetName);
                    if (action != null)
                    {
                        action.BeginInvoke(asset, null, null);
                    }
                });
        }
    }
}
