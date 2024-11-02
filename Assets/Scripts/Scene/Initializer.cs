using System;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : Singleton<Initializer>
{
    public void Awake()
    {
        GameObject globalgo = GameObject.Find("Global");
        if (globalgo == null)
        {
            GameObject globalprefab = Resources.Load("Global") as GameObject;
            if (globalprefab)
            {
                globalgo = GameObject.Instantiate(globalprefab);
                globalgo.name = "Global";
            }
        }
        if (globalgo!=null)
            DontDestroyOnLoad(globalgo);
    }
}
