using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace VersionControl
{
    #region -------FileLoader-------
    abstract class FileLoader
    {
        protected BoolDelegate downFinish;

        public FileMD5 file;
        public FileStream fileStream;
        public string netDirPath = "";
        public string localDirPath = "";
        public virtual bool isDone
        {
            get
            {
                return _isDone;
            }
        }
        public virtual string error
        {
            get
            {
                return _error;
            }
        }
        /// <summary>
        /// 下载失败后，是否需要尝试下载
        /// </summary>
        public bool NeedLoadAgain
        {
            get { return tryTimes < tryMax; }
        }

        protected string _error = "";
        protected bool _isDone = false;

        /// <summary>
        /// 下载失败后最多尝试次数
        /// </summary>
        protected int tryMax = 3;
        protected int tryTimes = 0;

        public FileLoader(FileMD5 file, string netDirPath, string localDirPath, BoolDelegate downLoadFinish)
        {
            this.file = file;
            this.netDirPath = netDirPath;
            this.localDirPath = localDirPath;
            this.downFinish = downLoadFinish;
        }

        public virtual void StartLoad()
        {
            this._isDone = false;
            if (tryTimes == 0)
            {
                Log.Fatlin("Time:" + Time.time + " Start Loading:" + netDirPath + file.relativePath);
            }
            tryTimes++;
        }

        /// <summary>
        /// 检查下载文件的完整性，完整则返回true
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckIntegrity()
        {
            if (File.Exists(localDirPath + file.relativePath))
            {
                string tStr = MD5Bulider.MDFile(localDirPath + file.relativePath);
                if (tStr == file.MD5str) { return true; }
            }
            return false;
        }

    }
    #endregion -------FileLoader-------

    #region -------WWWLoader-------
    class WWWLoader : FileLoader
    {
        private WWW wwwObj = null;

        public WWWLoader(FileMD5 file, string urlDirPath, string localDirPath, BoolDelegate downLoadFinish)
            : base(file, urlDirPath, localDirPath, downLoadFinish)
        {

        }

        public override void StartLoad()
        {
            base.StartLoad();
            VersionManager.Instance.StartCoroutine(DownloadFile());
        }

        IEnumerator DownloadFile()
        {
            wwwObj = new WWW(netDirPath + file.relativePath);
            yield return wwwObj;
            _isDone = wwwObj.isDone;
            _error = wwwObj.error;
            SaveToLocal();
        }

        /// <summary>
        /// 检查下载文件的完整性，完整则返回true
        /// </summary>
        /// <returns></returns>
        public override bool CheckIntegrity()
        {
            if (wwwObj != null)
            {
                string tStr = MD5Bulider.MDBtyes(wwwObj.bytes);
                if (tStr == file.MD5str)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 保存文件到本地.wsy
        /// </summary>		
        private void SaveToLocal()
        {
            byte[] mbyte = wwwObj.bytes;
            string savePath = localDirPath + file.relativePath;
            string saveDir = Path.GetDirectoryName(savePath);

            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            fileStream = new FileStream(savePath, FileMode.OpenOrCreate);

            if (fileStream.CanWrite)
            {
                fileStream.Write(mbyte, 0, mbyte.Length);
            }
            fileStream.Close();
            DisposeWWW();
        }

        private void DisposeWWW()
        {
            if (wwwObj.assetBundle != null)
            {
                wwwObj.assetBundle.Unload(false);
            }
            wwwObj.Dispose();
        }
    }
    #endregion -------WWWLoader-------

    #region -------WebClientLoader-------
    class WebClientLoader : FileLoader
    {
        public WebClientLoader(FileMD5 file, string urlDirPath, string localDirPath, BoolDelegate downLoadFinish)
            : base(file, urlDirPath, localDirPath, downLoadFinish)
        {

        }

        public override void StartLoad()
        {
            base.StartLoad();
            FileUpDownLoad.Instance.DownLoadFile(OnFileDownLoadCompleted, file.relativePath, netDirPath, localDirPath);
        }

        void OnFileDownLoadCompleted(string error)
        {
            this._isDone = true;
            this._error = error;
        }

    }
    #endregion -------WebClientLoader-------

    #region -------HttpWebRequestLoader-------
    class HttpWebRequestLoader : FileLoader
    {
        public HttpWebRequestLoader(FileMD5 file, string urlDirPath, string localDirPath, BoolDelegate downLoadFinish)
            : base(file, urlDirPath, localDirPath, downLoadFinish)
        {

        }

        public override void StartLoad()
        {
            base.StartLoad();
            VersionManager.Instance.StartCoroutine(DownLoadFile(null));
        }

        IEnumerator DownLoadFile(IProgress progress)
        {
            //要下载的文件在Web服务器上的全路径
            string urlFilePath = netDirPath + file.relativePath;
            //另存为的绝对路径＋文件名
            string savePath = localDirPath + file.relativePath;
            string saveDir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            progress.SetTips(file.relativePath + ": ");

            System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(urlFilePath); //从urlFilePath地址得到一个WEB请求   
            System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse(); //从WEB请求得到WEB响应   
            long totalBytes = myrp.ContentLength; //从WEB响应得到总字节数   
            //Prog.Maximum = (int)totalBytes; //从总字节数得到进度条的最大值   
            System.IO.Stream st = myrp.GetResponseStream(); //从WEB请求创建流（读）   

            FileStream fileStream = new FileStream(savePath, System.IO.FileMode.OpenOrCreate); //创建文件流（写）   
            long totalDownloadedByte = 0; //下载文件大小   
            byte[] by = new byte[1024];
            int osize = st.Read(by, 0, (int)by.Length); //读流   
            while (osize > 0)
            {
                totalDownloadedByte = osize + totalDownloadedByte; //更新文件大小   
                //Application.DoEvents();
                fileStream.Write(by, 0, osize); //写流   
                progress.Update((float)totalDownloadedByte / (float)totalBytes);
                osize = st.Read(by, 0, (int)by.Length); //读流   
                yield return 1;
            }
            fileStream.Close(); //关闭流
            st.Close(); //关闭流
            Log.Fatlin("Download finished!!!^_^");

            this._isDone = true;
        }
    }
    #endregion -------HttpWebRequestLoader-------


    #region -------NetworkLoader-------
    class NetworkLoader : FileLoader
    {
        private string host = "treasure.poqop.com";
        private int port = 8080;
        string loadDir = "/ResControl/";
        NetworkStream networkStream;
        Socket client;
        string uri;
        int n = 0;
        int read = 0;
        float downloadRate = 0;
        uint contentLength;

        public NetworkLoader(FileMD5 file, string urlDirPath, string localDirPath, BoolDelegate downLoadFinish)
            : base(file, urlDirPath, localDirPath, downLoadFinish)
        {

        }

        public override void StartLoad()
        {
            base.StartLoad();
            Download();
        }

        void Download()
        {
            uri = loadDir + file.relativePath;
            n = 0;
            read = 0;
            //        Log.Fatlin("begin download: " + downloadFilename);
            //        Log.Fatlin("file uri: " + uri);

            string query = "GET " + uri.Replace(" ", "%20") + " HTTP/1.1\r\n" +
                            "Host: " + host + "\r\n" +
                            "User-Agent: undefined\r\n" +
                            "Connection: close\r\n" +
                            "\r\n";

            //        Log.Fatlin(query);

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            client.Connect(host, port);



            networkStream = new NetworkStream(client);

            var bytes = Encoding.Default.GetBytes(query);
            networkStream.Write(bytes, 0, bytes.Length);

            var bReader = new BinaryReader(networkStream, Encoding.Default);

            string response = "";
            string line;
            char c;

            do
            {
                line = "";
                c = '\u0000';
                while (true)
                {
                    c = bReader.ReadChar();
                    if (c == '\r')
                        break;
                    line += c;
                }
                c = bReader.ReadChar();
                response += line + "\r\n";
            }
            while (line.Length > 0);

            //        Log.Fatlin(response);

            Regex reContentLength = new Regex(@"(?<=Content-Length:\s)\d+", RegexOptions.IgnoreCase);
            contentLength = uint.Parse(reContentLength.Match(response).Value);

            string savePath = localDirPath + file.relativePath;
            string saveDir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
            VersionManager.Instance.StartCoroutine(DownloadFile(null));
        }

        IEnumerator DownloadFile(IProgress progress)
        {
            byte[] buffer = new byte[4 * 1024];
            int byteCount = buffer.Length;
            Log.Fatlin("DownloadFile" + file.relativePath);
            if (progress != null)
            {
                progress.SetTips(file.relativePath + ": ");
            }
            while (n < contentLength)
            {
                if (networkStream.DataAvailable)
                {
                    read = networkStream.Read(buffer, 0, byteCount);
                    n += read;
                    fileStream.Write(buffer, 0, read);
                }
                downloadRate = (float)n / (float)contentLength;
                if (progress != null)
                {
                    progress.Update(downloadRate);
                }

                Log.Fatlin("downloadRate:" + downloadRate);
                //Log.Fatlin("Downloaded: " + n + " of " + contentLength + " bytes ..." + DownloadRate * 100.0f + "%");

                yield return 1;
            }
            Log.Fatlin("down ok:" + downloadRate);
            fileStream.Flush();
            fileStream.Close();
            client.Close();

            fileStream = null;
            client = null;
            this._isDone = true;

            if (downFinish != null)
            {
                Log.Fatlin("------------downFinish----------");
                downFinish(true);
            }
        }
    }
    #endregion -------NetworkLoader-------

    #region -------私有类FileMD5-------
    public class FileMD5
    {
        public string relativePath;
        public string MD5str;
        /// <summary>
        /// 是否立刻更新
        /// </summary>
        public bool updateNow;

        public FileMD5()
        {
        }


        public bool EqualTo(FileMD5 param)
        {
            if (this.MD5str != param.MD5str || this.relativePath != param.relativePath)
            {
                return false;
            }
            return true;
        }
    }
    #endregion -------私有类FileMD5-------

}