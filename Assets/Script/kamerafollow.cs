using UnityEngine;

public class KameraMengikutiBola : MonoBehaviour
{
    public Transform bolaPutih; // Bola yang akan diikuti
    public Vector3 offsetKamera = new Vector3(0, 5, -7); // Offset kamera dari bola
    public float kecepatanMengikuti = 5f; // Kecepatan kamera mengikuti bola

    private bool isFollowing = false; // Apakah kamera mengikuti bola

    void LateUpdate()
    {
        if (isFollowing)
        {
            // Posisi kamera bergerak halus mengikuti bola
            Vector3 posisiTarget = bolaPutih.position + offsetKamera;
            transform.position = Vector3.Lerp(transform.position, posisiTarget, kecepatanMengikuti * Time.deltaTime);
        }
    }

    // Fungsi untuk mulai mengikuti bola setelah ditembak
    public void MulaiMengikuti()
    {
        isFollowing = true;
    }

    // Fungsi untuk berhenti mengikuti bola
    public void BerhentiMengikuti()
    {
        isFollowing = false;
    }
}
