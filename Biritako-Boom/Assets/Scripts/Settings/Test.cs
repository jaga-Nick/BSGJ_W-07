using Cysharp.Threading.Tasks;
using Setting;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            AudioManager.Instance().LoadBgm("BgmTitle");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AudioManager.Instance().LoadSoundEffect("BgmTitle");
        }
    }
}
