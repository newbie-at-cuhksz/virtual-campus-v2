using UnityEngine;
using System.Collections;

public class HighLightControl : MonoBehaviour
{

    #region Private Field
    [SerializeField]
    private Color FlashingColor1=Color.blue;
    [SerializeField]
    private Color FlashingColor2=Color.grey;
    [SerializeField]
    private Color ConstColor=Color.white;


    #endregion


    //持有当前外发光需要的组件
    private HighlightableObject m_ho;

    void Awake()
    {
        //初始化组件
        m_ho = GetComponent<HighlightableObject>();
    }


    void HifhLightFunction()
    {
        //循环往复外发光开启（参数为：颜色1，颜色2，切换时间）
        m_ho.FlashingOn(FlashingColor1, FlashingColor2, 0.5f);

        //关闭循环往复外发光
        m_ho.FlashingOff();


        //持续外发光开启（参数：颜色）
        m_ho.ConstantOn(ConstColor);

        //关闭持续外发光
        m_ho.ConstantOff();
    }

    /// <summary>
    /// 鼠标指向模型时触发
    /// </summary>
    private void OnMouseEnter()
    {
        //开启外发光
        m_ho.FlashingOn(FlashingColor1, FlashingColor2, 0.5f);
    }

    /// <summary>
    /// 鼠标离开模型时触发
    /// </summary>
    private void OnMouseExit()
    {
        //关闭外发光
        m_ho.FlashingOff();
    }
}