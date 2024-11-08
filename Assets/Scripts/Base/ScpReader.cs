//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Base
{
    public class ScpReader
    {
        string[][] Array = null;
        string tag = "";

        public ScpReader(string name)
        {
            tag = name;

            if ((name.Length >= 3) && (name.Substring(0, 3) == "../"))
            {
                int len = name.Length;
                name = name.Substring(3, len - 3);
            }
            else
            {
                name = "Scp/" + name;
            }

            string text = "";
            if (Application.isEditor)
            {
                string spath = Application.dataPath + "/Resources/" + name + ".csv";
                if (System.IO.File.Exists(spath) == false)
                {
                    return;
                }
                text = File.ReadAllText(spath);
            }
            else
            {
                TextAsset pAsset = (TextAsset)Resources.Load(name);
                if (pAsset == null)
                {
                    return;
                }
                text = pAsset.text;
            }
            LoadFromString(text);
        }

        public ScpReader(TextAsset pAsset, string tag)
        {
            if (pAsset == null)
            {
                return;
            }
            LoadFromString(pAsset.text);

        }

        public ScpReader(string text, string tag)
        {
            LoadFromString(text);
        }

        private void LoadFromString(string text)
        {

            //读取每一行的内容
            string[] lineArray = text.Split("\r"[0]);

            //创建二维数组
            Array = new string[lineArray.Length][];

            //把csv中的数据储存在二位数组中
            for (int i = 0; i < lineArray.Length; i++)
            {
                Array[i] = lineArray[i].Split(","[0]);
                if (Array[i].Length > 0)
                {
                    string s = Array[i][0];
                    if (s.Length > 0 && s[0] == '\n')
                    {
                        s = s.Substring(1);
                    }
                    Array[i][0] = s;
                }
            }


        }

        public void Dispose()
        {
            Array = null;
        }

        public int GetRecordCount()
        {
            if (Array == null)
                return 0;
            return Array.GetLength(0) - 1;
        }

        public int GetRecordCount(int dwRow)
        {
            if (Array == null)
                return 0;

            if (dwRow + 1 >= Array.GetLength(0))
            {
                return 0;
            }
            return Array[dwRow + 1].Length;
        }

        public int GetFieldCount()
        {
            if (Array == null)
                return 0;
            if (Array.Length < 1)
                return 0;
            return Array[0].Length;
        }

        public string GetFieldDesc(int dwCol)
        {
            if (Array == null)
                return "";
            if (dwCol >= Array[0].Length)
            {
                return "";
            }
            return Array[0][dwCol];
        }

        public object GetObject(int dwRow, int dwCol)
        {
            if (dwRow + 1 >= Array.GetLength(0))
            {
                return null;
            }
            if (dwCol >= Array[dwRow + 1].Length)
            {
                return null;
            }

            string ob = Array[dwRow + 1][dwCol];
            if (ob != null)
            {
                object value;
                Type nValType = StringConverter.ConvertString(ob, out value);
                if (nValType == typeof(float)
                    || nValType == typeof(double))
                {
                    float result = Convert.ToSingle(ob.ToString());
                    return result;
                }
                if (nValType == typeof(byte)
                    || nValType == typeof(short)
                    || nValType == typeof(int))
                {
                    int result = Convert.ToInt32(ob.ToString());
                    return result;
                }
                else
                {
                    return ob;
                }
            }

            return null;
        }

        public float GetFloat(int dwRow, int dwCol, float fDef)
        {
            if (dwRow + 1 >= Array.GetLength(0))
            {
                return fDef;
            }
            if (dwCol >= Array[dwRow + 1].Length)
            {
                return fDef;
            }

            float result = fDef;

            string ob = Array[dwRow + 1][dwCol];
            if (ob != null)
            {
                object value;
                Type nValType = StringConverter.ConvertString(ob, out value);
                if (nValType == typeof(float)
                        || nValType == typeof(double))
                {
                    result = Convert.ToSingle(ob.ToString());
                    return result;
                }
                else if (nValType == typeof(byte)
                    || nValType == typeof(short)
                    || nValType == typeof(int))
                {
                    result = (float)Convert.ToInt32(ob.ToString());
                    return result;
                }
                else
                {
                    //Debug.Log ("GetInt failed dwRow:" + dwRow + "dwCol: " + dwCol);
                    return fDef;
                }
            }

            return fDef;
        }


        public int GetInt(int dwRow, int dwCol, int nDef)
        {
            if (dwRow + 1 >= Array.GetLength(0))
            {
                return nDef;
            }
            if (dwCol >= Array[dwRow + 1].Length)
            {
                return nDef;
            }

            int result = nDef;

            string ob = Array[dwRow + 1][dwCol];
            if (ob != null)
            {
                object value;
                Type nValType = StringConverter.ConvertString(ob, out value);
                if (nValType == typeof(byte)
                        || nValType == typeof(short)
                        || nValType == typeof(int))
                {
                    result = Convert.ToInt32(ob.ToString());
                    return result;
                }
                else if (nValType == typeof(float)
                    || nValType == typeof(double))
                {
                    result = (int)Convert.ToSingle(ob.ToString());
                    return result;
                }
                else
                {
                    //Debug.Log ("GetInt failed dwRow:" + dwRow + "dwCol: " + dwCol);
                    return nDef;
                }
            }

            return nDef;
        }

        public string GetString(int dwRow, int dwCol, string strVal)
        {
            if (dwRow + 1 >= Array.GetLength(0))
            {
                return strVal;
            }
            if (dwCol >= Array[dwRow + 1].Length)
            {
                return strVal;
            }

            string ob = Array[dwRow + 1][dwCol];
            if (ob != null)
            {
                return ob;
            }
            return strVal;
        }
    }


}