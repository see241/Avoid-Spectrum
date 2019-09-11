using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public float speed;
    public bool isRevival;

    private delegate void del();

    private del Move;

    public UnityEngine.UI.Image skilCoolBar;
    private Vector2 beginPos;
    private Vector2 lastPos;

    public KeyCode skillKey;

    private Vector2 deltaPos;

    private CircleCollider2D collider2D;

    private SpriteRenderer spriteRenderer;

    public float skilCoolTime;
    private bool isCooltime;
    private bool isMoving;
    public ParticleSystem skilEffct;
    private TrailRenderer trail;
    private bool beginInitFlag;
    private int lastTouchCount;

    [SerializeField]
    private float moveSensitive;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = ShopManager.instance.shapes[PlayerPrefs.GetInt("SavedPlayerShape", 500)];
        collider2D = GetComponent<CircleCollider2D>();
        trail = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        switch (InGameManager.instance.controlType)
        {
            case ControlType.Joystick:
                Move = MoveToJoyStick;
                break;

            case ControlType.Touch:
                Move = MoveToTouch;
                break;
        }
    }

    public void SetMoveSensitive(float v)
    {
        moveSensitive = v;
        PlayerPrefs.SetFloat("MoveSensitive", moveSensitive);
    }

    public float GetMoveSensitive()
    {
        return moveSensitive;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    public void ApplyChangedControlType(ControlType type)
    {
        switch (type)
        {
            case ControlType.Joystick:
                Move = MoveToJoyStick;
                Joystick.instance.gameObject.SetActive(true);
                break;

            case ControlType.Touch:
                Move = MoveToTouch;
                Joystick.instance.gameObject.SetActive(false);
                break;
        }
    }

    internal void SetPlayerShape(ShopItem item)
    {
        GetComponent<SpriteRenderer>().sprite = ShopManager.instance.shapes[item.itemCode];
        PlayerPrefs.SetInt("SavedPlayerShape", item.itemCode);
    }

    public void Init()
    {
        gameObject.SetActive(true);
        transform.position = Vector2.zero;
        trail.enabled = false;
        isCooltime = false;
        skilCoolBar.gameObject.SetActive(false);
        isRevival = false;
        lastPos = Vector2.zero;
    }

    public void RevivalInit()
    {
        lastPos = transform.position;
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
            if (lastTouchCount < Input.touchCount)
            {
                beginInitFlag = true;
                lastPos = transform.position;
            }
            if (touch.phase == TouchPhase.Began)
            {
                beginPos = Camera.main.ScreenToWorldPoint(touch.position);
                if (InGameManager.instance.isPause)
                {
                    beginInitFlag = true;
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (beginInitFlag)
                {
                    beginPos = Camera.main.ScreenToWorldPoint(touch.position);
                    beginInitFlag = false;
                }
                if (!InGameManager.instance.isPause)
                {
                    transform.position = lastPos + ((Vector2)Camera.main.ScreenToWorldPoint(touch.position) - beginPos) * moveSensitive;
                    if (((touch.deltaPosition.x * touch.deltaPosition.x) + (touch.deltaPosition.y * touch.deltaPosition.y)) >= 3)
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngle(transform.position, (Vector2)transform.position + touch.deltaPosition) - 90));
                    if (Vector2.Distance(transform.position, Vector2.zero) > 4.25f)
                    {
                        transform.position = transform.position.normalized * 4.24f;
                    }
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                lastPos = transform.position;
                beginInitFlag = true;
            }
        }
        lastTouchCount = Input.touchCount;
    }

    private float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 v2 = end - start;
        return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
    }

    private void MoveToJoyStick()
    {
        transform.position += (Vector3)(Joystick.instance.moveVec * Time.deltaTime * speed);
        if (Vector2.Distance(transform.position, Vector2.zero) > 4.25f)
        {
            transform.position = transform.position.normalized * 4.24f;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngle(transform.position, (Vector2)transform.position + Joystick.instance.moveVec.normalized) - 90));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        lastPos = Vector2.zero;
        InGameManager.instance.PlayerDie(gameObject);
        gameObject.SetActive(false);
    }

    private IEnumerator Skill(Vector2 dPos)
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

    public void SetPlayerColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        ShopManager.instance.latestColor = GetComponent<SpriteRenderer>().color;
        InGameManager.instance.dieEffect.startColor = color;
    }

    public void _ColorChange(Color c, float t)
    {
        StartCoroutine(ColorChange(c, t));
    }

    public void AdaptTimeScale()
    {
        speed = 3 / Time.timeScale;
    }

    private IEnumerator ColorChange(Color c, float t)
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
            else
            {
                countTime += Time.deltaTime / 2;
                spriteRenderer.color += colorForTime * Time.deltaTime / 2;
            }

            skilCoolBar.fillAmount = spriteRenderer.color.a;
            yield return null;
        }

        spriteRenderer.color = c;
    }
}