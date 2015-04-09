/**
 * 文件名称：XmlManager.cs
 * 简    述：Xml配置表资源管理类.
 * 注意:此处要求xml是子节点名为"object"的一维表.
 * 创建标识：wsy 2013/3/28
 */
using UnityEngine;
using System.Collections.Generic;
	
public class XmlManager 
{
    public static readonly XmlManager Instance = new XmlManager();		
    /// object型回调函数委托.wsy
    public delegate void GetVODelegate(object param);
		
	/// 以xml路径为Key值,存储各xml用以取VO的属性名.	wsy
	private Dictionary<string,string > dicKeyAttr;
	/// 以xml路径为Key值,存储各xml的解析函数.	wsy
    private Dictionary<string, XmlUtil.AnalysisXmlNode> dicReadFunc;
	/// 以xml路径为Key值,存储解析各xml所得的数据vo库.	wsy 
	private Dictionary<string,Dictionary<string, object>> dicVO;
	/// 以xml路径为Key值,存储当xml文件还未加载时取xml数据的回调.	wsy 
	private  Dictionary<string, List<GetVORecord>> dicCallBack;
		
	private XmlManager ()
	{
		dicKeyAttr = new Dictionary<string, string> ();
        dicReadFunc = new Dictionary<string, XmlUtil.AnalysisXmlNode>();
		dicVO = new Dictionary<string, Dictionary<string, object>> ();
		dicCallBack = new Dictionary<string, List<GetVORecord>> ();
	}		
		
    /// <summary>
    /// xml注册解析函数.注册时,xml文件已加载,则同时解析该xml数据;
	/// xmlPath=xml文件路径;keyAttr=取VO的属性名;callBack=xml node 解析方法,为null时,xml解析为Dictionary<string, string>类型数据.wsy 2013/3/28	
    /// </summary>
    /// <param name="xmlPath"></param>
    /// <param name="keyAttr"></param>
    /// <param name="callBack"></param>
    public void Regsiter(string xmlPath, string keyAttr, XmlUtil.AnalysisXmlNode callBack)
	{	
		if(!dicReadFunc.ContainsKey (xmlPath))
		{
			dicKeyAttr.Add (xmlPath, keyAttr);
			dicReadFunc.Add (xmlPath,callBack);
            if (ResManager.Instance.ContainsRes(xmlPath))
			{
				WWW wwwObj = ResManager.Instance.GetRes(xmlPath);
				ReadXml(xmlPath, wwwObj.text);
			}
		}	
			
	}
	/// <summary>
    /// 清除指定xml的数据.wsy
	/// </summary>
	/// <param name="xmlPath"></param>
	public void RemoveXml(string xmlPath)
	{
		if(dicVO.ContainsKey (xmlPath))
		{
			dicVO.Remove (xmlPath);
		}
	}
		
	/// <summary>
	///通过xml路径及keyAttr属性获取vo.(注意:该函数只能在xml文件已加载完成并注册后调用,否则永远返回null).
	///当keyAttr="[...]"时,返回该xml的所有解析数据.
	/// .wsy 2013/3/28
	/// </summary>		
	public object GetVO(string xmlPath, string keyAttr)
	{
		if(dicVO.ContainsKey(xmlPath))
		{
			if(keyAttr=="[...]")
			{
				return dicVO[xmlPath];
			}
			else if(dicVO[xmlPath].ContainsKey (keyAttr))
			{					
				return dicVO[xmlPath][keyAttr];
			}
		}
		else
		{
			if(!dicReadFunc.ContainsKey(xmlPath))
			{					
				Debug .Log ("Xml error:\""+xmlPath+"\" not Regsiter!");
			}
		}		
		return null;
	}

    /// <summary>
    /// 获得对应资源
    /// </summary>
    public Dictionary<string, object> GetDic(string xmlPath)
    {
        if (dicVO.ContainsKey(xmlPath))
        {
            return dicVO[xmlPath];
        }
        return null;
    }
		
	/// <summary>
	///通过xml路径及keyAttr属性获取vo传递给callBack回调.(如果该xml文件还未加载,调用此函数可触发其加载及解析).wsy 2013/3/28	
	///当keyAttr="[...]"时,返回该xml的所有解析数据.
	/// </summary>		
	public void GetVO(string xmlPath, string keyAttr, GetVODelegate callBack)
	{
		if(dicVO.ContainsKey(xmlPath))
		{
			if(callBack!=null)
			{
				callBack(GetVO(xmlPath, keyAttr));
			}
		}
		else
		{
			if(dicReadFunc.ContainsKey(xmlPath))
			{
                if (ResManager.Instance.ContainsRes(xmlPath))
				{
					WWW wwwObj = ResManager.Instance.GetRes(xmlPath);						
					ReadXml(xmlPath, wwwObj.text);
					if(callBack!=null)
					{
						callBack(GetVO(xmlPath, keyAttr));							
					}
				}
				else
				{
					ResManager.Instance.GetRes(xmlPath, Loaded);	
					if(callBack==null)
					{
						return ;
					}
					GetVORecord temp = new GetVORecord (keyAttr,callBack);
					if(dicCallBack.ContainsKey (xmlPath))
					{
						dicCallBack[xmlPath].Add (temp);
					}
					else
					{
						List<GetVORecord> list = new List<GetVORecord> ();
						list.Add (temp);
						dicCallBack.Add (xmlPath, list);
					}						
				}
			}
			else
			{
				Debug.LogWarning("Xml error:\""+xmlPath+"\" not Regsiter!");
			}
		}
	}
		
	/// xml文件加载完成回调.wsy 2013/3/28
	private void Loaded(LoadHelper data)
	{
        if (data.loadError || "" == data.www.text)
		{ 
			return;
		}
        string xmlStr = data.www.text;
        string xmlPath = data.Url;
		ReadXml(xmlPath, xmlStr);	
			
		//执行当xml文件还未加载时取xml数据的回调.wsy 2013/3/28
		if(dicCallBack.ContainsKey (xmlPath))
		{
			for(int i=0; i<dicCallBack[xmlPath].Count; i++)
			{										
				dicCallBack[xmlPath][i].callBack (GetVO(xmlPath,dicCallBack[xmlPath][i].keyAttr));					
			}
			dicCallBack.Remove (xmlPath);
		}
	}	
		
	/// 在此真正解析xml.wsy 2013/3/28	
    private Dictionary<string, object> ReadXml(string xmlPath, string xmlStr)
	{
		//Debug .Log ("ReadXml:"+xmlPath);
		Dictionary<string, object> dicObj = XmlUtil.Instance.AnalysisXmlString(xmlStr, "object", dicKeyAttr[xmlPath], dicReadFunc[xmlPath]);
        dicVO.Add (xmlPath, dicObj);	
		ResManager.Instance.RemoveRes (xmlPath);
        return dicObj;
	}
		
	class GetVORecord		
	{		
		public string keyAttr;
		public GetVODelegate callBack;
			
		public GetVORecord(string keyAttr, GetVODelegate callBack)
		{			
			this.keyAttr = keyAttr;
			this.callBack = callBack;
		}
	}	
	
}
