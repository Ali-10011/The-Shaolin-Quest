using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject[] Screens;
    [SerializeField] GameObject player;
    public static int itemNo = 0;
    Canvas canvas;


    public float colDepth = 4f;
    public float zPosition = 0f;
    private Vector2 screenSize;

    private Transform topCollider;
    private Transform bottomCollider;
    private Transform leftCollider;
    private Transform rightCollider;
    private Vector3 cameraPos;


    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void OnClickPause()
    {
        Time.timeScale = 0;
        OnClickMenuBtn(3);
    }

    public void OnClickMenuStart()
    {
        if (!PlayerPrefs.HasKey("UnlockedLevels"))
        {
            PlayerPrefs.SetInt("UnlockedLevels", 1);
        }
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels");
        Transform levelScreen = transform.Find("LevelSelect");
        for (int i = 2; i <= unlockedLevels; i++)
        { 
            levelScreen.Find("Level" + i).gameObject.GetComponent<Button>().interactable = true;
        }
        OnClickMenuBtn(1);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        OnClickMenuBtn(2);
    }

    public void OnClickLevel(Button btn)
    {
        PlayerPrefs.SetInt("currentLevel", btn.name[btn.name.Length - 1] - '0');
        OnClickMenuBtn(2);
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(new Vector3(0, -14.5f, 20.6f));
        GameObject _player = Instantiate(player,null);
        _player.transform.position = viewportPos;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }


    public void OnClickNextLvl()
    {
        DestroyLevel();
        int lvl = PlayerPrefs.GetInt("currentLevel");
        lvl++;
        Transform scorePanel = canvas.transform.Find("LevelSelect");
        Button lvlButton = scorePanel.Find("Level" + lvl).GetComponent<Button>();
        OnClickLevel(lvlButton);
    }

    public void Restart()
    {
        DestroyLevel();
        int lvl = PlayerPrefs.GetInt("currentLevel");
        Transform scorePanel = canvas.transform.Find("LevelSelect");
        Button lvlButton = scorePanel.Find("Level" + lvl).GetComponent<Button>();
        OnClickLevel(lvlButton);
        Time.timeScale = 1f;
    }

    void DestroyLevel()
    {
        DestroyImmediate(GameObject.FindGameObjectWithTag("Player"));
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
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
            {
                if (i == 2) canvas.renderMode = RenderMode.ScreenSpaceCamera;
                else canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                Screens[i].SetActive(true);
            }
            else
            {
                Screens[i].SetActive(false);
            }
        }
    }
}
