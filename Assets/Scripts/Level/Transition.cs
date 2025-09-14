using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Transition : MonoBehaviour
{
    static Transition Singleton;
    [SerializeField] Image targetImage;

    void Start()
    {
        if( Singleton == null )
        {
            Singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private static void DoSceneLoad<T>( T sceneIdentifier )
    {
        switch( sceneIdentifier )
        {
            case string:
                SceneManager.LoadScene(sceneIdentifier as string);
                break;
            case int:
                int? identifier = sceneIdentifier as int?;
                if( identifier.HasValue )
                {
                    SceneManager.LoadScene(identifier.Value);
                }
                break;
        }
    }

    public static void ToScene<T>( T sceneIdentifier )
    {
        if( Singleton != null )
        {
            Singleton.StartCoroutine(Singleton.DoTransition(sceneIdentifier));
        }
        else
        {
            DoSceneLoad(sceneIdentifier);
        }
    }

    public IEnumerator DoTransition<T>( T sceneIdentifier )
    {
        var elapsedTime = 0f;
        var targetTime = .3f;

        Singleton.targetImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        // Start Transition
        while( elapsedTime < targetTime )
        {
            Singleton.targetImage.fillAmount = Mathf.Lerp(0, 1, elapsedTime / targetTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        DoSceneLoad(sceneIdentifier);

        Singleton.targetImage.fillOrigin = (int)Image.OriginHorizontal.Right;
        // Start Transition
        elapsedTime = 0f;
        while( elapsedTime < targetTime )
        {
            Singleton.targetImage.fillAmount = Mathf.Lerp(1, 0, elapsedTime / targetTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Singleton.targetImage.fillAmount = 0;
    }
}