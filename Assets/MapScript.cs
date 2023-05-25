using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PhysicsBasedCharacterController;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapScript : MonoBehaviour
{
    private RectTransform m_Rect;
    private float m_RectWidth;
    private float m_RectHeight;
    private float m_Up;
    private float m_Right;
    private float m_Down;
    private float m_Left;
    private float m_Width;
    private float m_Height;

    private Camera m_Camera;
    [SerializeField] private GameObject m_Ring;
    [SerializeField] private Transform m_Map;
    [SerializeField] Transform m_Player;
    [SerializeField] private Sprite m_PlayerSprite;
    [SerializeField] private Sprite m_AiSprite;
    private Vector3 m_PlayerPosition;
    private GameObject m_PlayerMap;

    private List<GameObject> m_CenterLayerObjs;
    private GameObject[] m_AiList;
    private List<RectTransform> m_AiMapImg;
    [SerializeField] private TunnelsTypes m_TunnelsTypes;
    private TunnelsTypes.TunnelType m_TunnelType;

    private float m_Elapsed;

    [SerializeField] private AudioSource m_source;
    [SerializeField] private AudioSource m_Ambientsource;

    [SerializeField] private AudioClip m_Growl;
    [SerializeField] private AudioClip[] m_AmbientClips;

    private float m_SoundElapsed;
    private float m_TitamElapsed;

    [SerializeField] private AudioSource m_Music;
    private float m_MusicElapsed;

    void Start()
    {
        m_SoundElapsed = 0.0f;
        m_MusicElapsed = 0.0f;
        m_TitamElapsed = 0.0f;
        m_AiMapImg = new List<RectTransform>();
        m_Elapsed = 0;
        m_AiList = GameObject.FindGameObjectsWithTag("ai");
        m_CenterLayerObjs = new List<GameObject>();
        m_Camera = Camera.main;

        m_Rect = GetComponent<RectTransform>();
        m_RectWidth = m_Rect.rect.width;
        m_RectHeight = m_Rect.rect.height;

        foreach (var tun in GameObject.FindObjectsOfType<GameObject>())
        {
            if (tun.layer == 6)
            {
                m_CenterLayerObjs.Add(tun);
            }
        }

        foreach (var tun in m_CenterLayerObjs)
        {
            float x = tun.transform.position.x;
            float y = tun.transform.position.z;
            m_Up = m_Up > y ? m_Up : y;
            m_Right = m_Right > x ? m_Right : x;
            m_Down = m_Down < y ? m_Down : y;
            m_Left = m_Left < x ? m_Left : x;
        }

        m_Width = m_Right - m_Left;
        m_Height = m_Up - m_Down;

        foreach (var tun in m_CenterLayerObjs)
        {
            foreach (var tunnelsType in m_TunnelsTypes._tunnelsTypesList)
            {
                if (tun.tag == tunnelsType.name)
                {
                    float x = tun.transform.position.x;
                    float y = tun.transform.position.z;
                    x -= (m_Width / 2);
                    x *= (m_RectWidth - 30) / m_Width;
                    y *= (m_RectHeight - 30) / m_Height;

                    CreateImage(x, y, tunnelsType.width, tunnelsType.height, tunnelsType.sprite);
                }
            }
        }

        m_PlayerPosition = m_Player.position;
        m_PlayerMap = new GameObject("playerImg");
        m_PlayerMap.transform.SetParent(m_Rect, false);

        Image imageComponent = m_PlayerMap.AddComponent<Image>();
        imageComponent.sprite = m_PlayerSprite;

        RectTransform rectTransform = m_PlayerMap.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(10, 10);

        foreach (var ai in m_AiList)
        {
            float x = ai.transform.position.x;
            float y = ai.transform.position.z;
            x -= (m_Width / 2);
            x *= (m_RectWidth - 30) / m_Width;
            y *= (m_RectHeight - 30) / m_Height;

            GameObject imageObject = new GameObject("aiImg");
            imageObject.transform.SetParent(m_Map, false);

            Image aiImageComponent = imageObject.AddComponent<Image>();
            imageComponent.sprite = m_AiSprite;

            RectTransform aiRectTransform = aiImageComponent.GetComponent<RectTransform>();
            aiRectTransform.sizeDelta = new Vector2(5, 5);
            aiRectTransform.anchoredPosition = new Vector2(x, y);

            m_AiMapImg.Add(aiRectTransform);
            imageObject.SetActive(false);
        }

        var end = GameObject.FindGameObjectWithTag("win");
        float endx = end.transform.position.x;
        float endy = end.transform.position.z;
        endx -= (m_Width / 2);
        endx *= (m_RectWidth - 30) / m_Width;
        endy *= (m_RectHeight - 30) / m_Height;

        GameObject endObject = new GameObject("Img");
        endObject.transform.SetParent(m_Map, false);

        Image endComponent = endObject.AddComponent<Image>();
        endComponent.color = Color.green;

        RectTransform endTransform = endObject.GetComponent<RectTransform>();
        endTransform.sizeDelta = new Vector2(10, 10);
        endTransform.anchoredPosition = new Vector2(endx, endy);
    }

    void Update()
    {
        m_PlayerPosition = m_Player.position;
        float x = m_PlayerPosition.x - (m_Width / 2);
        float y = m_PlayerPosition.z;
        x = x * (m_RectWidth - 30) / m_Width;
        y = y * (m_RectHeight - 30) / m_Height;
        m_PlayerMap.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        Quaternion currentRotation = m_PlayerMap.GetComponent<RectTransform>().rotation;
        currentRotation.z = -m_Camera.transform.rotation.y;
        m_PlayerMap.GetComponent<RectTransform>().rotation = currentRotation;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            m_Elapsed += Time.deltaTime;
            float timer;
            float vol;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                timer = 0.5f;
                vol = 1.0f;
            }
            else
            {
                timer = 1.0f;
                vol = 0.4f;
            }

            if (m_Elapsed > timer)
            {
                GameObject imageObject = Instantiate(m_Ring);
                FootSteps src = imageObject.GetComponent<FootSteps>();
                imageObject.transform.SetParent(m_Map, false);
                RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = m_PlayerMap.GetComponent<RectTransform>().anchoredPosition;

                m_Elapsed = 0;
                src.PlayStep(m_source, vol);
            }
        }

        for (int i = 0; i < m_AiList.Length; i++)
        {
            float aix = m_AiList[i].transform.position.x;
            float aiy = m_AiList[i].transform.position.z;
            aix -= (m_Width / 2);
            aix *= (m_RectWidth - 30) / m_Width;
            aiy *= (m_RectHeight - 30) / m_Height;
            m_AiMapImg[i].anchoredPosition = new Vector2(aix, aiy);
            RaycastHit hit;
            var myRay = Physics.Raycast(m_AiList[i].transform.position + Vector3.up,
                (m_Player.transform.position - m_AiList[i].transform.position), out hit);
            if (myRay)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    m_AiList[i].GetComponent<AiBehaviour>().m_Seen = true;
                }
            }

            if (Vector3.Distance(m_AiList[i].transform.position, m_Player.transform.position) < 40.0f && myRay)
            {
                m_TitamElapsed += Time.deltaTime;
                var src = m_AiList[i].GetComponent<AudioSource>();
                if (m_TitamElapsed > 20.0f && !src.isPlaying)
                {
                    m_TitamElapsed = 0.0f;
                    src.clip = m_Growl;
                    src.volume = 1.0f;
                    src.Play();
                }
            }
        }

        m_SoundElapsed += Time.deltaTime;
        if (m_SoundElapsed > 30.0f)
        {
            m_SoundElapsed = 0.0f;
            m_Ambientsource.clip = m_AmbientClips[Random.Range(0, m_AmbientClips.Length)];
            m_Ambientsource.volume = 0.2f;
            m_Ambientsource.Play();
        }

        if (m_MusicElapsed < 100.0f && !m_Music.isPlaying)
        {
            m_MusicElapsed += Time.deltaTime;
        }
        else if (!m_Music.isPlaying)
        {
            m_Music.Play();
        }
        else
        {
            m_MusicElapsed = 0.0f;
        }
    }

    private void CreateImage(float x, float y, float width, float height, Sprite sprite)
    {
        GameObject imageObject = new GameObject("Img");
        imageObject.transform.SetParent(m_Map, false);

        Image imageComponent = imageObject.AddComponent<Image>();
        imageComponent.sprite = sprite;

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = new Vector2(x, y);
        rectTransform.eulerAngles = new Vector3(0, 0, 0);
    }
}