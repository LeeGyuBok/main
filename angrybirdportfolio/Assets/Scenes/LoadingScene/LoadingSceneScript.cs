using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneScript : MonoBehaviour
{
   public void Awake()
   {
      StartCoroutine(Loaidng());
   }

   IEnumerator Loaidng()
   {
      yield return new WaitForSeconds(1.5f);
      SceneManager.LoadScene("InGameScene");
   }
}
