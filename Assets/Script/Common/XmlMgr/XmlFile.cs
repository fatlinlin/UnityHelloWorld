using UnityEngine;
using System.Xml;
using System.IO;

public class XmlFile 
{
	///向xml文件中写入某结构体数据的方法委托.
	public delegate void WriteCallback(object obj, XmlElement element);
	public WriteCallback writeFunc;
	private string filePath;
	private XmlDocument xmlDoc;
	private XmlNode xmlRoot;
	
	/// <summary>
	/// 读取由filePath指定的xml文件,若文件不存在则自行创建.wsy
	/// writeFunc:向xml文件中写入某结构体数据的方法委托.wsy
	/// </summary>
	public XmlFile(string filePath, WriteCallback writeFunc)
	{
		this.filePath = filePath;
		this.writeFunc = writeFunc;
		
		xmlDoc = new XmlDocument();
		string directoryPath = Path.GetDirectoryName (filePath);
		if(Directory.Exists (directoryPath)==false )
		{
			Log.Fatlin("Create Directory: " + directoryPath);		
			Directory.CreateDirectory(directoryPath);
		}
		
		if(File.Exists (filePath))
		{
			xmlDoc.Load(filePath); 	
			xmlRoot = xmlDoc.SelectSingleNode ("root");	
		}
		else
		{		
			XmlElement xe = xmlDoc.CreateElement("root");
			xmlDoc.AppendChild(xe);
			xmlRoot = (XmlNode)xe;	
		}
	}
	
	/// <summary>
	/// 向xml文件中写入节点.wsy	
	/// </summary>
	public void AddNode(object obj)
	{	
		XmlElement xe = xmlDoc.CreateElement ("object");
		if(writeFunc!=null)
		{
			writeFunc(obj, xe);
		}
		xmlRoot.AppendChild(xe);		
	}
	
	/// <summary>
	/// 以key属性为索引,替换xml文件中的一个节点.wsy	
	/// 即使xml文件中没有找到与之对应的节点,新节点也添加.
	/// </summary>
	public void ReplaceNode(string key, object obj)
	{			
		XmlElement newXe = xmlDoc.CreateElement ("object");	
		if(writeFunc!=null)
		{
			writeFunc(obj, newXe);
		}
		
		//获取根节点的所有子节点.wsy
		XmlNodeList nodeList = xmlRoot.ChildNodes;		
		//遍历所有子节点.wsy
		for (int i=0; i<nodeList.Count; i++)
		{
			XmlElement xe = (XmlElement)nodeList[i]; 
			if (xe.GetAttribute(key) == newXe.GetAttribute(key))
			{
				xmlRoot.RemoveChild (nodeList[i]);
				break ;
			}					
		}
		
		xmlRoot.AppendChild(newXe);			
	}
	
	/// <summary>
	///移除Xml文件中,key属性值为keyValue的一个节点.wsy
	/// </summary>	
	public void RemoveNode(string key, string keyValue)
	{			
		//获取根节点的所有子节点.wsy
		XmlNodeList nodeList = xmlRoot.ChildNodes;
		
		//遍历所有子节点.wsy
		for (int i=0; i<nodeList.Count; i++)
		{
			XmlElement xe = (XmlElement)nodeList[i]; 
			if (xe.GetAttribute(key) == keyValue)
			{
				xmlRoot.RemoveChild (nodeList[i]);
				break ;
			}					
		}
	}	
	
	/// <summary>
	/// 向xml文件中写入一组节点.wsy
	/// </summary>
	public void AddNodes(object[] objs)
	{				
		for(int i=0; i<objs.Length; i++)
		{
			XmlElement xe = xmlDoc.CreateElement ("object");
			if(writeFunc!=null)
			{
				writeFunc(objs[i], xe);
			}
			xmlRoot.AppendChild(xe);	
		}
	}	
	
	/// <summary>
	/// 以key属性为索引,替换掉xml文件中的一组节点.wsy	
	/// 即使xml文件中没有找到与之对应的节点,新节点也添加.
	/// </summary>
	public void ReplaceNodes(string key, object[] objs)
	{		
		//获取根节点的所有子节点.wsy
		XmlNodeList nodeList = xmlRoot.ChildNodes;
		
		for(int i=0; i<objs.Length; i++)
		{
			XmlElement newXe = xmlDoc.CreateElement ("object");	
			if(writeFunc!=null)
			{
				writeFunc(objs[i], newXe);
			}
			
			//遍历所有子节点.wsy
			for (int j=0; j<nodeList.Count; j++)
			{
				XmlElement xe = (XmlElement)nodeList[j]; 
				if (xe.GetAttribute(key) == newXe.GetAttribute(key))
				{
					xmlRoot.RemoveChild (nodeList[j]);
					break ;
				}					
			}
			xmlRoot.AppendChild(newXe);	
		}
	}
	
	/// <summary>
	///清除Xml文件中,key属性值为keyValue的一组节点.wsy
	/// </summary>	
	public void RemoveNodes(string key, string[] keyValue)
	{			
		//获取根节点的所有子节点.wsy
		XmlNodeList nodeList = xmlRoot.ChildNodes;
		
		for(int i=0; i<keyValue.Length; i++)
		{
			//遍历所有子节点.wsy
			for (int j=0; j<nodeList.Count; j++)
			{
				XmlElement xe = (XmlElement)nodeList[j]; 
				if (xe.GetAttribute(key) == keyValue[i])
				{
					xmlRoot.RemoveChild (nodeList[j]);
					break ;
				}					
			}
		}
	}
	
	/// <summary>
	///保存Xml文件到xmlPath路径.wsy
	/// </summary>
	public void Save()
	{
		xmlDoc.Save(filePath);		
	}

    /// <summary>
    /// 将Xml文件保存为另外的文件名 Lorry
    /// </summary>
    /// <param name="filePath"></param>
    public void SaveAs(string filePathName)
    {
        xmlDoc.Save(filePathName);
    }
	
	/// <summary>
	///清空由xmlPath指定的Xml文件.wsy
	/// </summary>
	public void Clear()
	{	
		xmlRoot.RemoveAll();
	}
	
	
}
