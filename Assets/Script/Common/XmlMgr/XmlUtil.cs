using UnityEngine;
using System.Collections.Generic;

public class XmlUtil
{
    public delegate object AnalysisXmlNode(Dictionary<string, string> dicBody);

	public static readonly XmlUtil Instance = new XmlUtil ();
		
	/// <summary>
	/// 将xml字符串解析为一个二维字典.
	/// xmlstr:xml文件读入字符串.
	/// nodeName:子节点名.
	/// keyAttr:作为字典键值的属性名.
	/// callback:子节点属性解析方法.
	/// .wsy 2013/2/26
	/// </summary>		
	public Dictionary<string, object> AnalysisXmlString (string xmlStr, string nodeName, string keyAttr, AnalysisXmlNode callback)
	{
		Dictionary<string, object> dicObject = new Dictionary<string, object> ();
		Dictionary<string,string> dicBody = GetElement (xmlStr, nodeName);
		object obj;
		while (dicBody != null) 
		{				
			obj = (callback!=null? callback(dicBody):dicBody);						
			//以keyAttr属性值(此属性值必须唯一)为键值,将obj存入二维字典dicObject中.wsy 2013/2/26
			if(!dicObject.ContainsKey (dicBody[keyAttr]))
			{
				dicObject.Add (dicBody[keyAttr], obj);					
			}
			else
			{
				Debug .Log("Read xml error: key \""+keyAttr+"\" repeat value "+dicBody [keyAttr]+"!");
			}
			//移除已读取的子节点.wsy 2013/2/26
			xmlStr = RemoveBody (xmlStr, nodeName);
			//读取下一个子节点.wsy 2013/2/26
			dicBody = GetElement (xmlStr, nodeName);
		}
          
		return dicObject;
	}	
		
	private Dictionary<string,string> GetElement (string strXml, string strName)
	{
		int start = strXml.IndexOf ("<" + strName);
		int end = strXml.IndexOf ("/>");
		if ((start > -1) && (end > start)) 
		{
			Dictionary<string,string> dic = new Dictionary<string, string> ();

			int stripStart = start + 1 + strName.Length;
			int stripLength = end - stripStart;
			string strAttrGroup = strXml.Substring (stripStart, stripLength);
			strAttrGroup = strAttrGroup.Trim ();

			string[] arrAttr = strAttrGroup.Split ('\"');
			for (int i = 0; i < arrAttr.Length-1; i+=2)
			{
				string key = arrAttr [i];
				key = key.Replace (" ","");
				key = key.Replace ("=","");
				dic.Add (key,arrAttr [i+1]);					
			}
			return dic;
		} 
        else
        {
			return null;
		}
	}

	private string RemoveNode (string strXml, string strLeft, string strRight)
	{
		int start = strXml.IndexOf (strLeft);
		int end = strXml.IndexOf (strRight);
		if ((start > -1) && (end > start)) {
			strXml = strXml.Substring (end + strRight.Length);
		}
		return strXml;
	}

	private string RemoveBody (string strXml, string strName)
	{
		return RemoveNode (strXml, "<" + strName + " ", "/>");
	}		
		
}


