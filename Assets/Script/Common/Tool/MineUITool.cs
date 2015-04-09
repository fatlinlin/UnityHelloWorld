using UnityEngine;
using System.Collections;

public class MineUITool
{
    private static GameObject _main;
    /// UI根节点=UI Root (2D);
    private static GameObject _root;
    /// UI相机=Camera;
    private static UICamera _camera;

    private static UIRoot uiRoot;

    private static UISprite uiSpite;

    #region ------------初始化------------------
    public static void Init(GameObject root)
    {
        _main = GameObject.Find("Main");
        _root = root;
        uiRoot = root.GetComponent<UIRoot>();
        _camera = _root.transform.Find("Camera").GetComponent<UICamera>();
        uiSpite = _root.transform.Find("Sprite").GetComponent<UISprite>();
        Log.Fatlin("sss");
        //        uiSpite.name = "C103193";
        //        uiSpite.spriteName = "C103193";
        //        atlas = (UIAtlas)Resources.Load("sprite", typeof(UIAtlas));
        var sp = new UISprite();


        var atlas = FindUIGameObject("Sprite").GetComponent<UISprite>().atlas; 
        UISprite sprite = NGUITools.AddSprite(_root, atlas, "C103193");
        sprite.transform.localScale = new Vector3(1, 1, 1);
        sprite.transform.localPosition = new Vector3(0, 0, 0);
        sprite.MakePixelPerfect();



        //TODO:atlas


    }
    #endregion

    ///通过名称及对齐类型,查找各UI界面的GO.wsy
    public static GameObject FindUIGameObject(string name)
    {
        Transform trans = _root.transform.Find(name);
        GameObject go;
        if (trans == null)
        {
            var ob = Resources.Load("UI/" + name);
            if (ob == null) return null;
            go = GameObject.Instantiate(ob) as GameObject;
            Vector3 pos = go.transform.localPosition;
            if (go != null)
            {
                go.transform.parent = _root.transform;
                go.name = name;
                go.transform.localPosition = pos;
                go.transform.localScale = Vector3.one;
            }
        }
        else { go = trans.gameObject; }
        return go;
    }
}
