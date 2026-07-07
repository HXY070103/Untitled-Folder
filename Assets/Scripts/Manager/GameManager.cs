using Unity.VectorGraphics;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Boss Level")]
    [SerializeField] private List<string> scenesThatIncreaseBossLevel = new List<string>();

    public static GameManager instance;

    public bool isChangeScene;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // change scene for mainmenu, tutorial...
    public void LoadScene(string sceneName)
    {
        if (isChangeScene)
        {
            return;
        }
        StartCoroutine(LoadSceneCo(sceneName, false));
    }

    public void LoadSavedScene(string sceneName)
    {
        if (isChangeScene)
        {
            return;
        }
        StartCoroutine(LoadSceneCo(sceneName, true));
    } 

    private IEnumerator LoadSceneCo(string sceneName, bool loadSaveData)
    {
        isChangeScene = true;

        // fade out 淡入
        yield return Fade(0f, 1f);

        SceneManager.LoadScene(sceneName);
        yield return null;
        yield return new WaitForSecondsRealtime(0.2f);

        if (SaveManager.instance != null)
        {
            SaveManager.instance.RefreshSavables();
            if (loadSaveData)
            {
                SaveManager.instance.LoadDataToCurrentScene();
            }
            SaveManager.instance.SaveLevelStart();
        }

        //fade in
        yield return Fade(1f, 0f);

        isChangeScene = false;
    }

    // change scene by waypoint: map to map
    public void ChangeScene(string sceneName, WaypointType type)
    {
        StartCoroutine(ChangeSceneCo(sceneName, type));
    }

    private IEnumerator ChangeSceneCo(string sceneName, WaypointType waypointType)
    {
        // Fade effect
        isChangeScene = true;
        yield return Fade(0f, 1f);

        SceneManager.LoadScene(sceneName);

        TryIncreaseBossLevel(sceneName);

        yield return null;
        yield return new WaitForSeconds(0.2f);

        bool targetSceneClear = false;

        if (SaveManager.instance != null)
        {
            targetSceneClear = SaveManager.instance.IsSceneCleared(sceneName);
            SaveManager.instance.RefreshSavables();
            SaveManager.instance.LoadDataToCurrentScene();
        }
       
        if (!targetSceneClear)
        {
            Vector3 position = GetWaypointPosition(waypointType);

            if (position != Vector3.zero)
            {
                Player.instance.TeleportPlayer(position);
            }
        }
        


        if (SaveManager.instance != null)
        {
            SaveManager.instance.RefreshSavables();
            SaveManager.instance.SaveGame();
        }

        yield return Fade(1f, 0f);
        isChangeScene = false;
    }

    private Vector3 GetWaypointPosition(WaypointType type)
    {
        var waypoints = FindObjectsByType<Waypoint>(FindObjectsSortMode.None);
        foreach(var point in waypoints)
        {
            if (point.GetWaypointType() == type)
            {
                point.SetCanbeTrigger(false);
                return point.GetRespawnPoint();
            }
        }
        return Vector3.zero;
    }
    public void DestorySelf()
    {
        if (instance == this)
        {
            instance = null;
        }
        Destroy(gameObject);
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("FadeImage is not assigned on GameManager.");
            yield break;
        }
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;

        float timer = 0f;
        Color color = fadeImage.color;
        color.a = fromAlpha;
        fadeImage.color = color;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, timer / fadeDuration);
            color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color = fadeImage.color;
        color.a = toAlpha;
        fadeImage.color = color;

        if (toAlpha <= 0f)
        {
            fadeImage.raycastTarget = false;
        }
    }

    private void TryIncreaseBossLevel(string sceneName)
    {
        if (SaveManager.instance == null)
        {
            return;
        }

        if (scenesThatIncreaseBossLevel.Contains(sceneName))
        {
            SaveManager.instance.IncreaseNumOfLevel();
        }
    }

    public List<string> GetRepeatLevel()
    {
        return scenesThatIncreaseBossLevel;
    }
}
