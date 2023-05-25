using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class spawnScript : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnPoint;

    void Start()
    {
        transform.position = m_SpawnPoint.position;
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("win"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("WinMenu");
        }
    }
}