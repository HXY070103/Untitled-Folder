using UnityEngine;

public class SkillBarrierManager : MonoBehaviour
{
    [Header("Barrier")]
    [SerializeField] private Barrier barrier;

    private void Start()
    {
        RefreshBarrier();
    }

    public void RefreshBarrier()
    {
        if (barrier == null)
        {
            Debug.LogWarning("SkillBarrierManager: barrier is not assigned.");
            return;
        }

        if (SaveManager.instance == null)
        {
            CloseBarrier();
            return;
        }

        if (SaveManager.instance.HasChoosenSkill())
        {
            OpenBarrier();
        } else
        {
            CloseBarrier();
        }
    }

    private void OpenBarrier()
    {
        if (barrier != null)
        {
            barrier.OpenBarrier();
        }
    }
    
    private void CloseBarrier()
    {
        if (barrier != null)
        {
            barrier.CloseBarrier();
        }
    }
}