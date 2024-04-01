using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInitializer : MonoBehaviour
{
    private void Awake()
    {
        TurnMangaer.SetNumberOfPlayer(1);
        TurnMangaer.SetPlayerNames(new List<string>() { GameInfo.GetAIName });
    }
}
