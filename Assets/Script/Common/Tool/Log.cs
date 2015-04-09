/**
* 文件名称：Log.cs
* 简    述：包含调试输出的方便函数，用于多人输出调试信息时方便的除去自己不需要关注的调试信息。
* 创建标识：wsy  2013/9/11
*/
using System;
using UnityEngine;


/// <summary>
/// Log静态方法类，方便每个前端人员进行调试信息的输出。
/// 一般情况下请使用自己的名字进行信息输出，比如  Log.Wsy("测试信息输出") , 并将对应的判断条件wsy = true 。
///  也可以根据需要将多个判断条件赋值为true, 但日常使用时请不要把这个类对象上传，因为每个人的配置都不一样。
/// </summary>
public class Log
{
    //通用的记录. wsy  2013/9/11
    private static bool commonLog = false;
    private static bool wsy = false;
    private static bool wbx = false;
    private static bool cyd = false;
    private static bool fhaolun = false;
    private static bool cly = true;
    private static bool fatlin = true;

    public Log()
    {
    }

    /// <summary>
    /// 通用trace函数。一般请不要使用这个函数来进行记录，除非确认前端同事都需要相同调试信息。
    /// </summary>
    /// <param name='msg'>
    /// Message.
    /// </param>
    public static void Trace(object msg)
    {
        if (commonLog)
            Debug.Log(msg);
    }

    public static void Print(object msg)
    {
//        if (UITool.panel != null)
//        {
//            UITool.panel.AddLog((string)msg);
//        }
    }

    public static void Wsy(object msg)
    {
        if (wsy)
        {
            Debug.Log("Wsy:" + msg);
        }
    }
    public static void Wbx(object msg)
    {
        if (wbx)
        {
            Debug.Log("Wbx:" + msg);
        }
    }

    public static void Fhaolun(object msg)
    {
        if (fhaolun)
        {
            Debug.Log("Fhaolun:" + msg);
        }
    }

    public static void Cyd(object msg)
    {
        if (cyd)
        {
            Debug.Log("Cyd:" + msg);
        }
    }

    public static void Cly(object msg, string className = "")
    {
        if (cly)
        {
            Debug.Log("[Cly:b]" + msg);
            //Debug.Log("Cly: in this class: "+className+";output: " + msg);
        }
    }
    public static void Fatlin(object msg)
    {
        if (fatlin)
        {
            Debug.Log("[Fatlin:b]" + msg);
            //Debug.Log("Cly: in this class: "+className+";output: " + msg);
        }
    }
    


}
