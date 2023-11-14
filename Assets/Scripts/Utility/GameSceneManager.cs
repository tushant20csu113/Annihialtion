﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfo
{
	public Collider collider = null;
	public CharacterManager characterManager = null;
	public Camera camera = null;
	public CapsuleCollider meleeTrigger = null;
}

// -------------------------------------------------------------------------
// CLASS	:	GameSceneManager
// Desc		:	Singleton class that acts as the scene database
// -------------------------------------------------------------------------
public class GameSceneManager : MonoBehaviour
{
	// Inspector Assigned Variables
	[SerializeField] private ParticleSystem _bloodParticles = null;

	// Statics
	private static GameSceneManager _instance = null;
	public static GameSceneManager instance
	{
		get
		{
			if (_instance == null)
				_instance = (GameSceneManager)FindObjectOfType(typeof(GameSceneManager));
			return _instance;
		}
	}

	// Private
	private Dictionary<int, AIStateMachine> _stateMachines = new Dictionary<int, AIStateMachine>();
	private Dictionary<int, PlayerInfo> _playerInfos = new Dictionary<int, PlayerInfo>();
	private Dictionary<int, InteractiveItem> _interactiveItems = new Dictionary<int, InteractiveItem>();
	private Dictionary<int, MaterialController> _materialControllers = new Dictionary<int, MaterialController>();

	// Properties
	public ParticleSystem bloodParticles { get { return _bloodParticles; } }

	// Public Methods
	// --------------------------------------------------------------------
	// Name	:	RegisterAIStateMachine
	// Desc	:	Stores the passed state machine in the dictionary with
	//			the supplied key
	// --------------------------------------------------------------------
	public void RegisterAIStateMachine(int key, AIStateMachine stateMachine)
	{
		if (!_stateMachines.ContainsKey(key))
		{
			_stateMachines[key] = stateMachine;
		}
	}

	// --------------------------------------------------------------------
	// Name	:	GetAIStateMachine
	// Desc	:	Returns an AI State Machine reference searched on by the
	//			instance ID of an object
	// --------------------------------------------------------------------
	public AIStateMachine GetAIStateMachine(int key)
	{
		AIStateMachine machine = null;
		if (_stateMachines.TryGetValue(key, out machine))
		{
			return machine;
		}

		return null;
	}

	// --------------------------------------------------------------------
	// Name	:	RegisterPlayerInfo
	// Desc	:	Stores the passed PlayerInfo in the dictionary with
	//			the supplied key
	// --------------------------------------------------------------------
	public void RegisterPlayerInfo(int key, PlayerInfo playerInfo)
	{
		if (!_playerInfos.ContainsKey(key))
		{
			_playerInfos[key] = playerInfo;
		}
	}

	// --------------------------------------------------------------------
	// Name	:	GetPlayerInfo
	// Desc	:	Returns a PlayerInfo reference searched on by the
	//			instance ID of an object
	// --------------------------------------------------------------------
	public PlayerInfo GetPlayerInfo(int key)
	{
		PlayerInfo info = null;
		if (_playerInfos.TryGetValue(key, out info))
		{
			return info;
		}

		return null;
	}

	// --------------------------------------------------------------------
	// Name	:	RegisterInteractiveItem
	// Desc	:	Stores the passed Interactive Item reference in the 
	//			dictionary with the supplied key (usually the instanceID of
	//			a collider)
	// --------------------------------------------------------------------
	public void RegisterInteractiveItem(int key, InteractiveItem script)
	{
		if (!_interactiveItems.ContainsKey(key))
		{
			_interactiveItems[key] = script;
		}
	}

	// --------------------------------------------------------------------
	// Name	:	GetInteractiveItem
	// Desc	:	Given a collider instance ID returns the
	//			Interactive Item_Base derived object attached to it.
	// --------------------------------------------------------------------
	public InteractiveItem GetInteractiveItem(int key)
	{
		InteractiveItem item = null;
		_interactiveItems.TryGetValue(key, out item);
		return item;
	}

	public void RegisterMaterialController(int key, MaterialController controller)
	{
		if (!_materialControllers.ContainsKey(key))
		{
			_materialControllers[key] = controller;
		}
	}

	protected void OnDestroy()
	{
		foreach (KeyValuePair<int, MaterialController> controller in _materialControllers)
		{
			controller.Value.OnReset();
		}
	}
}
