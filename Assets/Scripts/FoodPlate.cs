using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPlate : MonoBehaviour
{
    public void Consume()
    {
        Debug.Log("Player eats food");
        // Tambahkan animasi atau efek makan di sini jika perlu
        Destroy(gameObject); // opsional: hilangkan setelah dimakan
    }
}

