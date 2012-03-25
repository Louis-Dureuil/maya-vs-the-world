/* Lux Engine V0.4.32
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    /// <summary>
    /// Describe a map in a savable/loadable format.
    /// </summary>
    [Serializable]
    public class Map
    {
        //Unique Header with code and version :
        string h = "Fiat Lux Map File Version "; 
        int v = Shared.MAPVERSION;
        
        public string BackgroundName;
        public int Width;
        public int Height;
        public string BackgroundMusicName;
        public List<string> spritesNames = new List<string>();
        public Map(string backgroundName, string backgroundMusicName, int width, int height)
        {
            BackgroundName = backgroundName;
            BackgroundMusicName = backgroundMusicName;
            Width = width;
            Height = height;
        }

        public string GetHeader()
        {
            return h + v.ToString();
        }
    }

    public static class MapManager
    {
        public static void Save(string filename, Map mapToSave)
        {
            FileStream file = new FileStream(filename, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, mapToSave);
            file.Close();
        }

        public static Map Load(string filename)
        {
            FileStream file = new FileStream(filename, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            Map result = (Map)bf.Deserialize(file);
            file.Close();
            return result;
        }

        public static string GetLoadableMaps(string DirectoryName)
        {
            StringBuilder sb = new StringBuilder();
            Map mapToTest;
            foreach (string filename in Directory.EnumerateFiles(DirectoryName))
            {
                try
                {
                    mapToTest = Load(filename);
                    if (mapToTest.GetHeader() == "Fiat Lux Map File Version " + Shared.MAPVERSION.ToString())
                        sb.AppendLine(filename);
                }
                catch { }
            }
            return sb.ToString();
        }
    }
}
