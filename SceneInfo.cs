using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour
{
    public AudioClip SceneBGM;

    private void Start()
    {
        AudioManager.instance.ChangeMusic(SceneBGM);
    }
}
