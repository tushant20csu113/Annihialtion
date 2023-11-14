using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObjective : MonoBehaviour 
{
	void OnTriggerEnter( Collider col )
	{
		if (GameSceneManager.instance)
		{
			PlayerInfo playerInfo = GameSceneManager.instance.GetPlayerInfo( col.GetInstanceID());
			if (playerInfo!=null)
				playerInfo.characterManager.DoLevelComplete();
		}
	}
}
