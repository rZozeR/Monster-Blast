using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void ChangeScene(int _index)
    {
        SceneManager.LoadScene(_index + 1);
    }
}
