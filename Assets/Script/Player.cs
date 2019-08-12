﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public float speed;

    private delegate void del();

    private del Move;

    public UnityEngine.UI.Image skilCoolBar;
    private Vector2 beginPos;
    private Vector2 lastPos;

    public KeyCode skillKey;

    private Vector2 deltaPos;

    CircleCollider2D collider2D;

    SpriteRenderer spriteRenderer;

    public float skilCoolTime;
    bool isCooltime;
    bool isMoving;
    public ParticleSystem skilEffct;
    TrailRenderer trail;
    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        collider2D = GetComponent<CircleCollider2D>();
        trail = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (InGameManager.instance.controlType == ControlType.Joystick)
            Move = MoveToJoyStick;
        else
            Move = MoveToKeyboard;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }
    public void Init()
    {
        gameObject.SetActive(true);
        transform.position = Vector2.zero;
        trail.enabled = false;
        isCooltime = false;
        skilCoolBar.gameObject.SetActive(false);
        spriteRenderer.color = new Color32(47, 252, 253, 255);
    }
    private void MoveToKeyboard()
    {
        float vX = 0, vY = 0;

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            isMoving = false;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            isMoving = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            isMoving = false;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            isMoving = false;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            vY = 1;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            vY = -1;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            vX = -1;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            vX = 1;
            isMoving = true;
        }

        transform.Translate(new Vector2(vX, vY).normalized * Time.deltaTime * speed);
        if (Vector2.Distance(transform.position, Vector2.zero) > 4.4)
        {
            InGameManager.instance.PlayerDie(gameObject);
            gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(skillKey))
        {
            if (vX != 0 || vY != 0)
            {
                if (!isCooltime)
                {
                    StartCoroutine(Skill(new Vector2(vX, vY)));
                }
            }
        }
    }

    private void MoveToTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                beginPos = Camera.main.ScreenToWorldPoint(touch.position);
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (!InGameManager.instance.isPause)
                {
                    transform.position = lastPos + (Vector2)Camera.main.ScreenToWorldPoint(touch.position) - beginPos;
                    if (Vector2.Distance(transform.position, Vector2.zero) > 4.25f)
                    {
                        transform.position = transform.position.normalized * 4.24f;
                    }
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                lastPos = transform.position;
            }
        }
    }

    private void MoveToJoyStick()
    {
        transform.Translate(Joystick.instance.moveVec * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InGameManager.instance.PlayerDie(gameObject);
        gameObject.SetActive(false);
    }
    IEnumerator Skill(Vector2 dPos)
    {
        Color startColor = spriteRenderer.color;
        trail.enabled = true;
        isCooltime = true;
        collider2D.enabled = false;
        skilEffct.transform.position = transform.position;
        skilEffct.transform.LookAt((Vector2)transform.position + dPos);
        skilEffct.transform.Rotate(180, 0, 0);
        ParticleSystem eff = Instantiate(skilEffct);
        spriteRenderer.color = new Color32(0, 255, 130, 0);
        Destroy(eff.gameObject, eff.duration + eff.startLifetime);
        for (int i = 0; i < 15; i++)
        {
            transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + dPos.normalized * 0.1f, 0.75f);
            yield return new WaitForFixedUpdate();
        }
        collider2D.enabled = true;
        trail.enabled = false;
        yield return StartCoroutine(ColorChange(startColor, skilCoolTime));
        skilCoolBar.gameObject.SetActive(false);
        isCooltime = false;
    }
    IEnumerator ColorChange(Color c, float t)
    {

        skilCoolBar.gameObject.SetActive(true);
        Color colorForTime = (c - spriteRenderer.color) / t;
        float startTime = Time.time;
        float countTime = Time.time;
        while (startTime + t > countTime)
        {
            if (isMoving)
            {
                countTime += Time.deltaTime;
                spriteRenderer.color += colorForTime * Time.deltaTime;
            }
            else{
                countTime += Time.deltaTime/2;
                spriteRenderer.color += colorForTime * Time.deltaTime/2;
            }
            
            skilCoolBar.fillAmount = spriteRenderer.color.a;
            yield return null;
        }

        spriteRenderer.color = c;
    }
}