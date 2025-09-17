using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartGameManager : MonoBehaviour
{
	[SerializeField] private string sceneName = "Level1";

	public void StartScene()
	{
		SceneManager.LoadScene(sceneName);				
	}
}
