using UnityEngine;
using System.Collections;
using VersionControl;


/// 无形参回调函数委托.wsy	
public delegate void VoidDelegate();
/// int型回调函数委托.wsy
public delegate void IntDelegate(int param);
/// float型回调函数委托.wsy 
public delegate void FloatDelegate(float param);
/// bool型回调函数委托.wsy
public delegate void BoolDelegate(bool param);
/// string型回调函数委托.wsy
public delegate void StringDelegate(string param);
/// object型回调函数委托.wsy
public delegate void ObjectDelegate(object param);

public class GameStart : MonoBehaviour
{
    public static GameStart Instance;

    private void Awake()
    {
        Instance = this;
        //初始化配置资源管理路径
        InitResManager();

        VersionManager.Instance.Init(null, OnFinishUpdate);
        VersionManager.Instance.InitPathToStart(ResManager.Instance.dataPath, ResManager.Instance.netDataPath, Language.BeingCheckResUpdate);
        Log.Fatlin("UITool.Init");
        MineUITool.Init(GameObject.Find("UI_Root"));

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 初始化配置资源管理路径
    /// </summary>
    private void InitResManager()
    {
        string localDataPath = "";
        string webDataPath = "";

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
                //localDataPath = Application.persistentDataPath + "/";
                localDataPath = Application.dataPath + "/../ResControl/";//Application.streamingAssetsPath+ "/";//Application.dataPath + "/../ResControl/";
                webDataPath = "http://treasure.poqop.com:8080/ResControl/";
                break;
            case RuntimePlatform.IPhonePlayer:
                localDataPath = Application.persistentDataPath + "/";
                webDataPath = "http://treasure.poqop.com:8080/ResControl/";
                break;
            case RuntimePlatform.Android:
                localDataPath = Application.persistentDataPath + "/";//Application.streamingAssetsPath+ "/";//Application.persistentDataPath + "/";
                webDataPath = "http://treasure.poqop.com:8080/ResControl/";
                break;

        }
        ResManager.Instance.InitPath(localDataPath, webDataPath);
    }



    private void OnFinishUpdate(bool result)
    {
        Log.Fatlin("OnFinishUpdate:" + result);
    }
}
