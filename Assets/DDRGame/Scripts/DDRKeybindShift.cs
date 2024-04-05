using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDRKeybindShift : MonoBehaviour
{
    public SpriteRenderer leftKey;
    public SpriteRenderer rightKey;
    public SpriteRenderer upKey;
    public SpriteRenderer downKey;

    public Sprite leftA;
    public Sprite rightA;
    public Sprite upA;
    public Sprite downA;

    public Sprite leftB;
    public Sprite rightB;
    public Sprite upB;
    public Sprite downB;

    public bool ArrowKeys;

    // Start is called before the first frame update
    void Start()
    {
        ArrowKeys = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(ArrowKeys == true)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                leftKey.sprite = leftB;
                rightKey.sprite = rightB;
                upKey.sprite = upB;
                downKey.sprite = downB;
                ArrowKeys = false;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                leftKey.sprite = leftB;
                rightKey.sprite = rightB;
                upKey.sprite = upB;
                downKey.sprite = downB;
                ArrowKeys = false;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                leftKey.sprite = leftB;
                rightKey.sprite = rightB;
                upKey.sprite = upB;
                downKey.sprite = downB;
                ArrowKeys = false;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                leftKey.sprite = leftB;
                rightKey.sprite = rightB;
                upKey.sprite = upB;
                downKey.sprite = downB;
                ArrowKeys = false;
            }
        }
  
        if(ArrowKeys != true)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                leftKey.sprite = leftA;
                rightKey.sprite = rightA;
                upKey.sprite = upA;
                downKey.sprite = downA;
                ArrowKeys = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                leftKey.sprite = leftA;
                rightKey.sprite = rightA;
                upKey.sprite = upA;
                downKey.sprite = downA;
                ArrowKeys = true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                leftKey.sprite = leftA;
                rightKey.sprite = rightA;
                upKey.sprite = upA;
                downKey.sprite = downA;
                ArrowKeys = true;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                leftKey.sprite = leftA;
                rightKey.sprite = rightA;
                upKey.sprite = upA;
                downKey.sprite = downA;
                ArrowKeys = true;
            }
        }
 
    }
}
