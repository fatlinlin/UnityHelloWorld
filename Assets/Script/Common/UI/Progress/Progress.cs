using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Progress : IProgress
{
    protected GameObject mGO;
    protected Transform mTrans;
    protected UISlider mSlider;
    protected UILabel rateLabel;
    protected UILabel tipsLabel;
    protected UITexture Bg;
    protected GameObject model1;
    protected GameObject model2;
    protected GameObject model3;
    protected GameObject model4;
    protected float rate;
    protected bool isShow;
    float targetRate;
    float delta;

    public Progress(GameObject go)
    {
        mGO = go;
        //        mTrans = go.transform;
        //        Transform tTrans = mTrans.Find("Slider");
        //        if (tTrans != null) mSlider = tTrans.GetComponent<UISlider>();
        //        tTrans = mTrans.Find("Label_rate");
        //        if (tTrans != null) rateLabel = tTrans.GetComponent<UILabel>();
        //        tTrans = mTrans.Find("Label_tips");
        //        if (tTrans != null) tipsLabel = tTrans.GetComponent<UILabel>();
        //        tTrans = mTrans.Find("Texture");
        //        if (tTrans != null) Bg = tTrans.GetComponent<UITexture>();
        //
        //        tTrans = mTrans.Find("Model1");
        //        if (tTrans != null) model1 = tTrans.gameObject;
        //        tTrans = mTrans.Find("Model2");
        //        if (tTrans != null) model2 = tTrans.gameObject;
        //        tTrans = mTrans.Find("Model3");
        //        if (tTrans != null) model3 = tTrans.gameObject;
        //        tTrans = mTrans.Find("Model4");
        //        if (tTrans != null) model4 = tTrans.gameObject;


    }
    #region -------对外接口--------
    /// <summary>
    /// 显示进度条.
    /// </summary>	
    public virtual void Show()
    {
        //        if (isShow) return;
        //        rate = 0.01f;
        //        targetRate = 0;
        //        delta = 0.0037f;
        //        if (mSlider != null) mSlider.value = rate;
        //        if (rateLabel != null) rateLabel.text = string.Format("{0:0}%", rate * 100);
        //        if (tipsLabel != null) tipsLabel.text = "";
        //        if(UIDataManager.Instance.ProgressTips!=null)
        //        {
        //            tipsLabel.text = UIDataManager.Instance.ProgressTips[Random.Range(0, UIDataManager.Instance.ProgressTips.Count - 1)];
        //        }
        //        NGUITools.SetActive(mGO, true);
        //        isShow = true;
        //        TimerMgr.Instance.AddCallBack(AutoAddRate,TimerMgr.RepeatType.OneTenthSecond);
        //
        //        NGUITools.SetActive(model1, false);
        //        NGUITools.SetActive(model2, false);
        //        NGUITools.SetActive(model3, false);
        //        NGUITools.SetActive(model4, false);
        //
        //        if(Bg!=null)
        //        {
        //            int url = Random.Range(1, 4);
        //            TextureManager.Instance.cardBg.SetTexture("LoadingBg" + url, Bg);
        //            if (url == 1)
        //            {
        //                NGUITools.SetActive(model1, true);
        //            }
        //            else if (url == 2)
        //            {
        //                NGUITools.SetActive(model2, true);
        //            }
        //            else if (url == 3)
        //            {
        //                NGUITools.SetActive(model3, true);
        //            }
        //            else if (url == 4)
        //            {
        //                NGUITools.SetActive(model4, true);
        //            }
        //
        //        }
       Log.Fatlin("show progress");
    }

    /// <summary>
    ///  设置提示内容
    /// </summary>
    /// <param name="text"></param>
    public void SetTips(string text)
    {
        if (tipsLabel != null) tipsLabel.text = text;
    }

    /// <summary>
    /// 更新进度条,rate=0~1.
    /// </summary>	
    public virtual void Update(float rate)
    {
        if (!isShow) return;
        targetRate = Mathf.Clamp01(rate);
        //this.rate = Mathf.Clamp01(rate);
        //if (mSlider != null) mSlider.value = rate;
        //if (rateLabel != null) rateLabel.text = string.Format("{0:0}%", rate * 100);
    }

    /// <summary>
    /// 关闭进度条.
    /// </summary>
    public virtual void Close()
    {
       Log.Fatlin("close progress");
        //        isShow = false;
        //        NGUITools.SetActive(mGO, false);
        //        TimerMgr.Instance.RemoveCallBack(AutoAddRate, TimerMgr.RepeatType.OneTenthSecond);
    }
    #endregion -------对外接口--------

    void AutoAddRate()
    {
        if (rate + delta < 1)
        {
            if (targetRate > rate)
            {
                rate += (targetRate - rate) * 0.13f;
            }
            else
            {
                rate += delta;
            }
            if (rateLabel != null) rateLabel.text = string.Format("{0:0}%", rate * 100);
        }
    }
}
