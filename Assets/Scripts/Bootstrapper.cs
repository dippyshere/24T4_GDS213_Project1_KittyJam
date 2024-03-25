using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrapper : MonoBehaviour
{
    private void Awake()
    {
        Addressables.LoadSceneAsync("Assets/Scenes/Menu.unity");
    }
}
