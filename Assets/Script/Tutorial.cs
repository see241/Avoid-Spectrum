using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private IEnumerator iTutorial;

    private void Awake()
    {
        iTutorial = StartTutorial();
    }

    private void OnEnable()
    {
        StartCoroutine(iTutorial);
    }

    private void OnDisable()
    {
        StopCoroutine(iTutorial);
        iTutorial = StartTutorial();
    }

    private IEnumerator StartTutorial()
    {
        yield return null;
    }
}