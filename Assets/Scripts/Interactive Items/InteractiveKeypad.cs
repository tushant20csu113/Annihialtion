using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveKeypad : InteractiveItem
{
	[SerializeField] protected Transform 			door 		= null;
	[SerializeField] protected List<MaterialController> _materialControllers = new List<MaterialController>();
	[SerializeField] protected GameObject			LightOff    = null;
	[SerializeField] protected GameObject			LightOn		= null;
	//[SerializeField] protected AudioCollection		_collection		= null;
	[SerializeField] protected int					_bank			= 0;
	[SerializeField] protected float				_activationDelay= 0.0f;	

	bool _isActivated	=	false;

    protected override void Start()
    {
        base.Start();
		// Activate Material Controller
		for (int i = 0; i < _materialControllers.Count; i++)
		{

			if (_materialControllers[i] != null)
			{
				_materialControllers[i].OnStart();
			}
		}
	}
    public override string GetText ()
	{
		
		ApplicationManager appDatabase = ApplicationManager.instance;
		if (!appDatabase) return string.Empty;

		string powerState 		= appDatabase.GetGameState("POWER");
		string lockdownState	= appDatabase.GetGameState("LOCKDOWN");
		string accessCodeState	= appDatabase.GetGameState("ACCESSCODE");

		// If we have not turned on the power
		if ( string.IsNullOrEmpty( powerState ) || !powerState.Equals("TRUE"))
		{
			return "Keypad : No Power";
		}
		else
		// Or we have not deactivated lockdown
		if ( string.IsNullOrEmpty( lockdownState ) || !lockdownState.Equals("FALSE"))
		{
			return "Keypad : Under Lockdown";
		}
		else
		// or we don't have the access code yet
		if ( string.IsNullOrEmpty( accessCodeState ) || !accessCodeState.Equals("TRUE"))
		{
			return "Keypad : Access Code Required";
		}

		// We have everything we need
		return "Keypad";
	}

	public override void Activate( CharacterManager characterManager )
	{
		if (_isActivated) return;
		ApplicationManager appDatabase = ApplicationManager.instance;
		if (!appDatabase) return;
	
		string powerState 		= appDatabase.GetGameState("POWER");
		string lockdownState	= appDatabase.GetGameState("LOCKDOWN");
		string accessCodeState	= appDatabase.GetGameState("ACCESSCODE");

		if ( string.IsNullOrEmpty( powerState ) || !powerState.Equals("TRUE")) 				return;
		if ( string.IsNullOrEmpty( lockdownState ) || !lockdownState.Equals("FALSE"))		return;
		if ( string.IsNullOrEmpty( accessCodeState ) || !accessCodeState.Equals("TRUE"))	return;

		// Delay the actual animation for the desired number of seconds
		StartCoroutine ( DoDelayedActivation( characterManager ));

		_isActivated = true;
	}

	protected IEnumerator DoDelayedActivation( CharacterManager characterManager)
	{
		if (!door) yield break;;

		/*// Play the sound
		if (_collection!=null)
		{
			AudioClip clip = _collection[ _bank ];
			if (clip)
			{
				if (AudioManager.instance)
					AudioManager.instance.PlayOneShotSound( _collection.audioGroup, 
															clip,
															_elevator.position, 
															_collection.volume, 
															_collection.spatialBlend,
															_collection.priority );
				
			}
		}*/

		// Wait for the desired delay
		yield return new WaitForSeconds( _activationDelay );

		// If we have a character manager
		if (characterManager!=null)
		{
			// Make it a child of the elevator
			//characterManager.transform.parent = door;
			for (int i = 0; i < _materialControllers.Count; i++)
			{

				if (_materialControllers[i] != null)
				{
					_materialControllers[i].Activate(true);
				}
			}
			LightOff.SetActive(false);
			LightOn.SetActive(true);
			// Get the animator of the elevator and activate it animation
			Animator animator = door.GetComponent<Animator>();
			if (animator) animator.SetTrigger( "Activate");

			/*// Freeze the FPS motor so it can rotate/jump/croach but can
			// not move off of the elevator.
			if (characterManager.fpsController)
			{
				characterManager.fpsController.freezeMovement = true;
			}*/
		}
	}
}
