using UnityEngine;
using System.Collections.Generic;

public class ResTool
{
	#region ---------静态接口------------
    public static void GetGroupRes(string[] urls, string progressText, OnGroupResLoaded callback, Progress progress = null)
    {
//        if (progress == null)
//        {
//            progress = ProgressBar.Instance;
//        }
        LoadGroupRes temp = new LoadGroupRes(urls, progress, callback);
        //progress.SetTips(progressText);
    }
	#endregion
 		
	#region ----------LoadGroupRes类:实现一组资源的加载---------------
	///一组资源加载完成的回调.
	public delegate void OnGroupResLoaded (bool loadAll);
	class LoadGroupRes
	{
		///此组资源加载完成回调.
		private OnGroupResLoaded callBack;
		///此组资源的文件路径.
        private List<string> pathList;
		///需要加载的文件总数及已加载数.
        private int count, loaded, successloaded;
		IProgress progress;
			
		public LoadGroupRes(string [] paths,IProgress progress, OnGroupResLoaded callBack)
		{
            this.callBack = callBack;
            this.progress = progress;
            pathList = new List<string> ();
            for (int i = 0, iMax = paths.Length; i < iMax; i++)
            {
                string path = paths[i];
                if (!pathList.Contains(path)) 
                {
                    pathList.Add(path);
                }
            }
            count = pathList.Count;
            loaded = 0;
            successloaded = 0;
            for (int i = 0, iMax = pathList.Count; i < iMax; i++)
            {
                ResManager.Instance.GetRes(pathList[i], ResLoaded);
            }
		}
			
		private void ResLoaded(LoadHelper data)
		{
            if (data.loadError)
            {
                Log.Cly("load error Source:" + data.Url);
            }
            loaded++;
            if (!data.loadError) { successloaded++; }
            float rate = successloaded / (float)count;
            if (progress!=null) 
            {
                progress.Update(rate);
            }

            if (loaded >= count) 
            {
                if (callBack != null)
                {
                    callBack(successloaded >= count);
                }
            }
		}

	}
	#endregion 
   
}
