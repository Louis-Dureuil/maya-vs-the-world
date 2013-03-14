using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;

namespace FLBS14.Primitives
{
    public class Animation : IDictionary<Image, double>
    {
        #region private
        private List<Image> images = new List<Image>();
        private List<double> timings = new List<double>();
        private Manager.AnimationManager manager;


        double time;
        bool isLoop = true;
        bool isRunning = true;
        double speed = 1.0;
        int index;

        #endregion

        #region logic

        public bool IsLoop
        {
            get { return isLoop; }
            set { isLoop = value; }
        }

        public bool IsRunning { get { return isRunning; } set { isRunning = value; } }

        public double Speed { get { return speed; } set { speed = value; } }

        public Image CurrentImage { get { return images[index]; } }

        public void Update(GameTime gameTime)
        {
            if (isRunning && images.Count > 0)
            {
                time += gameTime.ElapsedGameTime.TotalSeconds * speed;
                if (time > timings[index])
                {
                    if (index + 1 < timings.Count)
                    {
                        index++;
                    }
                    else
                    {
                        if (IsLoop)
                        {
                            index = 0;
                        }
                        else
                        {
                            IsRunning = false;
                        }
                    }
                    time = 0;
                }
            }
        }
        #endregion

        public Animation(Manager.AnimationManager manager)
        {
            this.manager = manager;
            manager.Load(this);
        }

        public Animation(Manager.AnimationManager manager, Data.Animation baseAnimation, Manager.ImageManager imageManager)
        {
            this.manager = manager;
            manager.Load(this);
            this.IsLoop = baseAnimation.IsLoop;
            foreach (KeyValuePair<string, double> baseImage in baseAnimation.Images)
            {
                Data.Image image = Data.DataManager.Load<Data.Image>(baseImage.Key);
                this.Add(new Image(imageManager, image), baseImage.Value);
            }
        }

        public void LoadAsynchronously(Data.Animation baseAnimation, Manager.ImageManager imageManager)
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                this.isLoop = baseAnimation.IsLoop;
                foreach (KeyValuePair<string, double> baseImage in baseAnimation.Images)
                {
                    Data.Image image = Data.DataManager.Load<Data.Image>(baseImage.Key);
                    this.Add(new Image(imageManager, image), baseImage.Value);
                }
            });
        }


        #region IDictionary

        public void Add(Image key, double value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (images.Contains(key))
            {
                throw new ArgumentException("An argument with the specified key already exists.");
            }
            images.Add(key);
            timings.Add(value);
        }

        public bool ContainsKey(Image key)
        {
            return images.Contains(key);
        }

        public ICollection<Image> Keys
        {
            get { return images; }
        }

        public bool Remove(Image key)
        {
            if (key == null) throw new ArgumentNullException("key");
            int removeIndex = images.IndexOf(key);
            images.RemoveAt(removeIndex);
            timings.RemoveAt(removeIndex);
            return true;
        }

        public bool TryGetValue(Image key, out double value)
        {
            value = 0;
            int getIndex = images.IndexOf(key);
            if (getIndex == -1) return false;
            value = timings[getIndex];
            return true;
        }

        public ICollection<double> Values
        {
            get { return timings; }
        }

        public double this[Image key]
        {
            get
            {
                int getIndex = images.IndexOf(key);
                if (getIndex == -1)
                {
                    throw new KeyNotFoundException();
                }
                return timings[getIndex];
            }
            set
            {
                int getIndex = images.IndexOf(key);
                if (getIndex == -1)
                {
                    throw new KeyNotFoundException();
                }
                timings[getIndex] = value;
            }
        }

        public void Add(KeyValuePair<Image, double> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            images.Clear();
            timings.Clear();
        }

        public bool Contains(KeyValuePair<Image, double> item)
        {
            return images.Contains(item.Key);
        }

        public void CopyTo(KeyValuePair<Image, double>[] array, int arrayIndex)
        {
            for (int i = 0; i < images.Count; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<Image, double>(images[i], timings[i]);
            }
        }

        public int Count
        {
            get { return images.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<Image, double> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<Image, double>> GetEnumerator()
        {
            for (int i = 0; i < images.Count; i++)
            {
                yield return new KeyValuePair<Image, double>(images[i], timings[i]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
