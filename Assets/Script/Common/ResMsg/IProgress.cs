using UnityEngine;
using System.Collections;

public interface IProgress
{
    /// <summary>
    /// 显示进度条.
    /// </summary>	
    void Show();

    /// <summary>
    ///  设置提示内容
    /// </summary>
    /// <param name="text"></param>
    void SetTips(string text);

    /// <summary>
    /// 更新进度,rate=0~1.
    /// </summary>	
    void Update(float rate);

    /// <summary>
    /// 关闭进度条.
    /// </summary>
    void Close();

}

