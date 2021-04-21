using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelOnClick : MonoBehaviour
{
    #region Public Fields
    [Tooltip("放入info panel的GameObject")]
    public GameObject InfoPanel;
    [Tooltip("放入panel的prefab，用于在info panel上显示指定panel")]
    public GameObject ChildPanelPrefab;
    #endregion

    #region Private Fields
    static private GameObject _CurrentGameObject;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _CurrentGameObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if( _CurrentGameObject == gameObject )
        {
            // update infomation
        }
        else
        {
            _CurrentGameObject = gameObject;
            // destroy child object
            for (int i = 0; i < InfoPanel.transform.childCount; i++)
            {
                Destroy(InfoPanel.transform.GetChild(i).gameObject);
            }
            // 实例化child object
            GameObject ChildPanel = Instantiate(ChildPanelPrefab) as GameObject;
            ChildPanel.transform.parent = InfoPanel.transform;
        }
    }
}
