using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLBS14.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FLBS14.Primitives
{
    public class Image
    {
        #region private
        #region resources
        private ImageManager manager;
        #endregion
        #region src
        string srcFile;
        Rectangle srcRectangle = new Rectangle();
        #endregion
        Texture2D texture;
        #endregion

        public Rectangle SrcRectangle { get { return srcRectangle; } }

        #region src
        public string SrcFile
        {
            get { return srcFile; }
        }

        public int SrcX
        {
            get { return srcRectangle.X; }
            set { srcRectangle.X = value; }
        }

        public int SrcY
        {
            get { return srcRectangle.Y; }
            set { srcRectangle.Y = value; }
        }

        public int SrcW
        {
            get { return srcRectangle.Width; }
            set { srcRectangle.Width = Math.Max(0, value); }
        }

        public int SrcH
        {
            get { return srcRectangle.Height; }
            set { srcRectangle.Height = Math.Max(0, value); }
        }
        #endregion

        public Image(ImageManager manager, string srcFile)
        {
            this.manager = manager;
            this.srcFile = srcFile;
            manager.Load(this);
        }

        public Image(ImageManager manager, Data.Image baseImage)
        {
            this.manager = manager;
            this.srcFile = baseImage.File;
            this.SrcX = baseImage.X;
            this.SrcY = baseImage.Y;
            this.SrcW = baseImage.W;
            this.SrcH = baseImage.H;
            manager.Load(this);
        }

        public void LoadAsynchronously(Data.Image baseImage)
        {
            this.srcFile = baseImage.File;
            this.SrcX = baseImage.X;
            this.SrcY = baseImage.Y;
            this.SrcW = baseImage.W;
            this.SrcH = baseImage.H;
            manager.Load(this);
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

    }
}
