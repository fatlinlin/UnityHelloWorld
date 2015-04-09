using System;
using System.IO;
using System.Net;
using UnityEngine;
using System.ComponentModel;

public class FileUpDownLoad
{   
    public static readonly FileUpDownLoad Instance = new FileUpDownLoad();
    public delegate void DownLoadCompleted(string error);
    public delegate void UpLoadCompleted(string error);

    private DownLoadCompleted onDownLoadCompleted;
    private UpLoadCompleted onUpLoadCompleted;
    private WebClient client;
    /// <summary>
    /// WebClient上传文件至服务器（不带进度条）
    /// </summary>
    /// <param name="fileNameFullPath">要上传的文件（全路径格式）</param>
    /// <param name="strUrlDirPath">Web服务器文件夹路径</param>
    /// <returns>True/False是否上传成功</returns>
    public bool UpLoadFile(string fileNameFullPath, string strUrlDirPath)
    {
        //得到要上传的文件文件名
        string fileName = fileNameFullPath.Substring(fileNameFullPath.LastIndexOf("\\") + 1);
        //新文件名由年月日时分秒及毫秒组成
        string NewFileName = DateTime.Now.ToString("yyyyMMddhhmmss")
            + DateTime.Now.Millisecond.ToString()
                + fileNameFullPath.Substring(fileNameFullPath.LastIndexOf("."));
        //得到文件扩展名
        string fileNameExt = fileName.Substring(fileName.LastIndexOf(".") + 1);
        if (strUrlDirPath.EndsWith("/") == false) strUrlDirPath = strUrlDirPath + "/";
        //保存在服务器上时，将文件改名（示业务需要）
        strUrlDirPath = strUrlDirPath + NewFileName;
        // 创建WebClient实例
        WebClient myWebClient = new WebClient();
        myWebClient.Credentials = CredentialCache.DefaultCredentials;
        // 将要上传的文件打开读进文件流
        FileStream myFileStream = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read);
        BinaryReader myBinaryReader = new BinaryReader(myFileStream);
        try
        {
            byte[] postArray = myBinaryReader.ReadBytes((int)myFileStream.Length);
            //打开远程Web地址，将文件流写入
            Stream postStream = myWebClient.OpenWrite(strUrlDirPath, "PUT");
            if (postStream.CanWrite)
            {
                postStream.Write(postArray, 0, postArray.Length);
            }
            else
            {
                //MessageBox.Show("Web服务器文件目前不可写入，请检查Web服务器目录权限设置！","系统提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            postStream.Close();//关闭流
            return true;
        }
        catch (Exception exp)
        {
            //MessageBox.Show("文件上传失败：" + exp.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }
    }

    /// <summary>
    /// 下载服务器文件至客户端（不带进度条）
    /// 要下载的文件在Web服务器上的全路径= urlDirPath+fileRelativePath
    ///文件保存到本地的全路径= localDirPath+fileRelativePath
    /// </summary>
    /// <param name="fileRelativePath">文件相对路径（如：Res/test.txt）</param>
    /// <param name="urlDirPath">要下载的文件在Web服务器上所在的目录（全路径　如：http://www.dzbsoft.com/testDir/）</param>
    /// <param name="localDirPath">下载到本地存放的目录（位置，本地文件夹）</param>
    /// <returns>True/False是否上传成功</returns>
    public WebClient DownLoadFile(DownLoadCompleted onDownLoadCompleted, string fileRelativePath, string urlDirPath, string localDirPath)
    {
        this.onDownLoadCompleted = onDownLoadCompleted;
        // 创建WebClient实例
        client = new WebClient();
        //要下载的文件在Web服务器上的全路径
        string urlFilePath = urlDirPath + fileRelativePath;
        //另存为的绝对路径＋文件名
        string savePath = localDirPath + fileRelativePath;

        //try
        //{
        //    WebRequest myWebRequest = WebRequest.Create(urlFilePath);
        //}
        //catch (Exception exp)
        //{
        //    Log.Fatlin("DownLoadFile Error:" + exp.Message);
        //    //MessageBox.Show("文件下载失败：" + exp.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}
        try
        {
            client.DownloadFileCompleted += OnFileDownLoadCompleted;
            client.DownloadFileAsync(new Uri(urlFilePath), savePath);
        }
        catch (Exception exp)
        {
            if (onDownLoadCompleted != null) { onDownLoadCompleted(exp.Message); }
            //MessageBox.Show("文件下载失败：" + exp.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        return client;
    }

    void OnFileDownLoadCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if (onDownLoadCompleted != null) { onDownLoadCompleted(e.Error == null ? "" : e.Error.Message); }
        client.Dispose();
    }

    /// <summary>
    /// 下载服务器文件至客户端（带进度条）
    /// 要下载的文件在Web服务器上的全路径= urlDirPath+fileRelativePath
    ///文件保存到本地的全路径= localDirPath+fileRelativePath
    /// </summary>
    /// <param name="fileRelativePath">文件相对路径（如：Res/test.txt）</param>
    /// <param name="urlDirPath">要下载的文件在Web服务器上所在的目录（全路径　如：http://www.dzbsoft.com/testDir/）</param>
    /// <param name="localDirPath">下载到本地存放的目录（位置，本地文件夹）</param>
    /// <param name="progress">进度条IProgress</param>
    /// <returns>True/False是否下载成功</returns>
    public bool DownLoadFile(string fileRelativePath, string urlDirPath, string localDirPath, IProgress progress)
    {
        //要下载的文件在Web服务器上的全路径
        string urlFilePath = urlDirPath + fileRelativePath;
        //另存为的绝对路径＋文件名
        string savePath = localDirPath + fileRelativePath;
        try
        {
            System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(urlFilePath); //从urlFilePath地址得到一个WEB请求   
            System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse(); //从WEB请求得到WEB响应   
            long totalBytes = myrp.ContentLength; //从WEB响应得到总字节数   
            //Prog.Maximum = (int)totalBytes; //从总字节数得到进度条的最大值   
            System.IO.Stream st = myrp.GetResponseStream(); //从WEB请求创建流（读）   
            System.IO.Stream so = new System.IO.FileStream(savePath, System.IO.FileMode.Create); //创建文件流（写）   
            long totalDownloadedByte = 0; //下载文件大小   
            byte[] by = new byte[1024];
            int osize = st.Read(by, 0, (int)by.Length); //读流   
            while (osize > 0)
            {
                totalDownloadedByte = osize + totalDownloadedByte; //更新文件大小   
                //Application.DoEvents();
                so.Write(by, 0, osize); //写流   
                //Prog.Value = (int)totalDownloadedByte; //更新进度条   
                progress.Update((float)totalDownloadedByte / (float)totalBytes);
                osize = st.Read(by, 0, (int)by.Length); //读流   
            }
            so.Close(); //关闭流
            st.Close(); //关闭流
            return true;
        }
        catch
        {
            return false;
        }
    }
}


