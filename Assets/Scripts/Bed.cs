using UnityEngine;

public class Bed : MonoBehaviour
{
    public Transform sleepPosition;

    private void OnDrawGizmos()
    {
        if (sleepPosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(sleepPosition.position, 0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player menyentuh bed!");
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Sleep(); 
            }
        }
    }
}
