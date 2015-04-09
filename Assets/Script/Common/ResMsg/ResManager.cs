/**
 * 文件名称：ResManager.cs
 * 简    述：继承于MonoBehaviour，全局单例，资源加载管理器，提供方便的下载方法; 
 * 使用之前请先初始化：ResManager.Init(localDataPath, webDataPath); 其中localDataPath=本地资源存放路径，webDataPath=网上资源存放路径
 * 注意：方法中的参数url为资源相对于localDataPath或webDataPath的路径。
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VersionControl;

/// <summary>
/// 资源加载完成时返回WWW对象的函数委托;
/// 在WWW中自己转换出string, audioClip, texture, assetBundle等各种资源;
/// 注： 在www对象中，除assetBundle资源对象外，其他资源对象（如audioClip，texture）
/// 每被get一次，对象便被创建一次（且每次创建，源数据都会解压缩到内存，从而导致内存持续增加）
/// </summary>
public delegate void OnResLoadOK(LoadHelper self);

#region ----------ResManager---------
public class ResManager : MonoBehaviour
{
    #region -------LoadPriority 资源下载优先级-------
    public enum LoadPriority
    {
        Low = 0,
        Normal,
        High,
        Immediate
    };
    #endregion -------LoadPriority-------

    #region ----------变量---------
    private static ResManager instance;
    public static ResManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Component.FindObjectOfType<ResManager>();
                if (instance == null)
                {
                    if (GameObject.Find("ResManager") != null)
                    {
                        GameObject.Destroy(GameObject.Find("ResManager"));
                    }
                    GameObject go = new GameObject("ResManager");
                    GameObject.DontDestroyOnLoad(go);
                    instance = go.AddComponent<ResManager>();
                }
            }
            return instance;
        }
    }

    private string _netDataPath;

    /// <summary>
    /// 网络数据路径
    /// </summary>
    public string netDataPath
    {
        set
        {
            _netDataPath = value;
            LoadHelper.InitBaseUrl(_dataPath, _netDataPath);
        }
        get { return _netDataPath; }
    }

    private string _dataPath;
    /// <summary>
    /// 当前数据路径
    /// </summary>
    public string dataPath
    {
        get { return _dataPath; }
    }

    /// <summary>
    /// 资源缓存.
    /// </summary>
    private Dictionary<string, LoadHelper> resCache;
    /// <summary>
    /// 等待加载的加载队列.
    /// </summary>
    private Queue<LoadHelper> loadQueueHigh;
    private Queue<LoadHelper> loadQueueNormal;
    private Queue<LoadHelper> loadQueueLow;
    /// <summary>
    /// 可同时进行并发加载的数量;
    /// </summary>
    private int maxConcurrency = 3;
    /// <summary>
    /// 重复调用的时间间隔
    /// </summary>
    private float repeatRate = 0.2f;
    /// <summary>
    /// 正在进行加载的对象;
    /// </summary>
    private List<LoadHelper> loadingList;
    /// <summary>
    /// 正在进行加载的对象中需要移除（已加载完成或加载出错）的对象
    /// </summary>
    private List<LoadHelper> removeList;
    private Dictionary<string, object> webVersion;
    private const string versionXML = "Version.xml";

    #endregion ----------变量---------
    ResManager()
    {
        resCache = new Dictionary<string, LoadHelper>();
        loadQueueHigh = new Queue<LoadHelper>();
        loadQueueNormal = new Queue<LoadHelper>();
        loadQueueLow = new Queue<LoadHelper>();
        loadingList = new List<LoadHelper>();
        removeList = new List<LoadHelper>();
    }
    #region -------MonoBehaviour的内部函数-------
    void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
    #endregion -------MonoBehaviour的内部函数-------


    #region --------对外接口---------
    public void InitPath(string localPath, string netPath, bool controlVersion = false)
    {
        _dataPath = localPath + (localPath.EndsWith("/") ? "" : "/");
        _netDataPath = netPath + (netPath.EndsWith("/") ? "" : "/");
        LoadHelper.InitBaseUrl(_dataPath, _netDataPath);
        if (controlVersion)
        {
            GetRes(_netDataPath + versionXML, OnXmlLoaded);
        }
    }

    private void OnXmlLoaded(LoadHelper self)
    {
        //        webVersion = XmlUtil.Instance.AnalysisXmlString(self.www.text, "object", "relativePath", AnalysisXmlNode);
    }

    /// <summary>
    ///判断是否已经加载完成相关资源;
    /// </summary>
    public bool ContainsRes(string url)
    {
        return resCache.ContainsKey(url);
    }

    /// <summary>
    /// 清除所有的资源缓存;
    /// </summary>
    public void RemoveAllRes(List<string> ignores = null)
    {
        Dictionary<string, LoadHelper> tDic = null;
        foreach (string key in resCache.Keys)
        {
            LoadHelper t = resCache[key];
            if (ignores == null || !ignores.Contains(key))
            {
                if (t.www.assetBundle != null)
                {
                    t.www.assetBundle.Unload(false);
                    t.www.Dispose();
                }
                else
                {
                    if (tDic == null) { tDic = new Dictionary<string, LoadHelper>(); }
                    tDic.Add(key, t);
                }
            }
            else
            {
                if (tDic == null) { tDic = new Dictionary<string, LoadHelper>(); }
                tDic.Add(key, t);
            }
        }
        resCache.Clear();
        if (tDic != null) { resCache = tDic; }
    }

    /// <summary>
    /// 清除url指定的资源缓存;
    ///  仅当卸载AssetBundle包资源时， 调用。（卸载其他类型资源时，直接调用GameObject.Destroy();）
    /// </summary>
    public void RemoveRes(string url)
    {
        if (UnLoadResAsset(url))
        {
            resCache[url].www.Dispose();
            resCache.Remove(url);
        }
    }

    /// <summary>
    /// 批量清除urls指定的资源缓存;
    /// 仅当卸载AssetBundle包资源时， 调用。（卸载其他类型资源时，直接调用GameObject.Destroy();）
    /// </summary>
    public void RemoveRes(string[] urls)
    {
        for (int i = 0, iMax = urls.Length; i < iMax; i++)
        {
            if (UnLoadResAsset(urls[i]))
            {
                //Log.Wsy("UnLoadResAsset:" + urls[i]);
                resCache[urls[i]].www.Dispose();
                resCache.Remove(urls[i]);
            }
        }
    }

    public WWW GetRes(string url)
    {
        if (resCache.ContainsKey(url))
        {
            return resCache[url].www;
        }
        return null;
    }

    public bool GetRes(string url, OnResLoadOK callback)
    {
        return GetRes(url, callback, LoadPriority.Normal);
    }



    /// <summary>
    /// 获取资源WWW 资源对象的方法.
    /// </summary>
    /// <returns>
    /// 是否可以立即获得资源，如果不能可能需要使用者自己先使用替代资源;
    /// </returns>	
    public bool GetRes(string url, OnResLoadOK callback, LoadPriority param)
    {
        if (url != "")
        {
            /////先判断现在是否已经加载相应的资源可以使用;
            if (resCache.ContainsKey(url))
            {
                if (callback != null)
                {
                    callback(resCache[url]);
                }
                return true;
            }
            else
            {
                //先看是否已经开始加载;
                for (int i = 0; i < loadingList.Count; i++)
                {
                    if (loadingList[i].Url == url)
                    {
                        if (callback != null)
                        {
                            if (loadingList[i].onLoadOK == null)
                            {
                                loadingList[i].onLoadOK = new OnResLoadOK(callback);
                            }
                            else
                            {
                                loadingList[i].onLoadOK += callback;
                            }
                        }
                        return false;
                    }
                }

                //未加载资源则放入对应的待加载队列;
                LoadHelper newLoad = new LoadHelper(url, callback);
                newLoad.priority = param;
                switch (param)
                {
                    case LoadPriority.Immediate:
                        RealLoad(newLoad);
                        break;
                    case LoadPriority.High:
                        loadQueueHigh.Enqueue(newLoad);
                        break;
                    case LoadPriority.Normal:
                        loadQueueNormal.Enqueue(newLoad);
                        break;
                    default:
                        loadQueueLow.Enqueue(newLoad);
                        break;
                }
            }//处理不在资源缓存中的资源;

            //启动间隔repeatRate秒重复调用. wsy
            if (!IsInvoking("UpdateLoad"))
            {
                //Log.Wsy(string.Format("ResManager Start {0:f2}s Repeat:current time={1:f2}) ", repeatRate, Time.time));
                InvokeRepeating("UpdateLoad", 0.1f, repeatRate);
            }
        }
        return false;
    }

    /// <summary>
    /// 保存文件到本地.wsy
    /// </summary>		
    public void SaveToLocal(byte[] mbyte, string relativePath)
    {
        string savePath = _dataPath + relativePath;
        string saveDir = Path.GetDirectoryName(savePath);
        //if (File.Exists(savePath))
        //{
        //    File.Delete(savePath);
        //}
        //else 
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        FileStream fstr = new FileStream(savePath, FileMode.Create);
        if (fstr.CanWrite)
        {
            fstr.Write(mbyte, 0, mbyte.Length);
        }
        fstr.Close();
    }


    public bool GetDllRes(string dllName, OnResLoadOK callback)
    {

        //        string ResURL = Config.GetHeroResourcePath(cardId);
        //        LoadHelper helper = new LoadHelper(ResURL);
        //        helper.resType = LoadHelper.RESTYPE.RESOUCE;
        //        helper.loadError = false;
        //        var gameObj = Resources.Load(ResURL);
        //        if (gameObj == null)
        //        {
        //            Log.Cly("Resources没有" + ResURL + "资源");
        //            return GetRes(Config.GetHeroPath(cardId), callback, LoadPriority.Normal);
        //        }
        //        else
        //        {
        //            helper.gameObj = (GameObject)gameObj;
        //            callback(helper);
        //            helper = null;
        //            return true;
        //        }
        Log.Fatlin("get dll path:" + Config.GetDllResPath(dllName));
//        LoadHelper helper = new LoadHelper(Config.GetDllResPath(dllName));
        return GetRes(Config.GetDllResPath(dllName), callback, LoadPriority.High);
    }

    /// <summary>
    /// 保存文件到本地.wsy
    /// </summary>		
    public bool ReadFromLocal(out byte[] byteArray, string relativePath)
    {
        string filePath = _dataPath + relativePath;
        string saveDir = Path.GetDirectoryName(filePath);
        if (File.Exists(filePath))
        {
            FileStream fstr = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader myBinaryReader = new BinaryReader(fstr);
            byteArray = myBinaryReader.ReadBytes((int)fstr.Length);
            fstr.Close();
            return true;
        }
        byteArray = null;
        return false;
    }
    #endregion -------公有方法---------

    #region ------------私有方法---------
    /// <summary>
    ///  卸载内存中url指定的Asset资源
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private bool UnLoadResAsset(string url)
    {
        if (resCache.ContainsKey(url))
        {
            WWW wwwObj = resCache[url].www;
            if (wwwObj.assetBundle != null)
            {
                wwwObj.assetBundle.Unload(false);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 间隔repeatTime秒重复调用;
    /// </summary>
    private void UpdateLoad()
    {
        // Log.Wsy(string.Format ("ResManager {0:f2}s Repeat:current time={1:f2}) ",repeatRate,Time.time));
        LoadHelper loadObj = HasFreeLoader() ? GetFromWaitQueue() : null;
        while (loadObj != null)
        {
            RealLoad(loadObj);
            loadObj = HasFreeLoader() ? GetFromWaitQueue() : null;
        }

        if (loadingList.Count <= 0)
        {
            CancelInvoke("UpdateLoad");
            return;
        }
        removeList.Clear();
        for (int i = 0, iMax = loadingList.Count; i < iMax; i++)
        {
            loadObj = loadingList[i];
            if (loadObj.www.error != null)
            {
                //出现加载错误的处理方法;
                if (loadObj.NeedLoadAgain())
                {
                    loadQueueLow.Enqueue(loadObj);
                }
                else
                {
                    loadObj.loadError = true;
                    Log.Fatlin(string.Format("Load Error:{0}\n{1}", loadObj.www.url, loadObj.www.error));
                    ExecuteLoadOKCallback(loadObj);
                }
                removeList.Add(loadObj);
            }
            else if (loadObj.www.isDone)
            {
                //如果在加载过程中已经完成过一次对象加载;那么这里就不需要进行资源缓存;
                Log.Fatlin("Loaded:" + loadObj.Url + ">>>" + loadObj.baseUrl);
                if (resCache.ContainsKey(loadObj.Url) == false)
                {
                    resCache.Add(loadObj.Url, loadObj);
                }

                if (loadObj.needSave)
                {
                    SaveToLocal(loadObj.www.bytes, loadObj.Url);
                }
                removeList.Add(loadObj);
                ExecuteLoadOKCallback(loadObj);

            }
        }

        for (int i = 0, iMax = removeList.Count; i < iMax; i++)
        {
            loadingList.Remove(removeList[i]);
        }
    }

    private void ExecuteLoadOKCallback(LoadHelper loadObj)
    {
        if (loadObj.onLoadOK != null)
        {
            //try
            //{
            loadObj.onLoadOK(loadObj);
            //}
            //catch (Exception e)
            //{
            // Log.Fatlin("load回调错误:" + loadObj.Url + " 错误信息:" + e.ToString());
            //}
        }
    }

    private bool HasFreeLoader()
    {
        return loadingList.Count < maxConcurrency;
    }

    private void RealLoad(LoadHelper loadhelper)
    {
        if (loadhelper == null)
        {
            return;
        }
        if (resCache.ContainsKey(loadhelper.Url))
        {
            loadhelper.onLoadOK(resCache[loadhelper.Url]);
            return;
        }

        //先看是否已经开始加载;
        for (int i = 0, iMax = loadingList.Count; i < iMax; i++)
        {
            if (loadingList[i].Url == loadhelper.Url)
            {
                loadingList[i].onLoadOK += loadhelper.onLoadOK;
                return;
            }
        }

        loadhelper.StartLoad();
        loadingList.Add(loadhelper);
    }

    private LoadHelper GetFromWaitQueue()
    {
        if (loadQueueHigh.Count > 0)
            return loadQueueHigh.Dequeue();
        if (loadQueueNormal.Count > 0)
            return loadQueueNormal.Dequeue();
        if (loadQueueLow.Count > 0)
            return loadQueueLow.Dequeue();
        return null;
    }

    #endregion ---------私有方法---------

}
#endregion ----------ResManager---------

#region -------Loadhelper-------
/// <summary>
/// Load helper.
/// </summary>
public class LoadHelper
{

    public enum RESTYPE { RESOUCE = 0, WWW }
    /// <summary>
    /// 下载的相对地址.
    /// </summary>
    public string Url
    {
        get { return url; }
    }
    public string baseUrl;
    public OnResLoadOK onLoadOK;
    public WWW www = null;

    /// <summary>
    /// 是否需要保存到本地
    /// </summary>
    public bool needSave = false;
    /// <summary>
    /// 是否加载出错
    /// </summary>
    public bool loadError = false;
    public ResManager.LoadPriority priority = ResManager.LoadPriority.Normal;
    /// <summary>
    /// 下载失败后最多尝试次数;
    /// </summary>
    protected int maxTryTimes = 3;
    protected int tryTimes = 0;
    private FileMD5 file;
    private string fullUrl;
    private string url;
    static string[] baseUrls;
    public RESTYPE resType = RESTYPE.WWW;
    public GameObject gameObj;

    public LoadHelper(string url, OnResLoadOK cbFunc = null)
    {
        this.url = url;
        if (cbFunc != null)
        {
            onLoadOK = new OnResLoadOK(cbFunc);
        }
    }

    public static void InitBaseUrl(string localDirPath, string netDirPath)
    {
        baseUrls = new string[3];
        baseUrls[0] = (Application.platform == RuntimePlatform.Android) ? "" : "file://";
        baseUrls[0] += Application.streamingAssetsPath + "/";
        baseUrls[1] = (localDirPath == Application.streamingAssetsPath + "/") && (Application.platform == RuntimePlatform.Android) ? "" : "file://";
        baseUrls[1] += localDirPath;
        baseUrls[2] = netDirPath;
    }

    public void StartLoad()
    {
        switch (tryTimes)
        {
            case 0:
                baseUrl = baseUrls[0];
                break;
            case 1:
                baseUrl = baseUrls[1];
                if (!File.Exists(baseUrl.Replace("file://", "") + url))
                {
                    baseUrl = baseUrls[2];
                    needSave = true;
                }
                break;
            case 2:
                baseUrl = baseUrls[2];
                needSave = true;
                break;
        }
        if (www != null)
        {
            www.Dispose();
            www = null;
        }
        www = new WWW(baseUrl + url);
        www.threadPriority = ThreadPriority.Low;
        //			int index = url.LastIndexOf (".unity3d");
        //			if (index == -1) {
        //				wwwObj = new WWW (fullUrl);
        //			} else {
        //				int version = UpdateMgr.getVer (url);
        //				wwwObj = WWW.LoadFromCacheOrDownload (fullUrl, version);
        //			}
        Log.Wsy("Start Load:" + baseUrl + url);
        tryTimes++;
    }

    public bool NeedLoadAgain()
    {
        return tryTimes < maxTryTimes;
    }

}
#endregion -------Loadhelper-------



