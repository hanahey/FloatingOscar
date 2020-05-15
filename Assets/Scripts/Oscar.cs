using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscar : MonoBehaviour
{
    private const float OSCAR_SCREEN_SCALE = 0.08f;
    private const float SCREEN_LOWEST_Y_POSITION = -50f; //based on camera ortho size of 50
    private const float TREE_BODY_WIDTH = 5f;

    private static Oscar instance;
    public static Oscar GetInstance()
    {
        return instance;
    }

    private Rigidbody2D OscarRigidbody2D;

    private SpriteRenderer OscarSprite;

    public event EventHandler OnHit;

    public event EventHandler OnStartPlaying;

    private float moveSpeed = 30f;

    private enum State
    {
        WaitingToStart,
        Playing,
        Hit,
    }

    private State state;

    private void Awake()
    {
        instance = this;
        OscarRigidbody2D = GetComponent<Rigidbody2D>();
        OscarSprite = GetComponent<SpriteRenderer>();
        //Static when state is 'waiting to start'
        OscarRigidbody2D.bodyType = RigidbodyType2D.Static;

    }

    private void Start()
    {
        ScaleOscar();
        GetOscarSpawnPosition();
    }


    private void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
                    Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    state = State.Playing;
                    OscarRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    OnStartPlaying?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    MoveLeft();
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    MoveRight();
                }
                else if (Input.touchCount > 0)
                {
                    MoveByTouch();
                }
                else if (Input.GetKey(KeyCode.Mouse0))
                {
                    MoveMouse();
                }
                PreventFallingOffSides();
                break;
            case State.Hit:
                break;

        }
    }

    private void GetOscarSpawnPosition()
    {
        // check if the camera is on landscape or portrait
        if (Level.GetInstance().GetCameraHeight() > Level.GetInstance().GetCameraHalfWidth() * 2)
        {
            //spawn Oscar a little higher on a portrait screen to allow touch input below him
            OscarSprite.transform.position = new Vector2(0f,
                SCREEN_LOWEST_Y_POSITION + GetOscarHeight() * 2);
        }
        else
        {
            // spawn Oscar lower on a landscape screen
            OscarSprite.transform.position = new Vector2(0f,
                SCREEN_LOWEST_Y_POSITION + GetOscarHeight());
        }
    }

    private void MoveRight()
    {
        OscarSprite.transform.position += new Vector3(1.0f, 0.0f, 0.0f * moveSpeed);
        OscarSprite.flipX = false; //by default, the Oscar sprite is looking to the right
    }

    private void MoveLeft()
    {
        OscarSprite.transform.position += new Vector3(-1.0f, 0.0f, 0.0f * moveSpeed);
        //OscarSprite.transform.position += Vector3.left * MOVE_SPEED * Time.deltaTime;
        OscarSprite.flipX = true; //make Oscar sprite look left
    }

    private void MoveByTouch()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

        if (touchPosition.x > OscarSprite.transform.position.x)
        {
            MoveRight();
        }
        else if (touchPosition.x < OscarSprite.transform.position.x)
        {
            MoveLeft();
        }
    }

    private void MoveMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x > OscarSprite.transform.position.x)
        {
            MoveRight();
        }
        else if (mousePosition.x < OscarSprite.transform.position.x)
        {
            MoveLeft();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        OscarRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Whine);
        OnHit?.Invoke(this, EventArgs.Empty);
    }

    private void ScaleOscar()
    {
        float width = Level.GetInstance().GetCameraHalfWidth();
        transform.localScale = Vector3.one * width * OSCAR_SCREEN_SCALE;
    }

    public float GetOscarWidth()
    {
        return OscarSprite.size.x * transform.localScale.x;
    }

    public float GetOscarHeight()
    {
        return OscarSprite.size.y * transform.localScale.y;
    }

    public float GetOscarYPosition()
    {
        return OscarSprite.transform.position.y;
    }

    private void PreventFallingOffSides()
    {
        Vector3 currentPosition = OscarSprite.transform.position;
        // clamp extreme left and right x values of screen
        float clampedX =
           Mathf.Clamp(currentPosition.x,
          -(Level.GetInstance().GetCameraHalfWidth() - (GetOscarWidth() * .5f) - TREE_BODY_WIDTH),
          Level.GetInstance().GetCameraHalfWidth() - (GetOscarWidth() * 0.5f) - TREE_BODY_WIDTH);

        if (!Mathf.Approximately(clampedX, currentPosition.x))
        {
            currentPosition.x = clampedX;
            transform.position = currentPosition;
        }
    }

    public void SetOscarMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }
}
