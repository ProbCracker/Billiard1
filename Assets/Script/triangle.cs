using System.Collections;
using UnityEngine;

public class PembatasSegitiga : MonoBehaviour
{
    // Variabel untuk mengontrol pergerakan segitiga
    public float jarakNaik = 1f;     // Jarak segitiga akan naik
    public float kecepatanNaik = 2f; // Kecepatan naik segitiga
    public float waktuMenghilang = 2f; // Waktu tunggu sebelum segitiga menghilang

    private Vector3 posisiAwal;

    void Start()
    {
        // Menyimpan posisi awal segitiga
        posisiAwal = transform.position;

        // Mulai coroutine untuk naik dan menghilang
        StartCoroutine(NaikDanMenghilang());
    }

    // Coroutine untuk menaikkan segitiga dan menghilangkannya
    IEnumerator NaikDanMenghilang()
    {
        // Posisi target yang akan dicapai setelah naik
        Vector3 posisiAkhir = posisiAwal + Vector3.up * jarakNaik;

        // Proses naik secara bertahap
        float waktu = 0;
        while (waktu < 1)
        {
            waktu += Time.deltaTime * kecepatanNaik;
            transform.position = Vector3.Lerp(posisiAwal, posisiAkhir, waktu);
            yield return null; // Tunggu frame berikutnya
        }

        // Tunggu beberapa saat sebelum menghilang
        yield return new WaitForSeconds(waktuMenghilang);

        // Menghilangkan objek segitiga (kamu bisa pilih antara menonaktifkan atau menghancurkan)
        gameObject.SetActive(false); // Nonaktifkan segitiga
        //Destroy(gameObject); // Atau hancurkan segitiga
    }
}
