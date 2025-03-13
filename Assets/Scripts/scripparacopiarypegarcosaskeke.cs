using UnityEngine;

public class scripparacopiarypegarcosaskeke : MonoBehaviour
{
    [SerializeField]
    private GameObject winpanel;
    private bool isWin;
    [SerializeField]
    private GameObject vfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void MirarWin()
    {

        if (isWin==true)
        {
            winpanel.SetActive(true);
        }

    }
}
