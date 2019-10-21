using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public delegate void buttonADelegate();
    public static event buttonADelegate onButtonA;

    public delegate void buttonBDelegate();
    public static event buttonADelegate onButtonB;

    public delegate void buttonCDelegate();
    public static event buttonCDelegate onButtonC;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            ButtonAPress();
            
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            ButtonBPress();

        else if(Input.GetKeyDown(KeyCode.Space))
            ButtonCPress();
    }

    public void ButtonAPress()
    {
        if(onButtonA != null)
            onButtonA();
    }

    public void ButtonBPress()
    {
        if(onButtonB != null)
            onButtonB();
    }

    public void ButtonCPress()
    {
        if(onButtonC != null)
            onButtonC();
    }
}
