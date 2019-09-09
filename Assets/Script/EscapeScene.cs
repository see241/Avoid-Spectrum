using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeScene : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            switch (InGameManager.instance.state)
            {
                case GameState.Main:
                    Application.Quit();
                    break;

                case GameState.Menu:
                    InGameManager.instance.state = GameState.Main;
                    break;

                case GameState.InGame:
                    if (InGameManager.instance.isPause)
                    {
                        UIManager.instance.BackHome();
                    }
                    else
                    {
                        UIManager.instance.Pause();
                    }
                    break;

                case GameState.Shop:
                    InGameManager.instance.state = GameState.Menu;
                    break;

                case GameState.Option:
                    InGameManager.instance.state = GameState.Menu;
                    break;
            }
    }
}