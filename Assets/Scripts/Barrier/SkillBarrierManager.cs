using UnityEngine;

public class SkillBarrierManager : MonoBehaviour
{
    [Header("Barrier")]
    [SerializeField] private Barrier barrier;

    private void Start()
    {
        if (SaveManager.instance.HasChoosenSkill())
        {
            OpenBarrier();
        } else
        {
            CloseBarrier();
        }
    }

    private void Update()
    {
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