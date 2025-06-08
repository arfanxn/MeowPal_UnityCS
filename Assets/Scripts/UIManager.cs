using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerController playerController;
    public FoodPlate foodPlate;
    public Bed bed;

    private void Start()
    {
        if (playerController == null || foodPlate == null || bed == null) {
            Debug.LogError("UIManager missing references!");
            return;
        }

        playerController.SetDependencies(foodPlate, bed);
    }


    public void EatBtnOnClick()
    {
        if (playerController == null || foodPlate == null) return;
        StartCoroutine(playerController.StartEating());
    }

    public void SleepBtnOnClick()
    {
        if (playerController == null || bed == null) return;
        playerController?.StartSleeping();
    }
}