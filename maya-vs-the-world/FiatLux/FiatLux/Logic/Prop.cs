/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace FiatLux.Logic
{
  
    /// <summary>
    /// Handle the fighter's numerical values, such as statistics or caracteristics.
    /// </summary>
    public class Prop
    {
        Dictionary<string, int> indexes;
        List<int> maxValues;
        List<bool> haveMax;
        List<int> values;
        List<string> names;
        List<string> convNames;
        
        public void AddConvNames(string[] convNames)
        {
            this.convNames = new List<string>(convNames);
            foreach (string Name in convNames)
            {
                this.convNames.Add(Name);
            }
        }

        #region cstr
        public Prop(string[] Names, string[] convNames)
        {
            indexes = new Dictionary<string, int>(Names.Length);
            maxValues = new List<int>(Names.Length);
            haveMax = new List<bool>(Names.Length);
            names = new List<string>(Names.Length);
            values = new List<int>(Names.Length);
            AddConvNames(convNames);
            foreach (string Name in Names)
            {
                Add(Name);
            }
        }

        public Prop(string[] Names, string[] convNames, Dictionary<string, bool> HaveMax)
        {
            indexes = new Dictionary<string, int>(Names.Length);
            maxValues = new List<int>(Names.Length);
            haveMax = new List<bool>(Names.Length);
            names = new List<string>(Names.Length);
            values = new List<int>(Names.Length);
            AddConvNames(convNames);
            foreach (string Name in Names)
                try
                {
                    Add(Name, 0, HaveMax[Name]);
                }
                catch
                {
                    Add(Name);
                }
        }

        public Prop(string[] Names, string[] convNames, Dictionary<string, int> InitialValues)
        {
            indexes = new Dictionary<string, int>(Names.Length);
            maxValues = new List<int>(Names.Length);
            haveMax = new List<bool>(Names.Length);
            names = new List<string>(Names.Length);
            values = new List<int>(Names.Length);
            AddConvNames(convNames);
            foreach (string Name in Names)
                try
                {
                    Add(Name, InitialValues[Name], false);
                }
                catch
                {
                    Add(Name);
                }
                
        }

        public Prop(string[] Names, string[] convNames,Dictionary<string, int> InitialValues, Dictionary<string, bool> HaveMax)
        {
            indexes = new Dictionary<string, int>(Names.Length);
            maxValues = new List<int>(Names.Length);
            haveMax = new List<bool>(Names.Length);
            names = new List<string>(Names.Length);
            values = new List<int>(Names.Length);
            AddConvNames(convNames);
            foreach (string Name in Names)
                try
                {
                    Add(Name, InitialValues[Name], HaveMax[Name]);
                }
                catch
                {
                    if (InitialValues.ContainsKey(Name))
                        Add(Name, InitialValues[Name], false);
                    else if (HaveMax.ContainsKey(Name))
                        Add(Name, 0, HaveMax[Name]);
                    else
                        Add(Name);
                }
        }
        #endregion
        public int IndexOf(string Name)
        {
            try
            {
                return indexes[Name];
            }
            catch
            {
                throw new Exception("The game tried to access the value: " + Name + ", which is not valid.");
            }
        }

        

        void Add(string Name)
        {
            Add(Name, 0, false);
        }

        void Add(string Name, int InitialValue, bool HasMax)
        {
            indexes.Add(Name, values.Count);
            maxValues.Add(InitialValue);
            haveMax.Add(HasMax);
            values.Add(InitialValue);
            names.Add(Name);
        }

        public int this[string Name]
        {
            get {
                try
                {
                    return this[indexes[Name]];
                }
                catch
                {
                    throw new Exception("The game tried to access the value: " + Name + ", which is not valid.");
                }
            }
            set
            {
                try
                {
                  this[indexes[Name]] = value;
                }
                catch
                {
                    throw new Exception("The game tried to access the value: " + Name + ", which is not valid.");
                }
            }
        }

        public int this[int Index, bool Max]
        {
            get
            {
                if (Max)
                    try
                    {
                        return maxValues[Index];
                    }
                    catch { throw new Exception("The game tried to access the value at Index: " + Index + ", which is not valid."); }
                else
                    return this[Index];
            }
            set
            {
                if (Max)
                    try
                    {
                        maxValues[Index] = value;
                        if (haveMax[Index])
                        {
                            this[Index] = this[Index];
                        }
                        else
                        {
                            this[Index] = value;
                        }
                    }
                    catch { throw new Exception("The game tried to access the value at Index: " + Index + ", which is not valid."); }
                else
                    this[Index] = value;
            }
        }

        public int this[string Name, bool Max]
        {
            get
            {
                try { return this[indexes[Name], Max]; }
                catch { throw new Exception("The game tried to access the value: " + Name + ", which is not valid."); }
            }
            set
            {
                try { this[indexes[Name], Max] = value; }
                catch { throw new Exception("The game tried to access the value: " + Name + ", which is not valid."); }
            }
        }

        public int this[int Index]
        {
            get
            {
                try
                {
                    return values[Index];
                }
                catch
                {
                    throw new Exception("The game tried to access the value at Index: " + Index.ToString() + ", which is not valid.");
                }
            }
            set
            {
                try
                {
                    if (haveMax[Index])
                        values[Index] = Math.Max(0, Math.Min(maxValues[Index], value));
                    else
                        values[Index] = Math.Max(0, value);
                }
                catch
                {
                    throw new Exception("The game tried to access the statistic of value: " + Index.ToString() + ", which is not valid.");
                }
            }
        }

        public bool HaveMax(int Index)
        {
            return haveMax[Index];
        }

        public bool HaveMax(string Name)
        {
            return haveMax[indexes[Name]];
        }

        public void ToMax(int Index)
        {
            this[Index] = maxValues[Index];
        }

        public void ToMax(string Name)
        {
            this[Name] = maxValues[indexes[Name]];
        }

        #region ToString

        public string ToString(int StatIndex)
        {
            if (convNames != null)
                return convNames[StatIndex] + ": " + this[StatIndex].ToString();
            else
                return names[StatIndex] + ": " + this[StatIndex].ToString();
        }

        public string ToString(int StatIndex, bool LongForm)
        {
            string name;
            if (convNames != null)

                name = convNames[StatIndex];
            else
                name = names[StatIndex];
            if (LongForm)
                return name + ": " + this[StatIndex].ToString() + "/" + maxValues[StatIndex];
            else return ToString(StatIndex);
        }

        public string ToString(string StatName, bool LongForm)
        {
            return this.ToString(indexes[StatName], LongForm);
        }

        public string ToString(bool LongForm)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                result.AppendLine(this.ToString(i, LongForm));
            }
            return result.ToString();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                result.AppendLine(this.ToString(i));
            }
            return result.ToString();
        }
        #endregion
    }
}
