#if UNITY_WINRT_8_0 || UNITY_WINRT_8_1 || UNITY_WINRT_10_0
#if UNITY_5_3_OR_NEWER
#define WSA_VR
#endif
#endif

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IRefreshable
{
    void Refresh();
    void SetVisible(bool vis);
    bool GetVisible();
    void SwitchVisible();
    void OnShow();
    void OnHide();
}


public class BaseUIProperty
{
    public static Vector3 DefaultPanelPos = new Vector3(0.45f, 0.15f, 2.5f);

    public static Vector3 BasePos = new Vector3(0.3f, 0.2f, 4.0f);
    public static Vector3 BaseForward = Vector3.forward;
    public static Vector3 BaseRight = Vector3.right;
    public static Vector3 BaseUp = Vector3.up;

    public static void Update()
    {
        Camera cam = Camera.main;
        if (!cam)
            return;

        BaseUIProperty.BaseForward = cam.transform.forward;
        BaseUIProperty.BaseRight = cam.transform.right;
        BaseUIProperty.BaseUp = cam.transform.up;
        BaseUIProperty.BasePos = cam.transform.position + BaseUIProperty.BaseForward * DefaultPanelPos.z +
            BaseUIProperty.BaseRight * DefaultPanelPos.x + BaseUIProperty.BaseUp * DefaultPanelPos.y;
    }
}

public class BasePanelEx<T> : Singleton<T>, IRefreshable where T : BasePanelEx<T>
{
    public virtual void Refresh()
    { }

    public virtual void OnClick(GameObject sender)
    {
    }

    public virtual void SetVisible(bool vis)
    {
        this.gameObject.SetActive(vis);
        if (vis)
            OnShow();
        else
            OnHide();
    }

    public virtual void OnShow() { }

    public virtual void OnHide() { }

    public virtual bool GetVisible()
    {
        return this.gameObject.activeSelf;
    }

    public virtual void SwitchVisible()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }

    // Use this for initialization
    public virtual void Awake()
    {
        //for (int i = 0; i < this.transform.childCount; i++)
        //{
        //    Transform t = this.transform.GetChild(i);
        //    Button b = t.GetComponent<Button>();
        //    if (b)
        //    {
        //        b.onClick.AddListener(delegate() { this.OnClick(t.gameObject); });
        //    }
        //}

        Button[] buttons = this.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Button b = buttons[i];
            if (b)
            {
                b.onClick.AddListener(delegate() { this.OnClick(b.gameObject); });
            }
        }
    }

}
