using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections;

public class DownloadUpdatePanelScript : MonoBehaviour
{

    public static DownloadUpdatePanelScript Init = null;



    /// <summary>
    /// 加载场景信息类
    /// </summary>
    private AsyncOperation mAsyncOperation;

    void Awake()
    {
        Init = this;
    }

    string host = "42.62.14.55";
    string uri;
    uint contentLength;
    int n = 0;
    int read = 0;
    float DownloadRate = 0;

    private string webPort = "9999";

    NetworkStream networkStream;
    FileStream fileStream;
    Socket client;

    void Start()
    {
        //StartCoroutine(CheckVersion());
        //Log.Fatlin(LoadScene("uEldgnNST1S1YLtnd7wb3w==.unity3d"));

        var subdirNames = new[] { "Music", "Scene", "clothes" };
        foreach (string subdirName in subdirNames.Where(subdirName => Directory.Exists(FileSytemPath() + subdirName) == false))
        {
            Directory.CreateDirectory(FileSytemPath() + subdirName);
        }

    }

    IEnumerator CheckVersion()
    {
        var www = new WWW("http://" + host + ":" + webPort + "/load/checksum.txt");
        yield return www;
//        Log.Fatlin(www.text);

        var checksumNet = www.text;

        var checkResult = CheckFile(checksumNet);
//        Log.Fatlin("Check Result: " + checkResult.ToString());
    }

    private static string FileCheckSum(string assetbundlePath)
    {
        MD5 _hash = MD5.Create();

        if (File.Exists(assetbundlePath) == false)
        {
            return "";
        }

        var bundle = assetbundlePath;
        StreamReader sr = new StreamReader(bundle);
        //string.Concat(Application.dataPath, "/../", _pathBundle));
        byte[] _filehash = _hash.ComputeHash(sr.BaseStream);
        sr.Close();
        string checksum = Convert.ToBase64String(_filehash);
        checksum = checksum.Replace("/", "$");
//        Log.Fatlin("checksum: " + checksum);
        return checksum;
    }

    void Download(string FileName)
    {
		var loadDir = "/load/";
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor) {
			loadDir = "/LoadiOS/";
		}
        uri = loadDir + FileName;
        n = 0;
        read = 0;
        var downloadFilename = FileName;
//        Log.Fatlin("begin download: " + downloadFilename);
//        Log.Fatlin("file uri: " + uri);

        string query = "GET " + uri.Replace(" ", "%20") + " HTTP/1.1\r\n" +
                        "Host: " + host + "\r\n" +
                        "User-Agent: undefined\r\n" +
                        "Connection: close\r\n" +
                        "\r\n";

//        Log.Fatlin(query);

        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        client.Connect(host, 9999);

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

        var finalPath = FileSytemPath() + downloadFilename;
        var tmpPath = finalPath + ".tmp";

        fileStream = new FileStream(tmpPath, FileMode.Create, FileAccess.Write, FileShare.None);
        StartCoroutine(DownloadFile(tmpPath, finalPath));
    }

    IEnumerator DownloadFile(string tmpPath, string finalPath)
    {
        byte[] buffer = new byte[4 * 1024];
        int byteCount = buffer.Length;

        while (n < contentLength)
        {
            if (networkStream.DataAvailable)
            {
                read = networkStream.Read(buffer, 0, byteCount);
                n += read;
                fileStream.Write(buffer, 0, read);
            }
            DownloadRate = (float)n / (float)contentLength;
//            Log.Fatlin("Downloaded: " + n + " of " + contentLength + " bytes ..." + DownloadRate * 100.0f + "%");
            if(_slider!=null)
            _slider.sliderValue = DownloadRate;
            yield return 1;
        }
        DownloadRate = 1f;

        fileStream.Flush();
        fileStream.Close();

        client.Close();

        fileStream = null;
        client = null;

//        Log.Fatlin("Download finished!!!^_^");

        File.Move(tmpPath, finalPath);
        //		
        //		var filePath = FileLocalPath()+"Parkour21.unity3d";
        //		
        //		var www = WWW.LoadFromCacheOrDownload(filePath, 1);
        //		yield return www;
        //		
        //		Application.LoadLevel("Parkour2.1");
        if (_finishAction!=null)
        _finishAction();
    }

    string FileLocalPath()
    {
		var filePath = "file://" + System.IO.Directory.GetCurrentDirectory()+ "\\Assetbundles\\";

        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = "file:///mnt/sdcard/Assetbundles/";
		}				
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			filePath = "file://"+Application.persistentDataPath + "\\Assetbundles\\";
		}
//		Log.Fatlin("File Loacal Path: "+filePath);
//		if (System.IO.Directory.Exists (filePath) == false) 
//		{
//			Directory.CreateDirectory (filePath);
//		}
        return filePath;
    }

    string FileSytemPath()
    {
		var filePath = System.IO.Directory.GetCurrentDirectory() + "\\Assetbundles\\";

        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = "/mnt/sdcard/Assetbundles/";
        }
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			filePath = Application.persistentDataPath + "\\Assetbundles\\";
		}
