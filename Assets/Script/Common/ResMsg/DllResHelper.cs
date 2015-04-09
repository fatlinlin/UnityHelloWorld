using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DllResHelper
{


    public void CheckDllUpdate()
    {
        //TODO:获取服务端dll Md5

        //TODO:遍历 如果客户端dll是否过期或不存在  重新获取


        ResManager.Instance.GetRes("", callback => { });
    }


    public void GetDllRes()
    {
    }


    private string GetMd5ByName(string name)
    {
        return PlayerPrefs.HasKey(name) ? PlayerPrefs.GetString(name) : null;
    }
}
