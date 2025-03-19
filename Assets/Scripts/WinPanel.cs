using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Lvl1()
    {
        SceneManager.LoadScene("lvl1");

    }
    public void Lvl2()
    {
        SceneManager.LoadScene("SampleScene");

    }
    public void Lvl3()
    {
        SceneManager.LoadScene("lvl3");

    }
    public void Lvl4()
    {
        SceneManager.LoadScene("lvl4");

    }

}
