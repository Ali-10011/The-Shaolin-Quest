using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject[] Screens;
    public static int itemNo = 0;
    Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void OnClickPause()
    {
        Time.timeScale = 0;
        OnClickMenuBtn(1);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        OnClickMenuBtn(0);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }

    public void Restart()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnClickMenuBtn(int val)
    {
        itemNo = val;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Screens.Length; i++)
        {
            if (i == itemNo)
                Screens[i].SetActive(true);
            else
                Screens[i].SetActive(false);
        }
    }
}