//		Log.Fatlin("File Loacal Path: "+filePath);
		if (System.IO.Directory.Exists (filePath) == false) 
		{
			Directory.CreateDirectory (filePath);
		}
        return filePath;
    }

    /// <summary>
    /// 判断文件是否存在，不存在自动下载
    /// </summary>
    /// <param name="FileName">文件名</param>
    /// <returns></returns>
    public bool LoadScene(string FileName, string fileS, Action finishAction)
    {

        var filePath = FileSytemPath() + fileS + FileName;
//        Log.Fatlin("load file path: " + filePath);
//        NGUILog.Fatlin("I want file: " + FileName);

        if (Directory.Exists(FileSytemPath()) == false)
        {
            Directory.CreateDirectory(FileSytemPath());

            _finishAction = finishAction;
        }

        bool existedFile = false;

        string[] files = null;
        if (String.IsNullOrEmpty(fileS))
        {
             files = Directory.GetFiles(FileSytemPath());
        }
        else
        {
             files = Directory.GetFiles(FileSytemPath()+fileS);
        }
        foreach (var file in files)
        {
            if (file.Contains(FileName) && (file.Contains(".tmp") == false))
            {
                finishAction();
                existedFile = true;
            }
        }
        if (existedFile)
        {
            return true;
        }
        else
        {
            if (String.IsNullOrEmpty(fileS))
                Download(FileName);
            else
                Download(fileS + FileName);

            _finishAction = finishAction;
            return false;
        }
    }

    private UISlider _slider;
    private Action _finishAction;
    /// <summary>
    /// 判断文件是否存在，不存在自动下载
    /// </summary>
    /// <param name="FileName">文件名</param>
    /// <returns></returns>
    public bool LoadScene(string FileName, UISlider slider, Action finishAction)
    {
        var filePath = FileSytemPath() + FileName;
//        Log.Fatlin("load file path: " + filePath);

//        NGUILog.Fatlin("I want file: " + FileName);

        if (Directory.Exists(FileSytemPath()) == false)
        {
            Directory.CreateDirectory(FileSytemPath());

            _slider = slider;
            _finishAction = finishAction;
            NGUITools.SetActive(_slider.gameObject, true);
            _slider.sliderValue = 0f;
            Download(FileName);
            return false;
        }

        bool existedFile = false;
        var files = Directory.GetFiles(FileSytemPath());
        foreach (var file in files)
        {
            if (file.Contains(FileName) && (file.Contains(".tmp") == false))
            {
                finishAction();
                return true;
                existedFile = true;
            }
        }
        //        if (existedFile)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        _slider = slider;
        _finishAction = finishAction;
        NGUITools.SetActive(_slider.gameObject, true);
        _slider.sliderValue = 0f;
        Download(FileName);
        return false;
        //        }
    }
    //    public bool LoadScene()
    //    {
    //        StartCoroutine(CheckVersion());
    //    }

    /// <summary>
    /// 扩展名
    /// </summary>
    private string extensionName = ".unity3d";

    /// <summary>
    /// 判断文件是否存在，不存在自动下载
    /// </summary>
    /// <param name="FileName">文件名</param>
    /// <returns></returns>
    public bool CheckFile(string checksum)
    {
        var filePath = FileSytemPath() + checksum + extensionName;
//        Log.Fatlin("load file path: " + filePath);

        if (checksum == FileCheckSum(filePath))
        {
            return true;
        }

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        else
        {
            if (Directory.Exists(FileSytemPath()) == false)
            {
                Directory.CreateDirectory(FileSytemPath());
            }
        }
        Download(checksum + extensionName);
        return false;
    }

    /// <summary>
    /// 获取下载进度
    /// </summary>
    /// <returns></returns>
    public float GetDownloadRate()
    {
        return DownloadRate;
    }

    /// <summary>
    /// 是否加载完场景
    /// </summary>
    /// <returns></returns>
    public bool GetLoadRate()
    {
        if (mAsyncOperation == null)
            return false;
        return mAsyncOperation.isDone;
    }

    /// <summary>
    /// 打开场景
    /// </summary>
    /// <param name="FileName">文件名</param>
    /// <param name="levelName">场景名</param>
    public void OpenLevel(string FileName, string levelName)
    {
        StartCoroutine(openLevel(FileName, levelName));
    }

    IEnumerator openLevel(string FileName, string levelName)
    {
        var filePath = FileLocalPath() + FileName;
        var www = WWW.LoadFromCacheOrDownload(filePath, 1);
        yield return www;
        www = null;
        mAsyncOperation = Application.LoadLevelAdditiveAsync(levelName);
    }

}
