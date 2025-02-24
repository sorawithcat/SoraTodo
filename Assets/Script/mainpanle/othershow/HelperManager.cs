using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperManager : MonoBehaviour
{
    [SerializeField] private GameObject helperPrefab;
    [SerializeField] private float speed = 200f;
    [SerializeField] private float speedOffsete = 60f;
    [SerializeField] private float yOffset1 = 90f;
    [SerializeField] private float yOffset2 = -80f;
    [SerializeField] private float creatTime = 0.5f;
    [SerializeField] private float creatTimeOffset = 0.1f;
    private List<string> helperList = new() { "sorawithcat" };
    private List<string> showedhelperList = new();

    private float currentTime = 0f;
    private float currentCreatTime = 1f;
    private float currentSpeed = 200f;
    private float currentYOffset = 0f;
    string showText;
    GameObject newHelper;
    ScrollingText scrollingTextcs;
    private void Start()
    {
        currentCreatTime = float.MinValue;
    }
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= currentCreatTime)
        {
            currentTime = 0f;
            currentCreatTime = Random.Range(creatTime - creatTimeOffset, currentTime + creatTimeOffset);
            currentSpeed = Random.Range(speed - speedOffsete, speed + speedOffsete);
            currentYOffset = Random.Range(yOffset2, yOffset1);
            newHelper = Instantiate(helperPrefab, new Vector3(transform.position.x, transform.position.y + currentYOffset, transform.position.z), Quaternion.identity);
            newHelper.transform.SetParent(transform);
            scrollingTextcs = newHelper.GetComponent<ScrollingText>();
            scrollingTextcs.scrollSpeed = currentSpeed;
            if (helperList.Count > 0)
            {
                showText = helperList[Random.Range(0, helperList.Count)];
            }
            else
            {
                helperList = showedhelperList;
                showedhelperList = new();
                showText = helperList[Random.Range(0, helperList.Count)];
            }
            scrollingTextcs.scrollingText.text = showText;
            scrollingTextcs.scrollingText.color = Random.ColorHSV();
            helperList.Remove(showText);
            showedhelperList.Add(showText);
            return;
        }
    }


}
