using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolaPutih : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 posisiAwal;

    public float batasBawah = -5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody tidak ditemukan! Tambahkan komponen Rigidbody ke objek ini.");
        }

        posisiAwal = transform.position;
    }

    void Update()
    {
        // Jika bola jatuh ke bawah batas, reset ke posisi awal
        if (transform.position.y < batasBawah)
        {
            ResetBolaKePosisiAwal();
        }
    }

    void ResetBolaKePosisiAwal()
    {
        transform.position = posisiAwal;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Debug.Log("Bola kembali ke posisi awal.");
    }
}
