using System.Collections.Generic;
using UnityEngine;

public class TurnSetterForDebug : MonoBehaviour
{
    [SerializeField] string[] names;
    private void Awake()
    {
        TurnMangaer.SetNumberOfPlayer(names.Length);
        TurnMangaer.SetPlayerNames(new List<string>(names));
    }
}
