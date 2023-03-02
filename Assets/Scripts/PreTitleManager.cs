using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreTitleManager : MonoBehaviour
{
    // Start is called before the first frame update
     void Start()
    {
        //Request KantanGameBox to acquire saved data
        KantanGameBox.GameGetData();
    }

    void Update()
    {
        //Wait until save data acquisition is complete
        if(KantanGameBox.IsGameGetDataFinish()){
            //Read save data
            PlayerInfo.FromJSON(KantanGameBox.ReadGameData());
            SceneManager.LoadScene("TitleScene");
        }
    }
}
