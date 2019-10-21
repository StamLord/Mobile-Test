using UnityEngine;
using System.Collections;

public static class MonoBehaviorExt
{
    public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour obj, IEnumerator coroutine)
    {
        Coroutine<T> coroutineObject = new Coroutine<T>();
        coroutineObject.coroutine = obj.StartCoroutine(coroutineObject.InternalRoutine(coroutine));
        return coroutineObject;
    }
}

public class Coroutine<T> 
{
    public T returnVal;
    public Coroutine coroutine;

    public IEnumerator InternalRoutine(IEnumerator coroutine)
    {
        while(true)
        {
            if(!coroutine.MoveNext())
            {
                yield break;
            }

            object yielded = coroutine.Current;
            if(yielded != null && yielded.GetType() == typeof(T))
            {
                returnVal = (T)yielded;
                yield break;
            }
            else
            {
                yield return coroutine.Current;
            }
        }
    }
}