using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bola : MonoBehaviour
{
    private Rigidbody rb;
    public float gayaDorong; // Kecepatan atau besar gaya dorong dari mouse
    public float detectionRadius = 2f; // Radius deteksi untuk menghindari mouse

    // Material asli dan material glow
    private Material materialAsli;
    public Material glowMaterial;  // Material glow yang sudah disiapkan
    private Renderer bolaRenderer; // Renderer bola

    // Durasi glow (berapa lama bola akan bersinar setelah diklik)
    public float durasiGlow = 0.5f;

    // Physics material reference for better rolling
    public PhysicMaterial bolaPhysicsMaterial;

    void Start()
    {
        // Mengambil komponen Rigidbody pada bola
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody tidak ditemukan! Tambahkan komponen Rigidbody ke objek ini.");
        }

        // Pastikan Rigidbody menggunakan gravitasi
        rb.useGravity = true;

        // Mengambil Renderer bola untuk mengubah material
        bolaRenderer = GetComponent<Renderer>();
        materialAsli = bolaRenderer.material; // Simpan material asli bola

        // Terapkan physics material untuk kontrol gesekan dan pantulan
        Collider collider = GetComponent<Collider>();
        if (collider != null && bolaPhysicsMaterial != null)
        {
            collider.material = bolaPhysicsMaterial;
        }
    }

    void Update()
    {
        // Mengambil posisi mouse di layar dan mengubahnya menjadi posisi dunia 3D
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane; // Atur kedalaman mouse sesuai kamera
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Hitung jarak antara bola dan mouse
        float distance = Vector3.Distance(transform.position, worldMousePosition);

        // Jika mouse mendekati bola dan jarak lebih kecil dari radius deteksi, dorong bola menjauh
        if (distance < detectionRadius)
        {
            // Hitung arah dari mouse ke bola
            Vector3 arahDorongan = (transform.position - worldMousePosition).normalized;

            // Berikan gaya dorong ke bola untuk menjauh dari mouse
            rb.AddForce(arahDorongan * gayaDorong * (detectionRadius - distance), ForceMode.Force);
        }
    }

    // Fungsi ini dipanggil ketika bola diklik
    void OnMouseDown()
    {
        // Arah dorongan bisa disesuaikan. Misalnya: dorong ke depan.
        Vector3 arahDorongan = new Vector3(1f, 0f, 1f).normalized;

        // Menambahkan gaya ke bola saat diklik
        rb.AddForce(arahDorongan * gayaDorong, ForceMode.Impulse);

        // Mulai animasi glow saat bola diklik
        StartCoroutine(HighlightBola());

        // Debugging untuk melihat arah dan gaya yang diberikan
        Debug.Log("Bola diklik, gaya dorong diterapkan dengan arah: " + arahDorongan);
    }

    // Coroutine untuk membuat bola bersinar (glow) sementara
    IEnumerator HighlightBola()
    {
        // Ganti material bola ke glowMaterial
        bolaRenderer.material = glowMaterial;

        // Tunggu selama durasi glow
        yield return new WaitForSeconds(durasiGlow);

        // Kembalikan material bola ke material asli
        bolaRenderer.material = materialAsli;
    }

    // Memastikan tabrakan hanya menghasilkan efek rolling, bukan melayang
    private void OnCollisionEnter(Collision collision)
    {
        // Pastikan bola tetap stabil dan tidak melayang-layang saat menabrak
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);  // Jaga agar Y tetap nol untuk rolling
        rb.angularVelocity = new Vector3(rb.angularVelocity.x, 0, rb.angularVelocity.z);  // Rotasi di Y tidak terlalu besar
    }
}
