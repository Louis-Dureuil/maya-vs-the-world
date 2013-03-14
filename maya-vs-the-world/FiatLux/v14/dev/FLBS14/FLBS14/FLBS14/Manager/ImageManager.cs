using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLBS14.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FLBS14.Manager
{
    public class ImageManager
    {
 
        List<Image> images = new List<Image>();
        //Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private IServiceProvider contentServiceProvider;
        AsyncContentManager manager;

        public ImageManager(IServiceProvider serviceProvider)
        {
            this.contentServiceProvider = serviceProvider;
            manager = new AsyncContentManager(serviceProvider, "Content");
        }

        public void Load(Image image)
        {
            images.Add(image);
            //if (!textures.ContainsKey(image.SrcFile))
            //{
            //    textures.Add(image.SrcFile, null);
                //manager.LoadAsynchronously<Texture2D>(image.SrcFile, (t) => { textures[image.SrcFile] = t; });
            //}
            manager.LoadAsynchronously<Texture2D>(image.SrcFile, (t) => { image.Texture = t; });
        }

    }
}
