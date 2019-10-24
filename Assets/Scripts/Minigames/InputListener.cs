using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public delegate void buttonADelegate();
    public static event buttonADelegate onButtonA;

    public delegate void buttonBDelegate();
    public static event buttonBDelegate onButtonB;

    public delegate void buttonCDelegate();
    public static event buttonCDelegate onButtonC;

    public delegate void buttonAUpDelegate();
    public static event buttonAUpDelegate onButtonAUp;

    public delegate void buttonBUpDelegate();
    public static event buttonBUpDelegate onButtonBUp;

    public delegate void buttonCUpDelegate();
    public static event buttonCUpDelegate onButtonCUp;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            ButtonAPress();
        
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            ButtonBPress();

        else if(Input.GetKeyDown(KeyCode.Space))
            ButtonCPress();

        if(Input.GetKeyUp(KeyCode.LeftArrow))
            ButtonAUp();

        if(Input.GetKeyUp(KeyCode.RightArrow))
            ButtonBUp();

        if(Input.GetKeyUp(KeyCode.Space))
            ButtonCUp();
    }

    public void ButtonAPress()
    {
        if(onButtonA != null)
            onButtonA();
    }

    public void ButtonAUp()
    {
        if(onButtonAUp != null)
            onButtonAUp();
    }

    public void ButtonBPress()
    {
        if(onButtonB != null)
            onButtonB();
    }

    public void ButtonBUp()
    {
        if(onButtonBUp != null)
            onButtonBUp();
    }

    public void ButtonCPress()
    {
        if(onButtonC != null)
            onButtonC();
    }

    public void ButtonCUp()
    {
        if(onButtonCUp != null)
            onButtonCUp();
    }
}
