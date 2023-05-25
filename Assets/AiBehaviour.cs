using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AiBehaviour : MonoBehaviour
{
    enum AiState
    {
        Idle,
        Patrol,
        Chase
    }

    private AiState m_State;

    private Animator m_Animator;
    private NavMeshAgent m_NavMeshAgent;

    private Vector3 m_InitialPos;
    [SerializeField] private Vector3 m_SecondPos;
    private bool AtInitPos;

    private float m_PatrolElapsed;

    private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;

    [SerializeField] private AudioClip m_Shout;
    [SerializeField] private AudioClip m_Attack;

    private AudioSource m_Source;

    public bool m_Seen;

    void Start()
    {
        m_Seen = false;
        m_State = AiState.Idle;
        m_Source = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        m_InitialPos = transform.position;
        AtInitPos = true;
        m_PatrolElapsed = 0.0f;

        m_WalkSpeed = m_NavMeshAgent.speed;
    }

    void Update()
    {
        if (m_State == AiState.Idle)
        {
            if (m_Animator.GetBool("Walk") || m_Animator.GetBool("Run"))
            {
                m_Animator.SetBool("Walk", false);
                m_Animator.SetBool("Run", false);
            }

            m_PatrolElapsed += Time.deltaTime;
        }
        else if (m_State == AiState.Patrol)
        {
            if (m_Animator.GetBool("Walk") == false)
            {
                m_Animator.SetBool("Walk", true);
                m_Animator.SetBool("Run", false);
                m_NavMeshAgent.speed = m_WalkSpeed;
            }

            if (Vector3.Distance(transform.position, m_NavMeshAgent.destination) < 2.5f)
            {
                m_State = AiState.Idle;
                m_NavMeshAgent.destination = transform.position;
            }
        }
        else if (m_State == AiState.Chase)
        {
            if (m_Animator.GetBool("Run") == false)
            {
                m_Animator.SetBool("Walk", false);
                m_Animator.SetBool("Run", true);
                m_NavMeshAgent.speed = m_RunSpeed;
                m_Source.clip = m_Attack;
                m_Source.volume = 1.0f;
                m_Source.Play();
            }

            if (Vector3.Distance(transform.position, m_NavMeshAgent.destination) < 2.5f)
            {
                m_State = AiState.Idle;
                m_Animator.SetTrigger("Shout");
                m_Source.clip = m_Shout;
                m_Source.volume = 1.0f;
                m_Source.Play();
                m_NavMeshAgent.destination = transform.position;
            }
        }

        if (m_Seen)
        {
            if (m_PatrolElapsed > 5.0f)
            {
                int chances = Random.Range(0, 3);
                AtInitPos = !AtInitPos;
                if (chances == 0) TakeWalk(AtInitPos ? m_SecondPos : m_InitialPos);
                m_PatrolElapsed = 0.0f;
            }
        }
    }

    public void FollowNoise(Vector3 pos)
    {
        m_NavMeshAgent.SetDestination(pos);
        m_State = AiState.Chase;
    }

    private void TakeWalk(Vector3 _pos)
    {
        m_NavMeshAgent.SetDestination(_pos);
        m_State = AiState.Patrol;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("DeathMenu");
        }
    }
}