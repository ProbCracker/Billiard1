using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongkatBiliar : MonoBehaviour
{
    public Transform bolaPutih; // Referensi ke bola putih
    public float jarakDariBola = 0.5f; // Jarak tongkat dari bola
    public float kecepatanRotasi = 100f; // Kecepatan rotasi tongkat

    private bool isAiming = false; // Apakah sedang mengarahkan tembakan
    private bool isCharging = false; // Apakah sedang mengisi kekuatan
    private bool isIncreasingCharge = true; // Apakah kekuatan tembakan sedang bertambah atau berkurang
    private float kekuatanTembakSementara = 0f; // Kekuatan tembak sementara
    private float kekuatanTembakMaks = 20f; // Kekuatan tembak maksimal
    private float kecepatanCharge = 5f; // Kecepatan pengisian kekuatan
    private float angle = 0f; // Sudut rotasi tongkat

    private LineRenderer lineRenderer; // Garis bantu arah tembakan
    private LineRenderer powerBar; // Garis yang menunjukkan kekuatan tembakan
    private Vector3 posisiAwalTongkat; // Posisi awal tongkat

    void Start()
    {
        // Simpan posisi awal tongkat
        posisiAwalTongkat = transform.position;

        // Inisialisasi LineRenderer untuk garis bantu
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.enabled = false; // Garis bantu awalnya tidak aktif

        // Inisialisasi LineRenderer untuk power bar
        powerBar = new GameObject("PowerBar").AddComponent<LineRenderer>();
        powerBar.positionCount = 2;
        powerBar.startWidth = 0.05f;
        powerBar.endWidth = 0.05f;
        powerBar.material = new Material(Shader.Find("Sprites/Default"));
        powerBar.startColor = Color.green;
        powerBar.endColor = Color.green;
        powerBar.enabled = false;
    }

    void Update()
    {
        // Jika sedang mengarahkan, tongkat mengikuti mouse dan dapat berputar 360 derajat
        if (isAiming)
        {
            UpdateRotasiTongkat();
            UpdateGarisBantu();
        }

        // Memperbarui power bar jika sedang charging
        if (isCharging)
        {
            UpdatePowerBar();
        }

        // Meluncurkan bola putih ketika mouse dilepas
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            isAiming = false;
            isCharging = false;

            // Sembunyikan garis bantu dan power bar
            lineRenderer.enabled = false;
            powerBar.enabled = false;

            LaunchBall(); // Meluncurkan bola setelah charging selesai
        }
    }

    void OnMouseDown()
    {
        // Mulai mengarahkan tembakan
        isAiming = true;
        isCharging = true; // Mulai charging kekuatan tembak
        isIncreasingCharge = true; // Reset pengisian
        kekuatanTembakSementara = 0f; // Reset kekuatan tembak
        lineRenderer.enabled = true; // Aktifkan garis bantu
        powerBar.enabled = true; // Aktifkan power bar
    }

    void OnMouseUp()
    {
        // Berhenti mengarahkan tembakan saat mouse dilepas
        isAiming = false;
        isCharging = false;

        // Sembunyikan garis bantu dan power bar
        lineRenderer.enabled = false;
        powerBar.enabled = false;

        LaunchBall(); // Meluncurkan bola setelah charging selesai
    }

    void UpdateRotasiTongkat()
    {
        // Mendapatkan posisi mouse di layar dan mengubahnya menjadi posisi dunia
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Plane plane = new Plane(Vector3.up, bolaPutih.position); // Membuat bidang horizontal untuk memproyeksikan rotasi
        float distance;

        // Cek apakah ray dari kamera memotong bidang
        if (plane.Raycast(ray, out distance))
        {
            // Dapatkan posisi di mana ray dari mouse bertemu dengan bidang horizontal
            Vector3 worldMousePosition = ray.GetPoint(distance);

            // Hitung arah dari bola putih ke posisi mouse
            Vector3 direction = worldMousePosition - bolaPutih.position;

            // Hitung sudut rotasi menggunakan arctan2
            angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            // Perbarui posisi dan rotasi tongkat dengan menjadikannya berputar 360 derajat di sekitar bola putih
            transform.position = bolaPutih.position + direction.normalized * jarakDariBola;
            transform.rotation = Quaternion.Euler(0, -angle, 0);
        }
    }

    void UpdateGarisBantu()
    {
        Vector3 arahTembakan = (bolaPutih.position - transform.position).normalized;
        Vector3 endPosition = bolaPutih.position + arahTembakan * 5f; // Garis bantu memiliki panjang maksimum

        RaycastHit hit;
        if (Physics.Raycast(bolaPutih.position, arahTembakan, out hit, 5f))
        {
            endPosition = hit.point; // Batasi jika ada objek di depan
        }

        lineRenderer.SetPosition(0, bolaPutih.position);
        lineRenderer.SetPosition(1, endPosition);
    }

    void UpdatePowerBar()
    {
        if (isIncreasingCharge)
        {
            kekuatanTembakSementara += kecepatanCharge * Time.deltaTime;
            if (kekuatanTembakSementara >= kekuatanTembakMaks)
            {
                kekuatanTembakSementara = kekuatanTembakMaks;
                isIncreasingCharge = false; // Mulai mengurangi jika mencapai maksimal
            }
        }
        else
        {
            kekuatanTembakSementara -= kecepatanCharge * Time.deltaTime;
            if (kekuatanTembakSementara <= 0f)
            {
                kekuatanTembakSementara = 0f;
                isIncreasingCharge = true; // Mulai menambah lagi jika mencapai 0
            }
        }

        // Perbarui posisi power bar
        Vector3 startBar = bolaPutih.position + new Vector3(0.5f, 0, 0);
        Vector3 endBar = startBar + new Vector3(0, kekuatanTembakSementara / kekuatanTembakMaks, 0);

        powerBar.SetPosition(0, startBar);
        powerBar.SetPosition(1, endBar);
    }

    void LaunchBall()
    {
        Vector3 arahTembakan = (bolaPutih.position - transform.position).normalized;
        Rigidbody rb = bolaPutih.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(arahTembakan * kekuatanTembakSementara, ForceMode.Impulse);
    }
}
