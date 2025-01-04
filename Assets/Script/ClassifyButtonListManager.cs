using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassifyButtonListManager : MonoBehaviour
{

    private List<ClassifyButtonManager> classifyButtonList;
    // Start is called before the first frame update
    void Start()
    {
        SetAllClassifyButtons();
    }

    /// <summary>
    /// 设置每个按钮
    /// </summary>
    private void SetAllClassifyButtons()
    {
        int i = 0;
        GetAllClassifyButtons();
        foreach (ClassifyButtonManager button in classifyButtonList)
        {
            button.thisID = i;
            i++;

        }
    }
    /// <summary>
    /// 获取所有按钮
    /// </summary>
    public List<ClassifyButtonManager> GetAllClassifyButtons()
    {
        classifyButtonList = new List<ClassifyButtonManager>(GetComponentsInChildren<ClassifyButtonManager>());
        return classifyButtonList;
    }

    // Update is called once per frame
    void Update()
    {

    }


}
