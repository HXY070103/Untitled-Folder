using UnityEngine;

public enum SkillChooseMode
{
    InitialChoose,
    TutorialPractice
}

public class SkillChoosePanel : MonoBehaviour
{
    [SerializeField] private SkillChooseMode skillChooseMode = SkillChooseMode.InitialChoose;
    private string pauseKey = "SkillChoose";

    [SerializeField] private GameObject panelRoot;

    private void Awake()
    {
        panelRoot.SetActive(false);
    }

    public void OpenPanel()
    {

        if (skillChooseMode == SkillChooseMode.InitialChoose)
        {
            if (SaveManager.instance != null && SaveManager.instance.HasChoosenSkill())
            {
                Debug.Log("Has chosen skill");
                return;
            }
        }

        panelRoot.SetActive(true);

        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.Pause(pauseKey);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    public void ChooseTimeControl()
    {
        ChooseSkill(SkillType.TimeControl);
    }

    public void ChooseLifeSteal()
    {
        ChooseSkill(SkillType.LifeSteal);
    }

    public void ChoosePhase()
    {
        ChooseSkill(SkillType.Phase);
    }

    private void ChooseSkill(SkillType skillType)
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();

        if (skillManager == null)
        {
            Debug.LogError("Cannot find PlayerSkillManager in this scene.");
            return;
        }

        skillManager.ChooseSkill(skillType);

        if (skillChooseMode == SkillChooseMode.InitialChoose)
        {
            RecordInitialSkill(skillManager);
            if (SaveManager.instance != null)
            {
                SaveManager.instance.MarkInitialSkillChosen();
                SaveManager.instance.SaveGame();
            }
            SkillBarrierManager barrierManager = FindFirstObjectByType<SkillBarrierManager>();
            if (barrierManager != null)
            {
                barrierManager.RefreshBarrier();
            }
        }
        ClosePanel();
    }

    private void RecordInitialSkill(PlayerSkillManager skillManager)
    {
        if (HistoryManager.instance == null)
        {
            Debug.LogWarning("No History Manager");
            return;
        }
        SkillBase selectedSkill = skillManager.GetCurrentSkill();
        if (selectedSkill == null)
        {
            Debug.LogWarning("Cannot record initial skill");
            return;
        }

        bool recordSucceed = HistoryManager.instance.RecordInitialSkill(selectedSkill.SkillName, selectedSkill.Description);
        if (!recordSucceed)
        {
            Debug.LogWarning("Fail to record initial skill");
        }
    }

    public void ClosePanel()
    {
        panelRoot.SetActive(false);

        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.Resume(pauseKey);
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}