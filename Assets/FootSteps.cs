using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FootSteps : MonoBehaviour
{
    private GameObject m_Player;

    private Vector3 m_PlayerPosition;
    
    private float m_Elapsed;

    private float m_Range;

    private GameObject[] aiList;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.Find("[Player]");
        m_PlayerPosition = m_Player.transform.position;
        aiList = GameObject.FindGameObjectsWithTag("ai");
        m_Elapsed = 0;
        m_Range = 16.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_Elapsed += Time.deltaTime;
        GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(new Vector2(10, 10), new Vector2(m_Range * 2, m_Range * 2), m_Elapsed);
        Color currentColor = GetComponent<Image>().color;
        currentColor.a = Mathf.Lerp(1, 0, m_Elapsed);
        GetComponent<Image>().color = currentColor;

        if (m_Elapsed > 1.0f)
        {
            Destroy(gameObject);
        }

        foreach (var ai in aiList)
        {
            if (m_Range > Vector3.Distance(m_PlayerPosition, ai.transform.position))
            {
                ai.GetComponent<AiBehaviour>().FollowNoise(m_PlayerPosition);
            }

            if (5.0f > Vector3.Distance(m_PlayerPosition, ai.transform.position))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("DeathMenu");
            }
        }

        m_Range = Input.GetKey(KeyCode.LeftShift) ? 40.0f : 20.0f;
    }

    public void PlayStep(AudioSource _source, float _volume)
    {
        _source.volume = _volume;
        _source.Stop();
        _source.Play();
    }
}