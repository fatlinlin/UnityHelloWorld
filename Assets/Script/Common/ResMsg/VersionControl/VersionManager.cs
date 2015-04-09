/**
* 文件名称：VersionManager.cs
* 简    述：继承于MonoBehaviour，资源更新管理类，在每次游戏启动时调用，检测网络资源更新
* 创建标识：wsy  2013/9/11
 * 添加对下载完成的网络资源文件的完整性验证（即验证MD5）。
 * 对于新版本中本地已存在的资源文件 ，进行MD5验证，避开上次更新失败的影响
 * wsy  2013/10/18
*/

using LitJson;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace VersionControl
{

    /// <summary>
    /// 版本控制
    /// </summary>
    public class VersionManager : MonoBehaviour
    {
        #region -------变量定义-------
        private static VersionManager instance;
        public static VersionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("VersionManager");
                    GameObject.DontDestroyOnLoad(go);
                    instance = go.AddComponent<VersionManager>();
                }
                return instance;
            }
        }
        public BoolDelegate onUpdateFinish;

        private const string versionFileName = "ResVersion.txt";
        private Queue<FileLoader> updateList;
        private FileLoader updatingObj;
        private Dictionary<string, FileMD5> localVersion;
        private Dictionary<string, FileMD5> webVersion;
        private string dataPath;
        private string netDataPath;
        private IProgress progress;

        ///更新结果，true=成功，false=失败;
        bool mResult = false;
        int count = 0;
        string fullPath;
        string progressText;
        #endregion -------变量变量定义-------

        FileLoader CreateFileLoader(FileMD5 file, string netDirPath, string localDirPath)
        {
            return new NetworkLoader(file, netDirPath, localDirPath, DownLoadResFinish); //WWWLoader// NetworkLoader// WebClientLoader  //HttpWebRequestLoader
        }

        #region -------公有函数-------
        public void Init(IProgress progress, BoolDelegate onFinish)
        {
            instance.onUpdateFinish = onFinish;
            instance.progress = progress;
        }

        /// <summary>
        /// 真正启动更新程序
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="netPath"></param>
        public void InitPathToStart(string localPath, string netPath, string progressText)
        {
            this.progressText = progressText;
            if (progress != null)
            {
                progress.SetTips(progressText);
            }
            dataPath = localPath;
            netDataPath = netPath;
            updateList = new Queue<FileLoader>();

            if (File.Exists(dataPath + versionFileName))
            {
                StartCoroutine(LoadFile("file://" + dataPath));
            }
            else
            {
                StartCoroutine(LoadFile(netDataPath));
            }
        }

        IEnumerator LoadFile(string dirPath)
        {
            WWW www = new WWW(dirPath + versionFileName);
            yield return www;
            OnFileLoaded(dirPath, www);
        }

        private void DownLoadResFinish(bool isOk)
        {
            StartNextUpdate();
        }

        private void OnFileLoaded(string dirPath, WWW wwwObj)
        {
            if (dirPath == "file://" + dataPath)
            {
                localVersion = GetDicFromJson(wwwObj.text);
                wwwObj.Dispose();
                StartCoroutine(LoadFile(netDataPath));
            }
            else if (dirPath == netDataPath)
            {
                webVersion = GetDicFromJson(wwwObj.text);
                //判断是否需要替换本地资源版本文件
                if (localVersion!=null&&
                    localVersion.ContainsKey(versionFileName) &&
                    webVersion.ContainsKey(versionFileName) &&
                    localVersion[versionFileName].MD5str != webVersion[versionFileName].MD5str &&
                    !string.IsNullOrEmpty(localVersion[versionFileName].MD5str))
                {
                    SaveToLocal(wwwObj, dataPath, versionFileName);
                }

                //通过对比本地与网上版本xml信息,获得需要更新的资源列表;
                CreateUpdateList();
                //检测更新列表是否已空，是则退出，否则继续加载下一个
                StartNextUpdate();
            }
        }


        /// <summary>
        /// 获取json数据 dic
        /// </summary>
        /// <param name="jsonSt"></param>
        /// <param name="result"></param>
        private Dictionary<string, FileMD5> GetDicFromJson(string jsonSt)
        {
            var result = new Dictionary<string, FileMD5>();
            var fileList = JsonMapper.ToObject<List<FileMD5>>(jsonSt);
            //var js= JsonMapper.ToJson(fileList);
            foreach (var fileMd5 in fileList)
            {
                if (!result.ContainsKey(fileMd5.relativePath))
                {
                    result.Add(fileMd5.relativePath, fileMd5);
                }
            }
            return result;
        }

        /// <summary>
        /// 保存文件到本地.wsy
        /// </summary>		
        private void SaveToLocal(WWW www, string dirPath, string relativePath)
        {
            byte[] mbyte = www.bytes;
            string savePath = dirPath + relativePath;
            string saveDir = Path.GetDirectoryName(savePath);

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            else if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            FileStream fstr = new FileStream(savePath, FileMode.Create);

            if (fstr.CanWrite)
            {
                fstr.Write(mbyte, 0, mbyte.Length);
            }
            fstr.Close();
            DisposeWWW(www);
        }

        private void DisposeWWW(WWW www)
        {
            if (www.assetBundle != null)
            {
                www.assetBundle.Unload(false);
            }
            www.Dispose();
        }

        #endregion -------公有函数-------

        #region -------私有函数-------

        /// <summary>
        /// 检测更新列表是否已空，是则退出，否则继续加载下一个
        /// </summary>
        private void StartNextUpdate()
        {
            if (updateList.Count > 0)
            {
                updatingObj = updateList.Dequeue();
                updatingObj.StartLoad();
            }
            else
            {
                UpdateFinish(true);
            }
        }

        private void UpdateFinish(bool result)
        {
            mResult = result;
            if (mResult)
            {
                //资源更新成功;               
                if (progress != null)
                {
                    progress.Update(1);
                }
            }
            else
            {
                //资源更新失败;
                //在此添加资源更新失败提示;

            }
            //退出更新程序;
            enabled = false;
            Destroy(this.gameObject, 1);
        }

        /// <summary>
        /// 通过对比本地与网上版本信息,获得需要更新的资源列表;
        /// 保留localVersion，删除多余文件用
        /// </summary>
        private void CreateUpdateList()
        {
            updateList.Clear();
            if (localVersion == null)
            {
                foreach (var pair in webVersion)
                {
                    FileMD5 file = pair.Value;
                    if (file.relativePath == versionFileName)
                    {
                        continue;
                    }
                    //判断是否立刻加入UpdateList更新资源
                    CheckAddToUpdateList(file);
                }
            }
            else
            {
                foreach (var pair in localVersion)
                {
                    FileMD5 file = pair.Value;
                    if (file.relativePath == versionFileName)
                    {
                        continue;
                    }
                    if (webVersion.ContainsKey(file.relativePath))
                    {
                        var webitem = webVersion[file.relativePath];
                        //判断是否立刻加入UpdateList更新资源
                        CheckAddToUpdateList(webitem);
                        webVersion.Remove(webitem.relativePath);
                    }
                    else
                    {
                        //删除多余
                        DeleteFile(file.relativePath);
                    }
                }

                //创建新的文件
                foreach (var pair in webVersion)
                {
                    FileMD5 file = pair.Value;
                    if (file.relativePath == versionFileName)
                    {
                        continue;
                    }
                    //判断是否立刻加入UpdateList更新资源
                    CheckAddToUpdateList(file);
                }

            }
            count = updateList.Count;
        }
        /// <summary>
        /// 判断是否立刻加入UpdateList更新资源
        /// </summary>
        /// <param name="fileItem"></param>
        void CheckAddToUpdateList(FileMD5 fileItem)
        {
            //检测指定文件，若指定文件存在并MD5符合，则返回true，否则返回false
            if (!CheckFile(fileItem))
            {
                //删除旧资源
                DeleteFile(fileItem.relativePath);
                //判断是否立刻下载资源
                if (fileItem.updateNow)
                {
                    updateList.Enqueue(CreateFileLoader(fileItem, netDataPath, dataPath));
                }
            }
        }

        /// <summary>
        /// 检测指定文件，若指定文件存在并MD5符合，则返回true，否则返回false
        /// </summary>
        /// <param name="file"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        private bool CheckFile(FileMD5 file)
        {
            if (File.Exists(dataPath + file.relativePath))
            {

                if (!PlayerPrefs.HasKey(file.relativePath))
                {
                    PlayerPrefs.SetString(file.relativePath, file.MD5str);
                    return false;
                }
                if (file.MD5str == PlayerPrefs.GetString(file.relativePath))
                {
                    Log.Fatlin("has:file:" + file.relativePath);
                    return true;
                }
                else
                {
                    Log.Fatlin("outof date");
                    PlayerPrefs.SetString(file.relativePath, file.MD5str);
                    return false;
                }
            }
            return false;
        }

        private void DeleteFile(string relativePath)
        {
            string deletePath = dataPath + relativePath;
            if (File.Exists(deletePath))
            {
                File.Delete(deletePath);
            }
        }
        #endregion -------私有函数-------

        #region -------MonoBehaviour的内部函数-------
        void OnDestroy()
        {
            if (progress != null)
            {
                progress.Close();
            }

            if (onUpdateFinish != null)
            {
                onUpdateFinish(mResult);
            }

            if (updatingObj != null && updatingObj.fileStream != null)
            {
                updatingObj.fileStream.Close();
            }
        }

        void FixedUpdate()
        {
            //UpdateLoad();
        }

        #endregion -------MonoBehaviour的内部函数-------
    }
}