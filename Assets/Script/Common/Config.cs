using UnityEngine;
using System.Collections;

public class Config
{

    /// <summary>
    /// 获取场景资源路径.wsy
    /// </summary>		
    public static string GetDllResPath(string name)
    {
        return "Res/DllRes/" + name + ".bytes";
    }
}
