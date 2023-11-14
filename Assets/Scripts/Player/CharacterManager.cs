using UnityEngine;
using System.Collections;
using System;

public class CharacterManager : MonoBehaviour
{
	// Inspector Assigned
	[SerializeField] private CapsuleCollider _meleeTrigger = null;
	[SerializeField] private CameraBloodEffect _cameraBloodEffect = null;
	[SerializeField] private SphereCollider _soundEmitter = null;
	[SerializeField] private Camera _camera = null;
	[SerializeField] private int _gunDamage = 25;

	[SerializeField] private float _health = 100.0f;
	[SerializeField] private PlayerHUD _playerHUD = null;
	[SerializeField] private float _tauntRadius = 10.0f;

	// Private
	private Collider _collider = null;
	
	private FPSController _fpsController = null;
	private CharacterController _characterController = null;
	private GameSceneManager _gameSceneManager = null;
	private int _aiBodyPartLayer = -1;
	private int _interactiveMask = 0;
	private float _nextTauntTime = 0;
	private float _initialTauntRadius = 0;
	private AudioClip _tauntSounds = null;

	public float health { get { return _health; } }
	public float stamina { get { return _fpsController != null ? _fpsController.stamina : 0.0f; } }
	// Use this for initialization
	void Start()
	{
		_collider = GetComponent<Collider>();
		_fpsController = GetComponent<FPSController>();
		_characterController = GetComponent<CharacterController>();
		_gameSceneManager = GameSceneManager.instance;
		_tauntSounds = _soundEmitter.transform.GetComponent<AudioClip>();
		_initialTauntRadius = _soundEmitter.radius;

		_aiBodyPartLayer = LayerMask.NameToLayer("AI Body Part");
		_interactiveMask = 1 << LayerMask.NameToLayer("Interactive");

		if (_gameSceneManager != null)
		{
			PlayerInfo info = new PlayerInfo();
			info.camera = _camera;
			info.characterManager = this;
			info.collider = _collider;
			info.meleeTrigger = _meleeTrigger;

			_gameSceneManager.RegisterPlayerInfo(_collider.GetInstanceID(), info);
		}
		// Get rid of really annoying mouse cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		// Start fading in
		if (_playerHUD) _playerHUD.Fade(2.0f, ScreenFadeType.FadeIn);
	}

	public void TakeDamage(float amount)
	{
		_health = Mathf.Max(_health - (amount * Time.deltaTime), 0.0f);
		if (_cameraBloodEffect != null)
		{
			_cameraBloodEffect.minBloodAmount = (1.0f - _health / 100.0f);
			_cameraBloodEffect.bloodAmount = Mathf.Min(_cameraBloodEffect.minBloodAmount + 0.3f, 1.0f);
		}
	}

	public void DoDamage(int hitDirection = 0)
	{
		if (_camera == null) return;
		if (_gameSceneManager == null) return;

		// Local Variables
		Ray ray;
		RaycastHit hit;
		bool isSomethingHit = false;

		ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

		isSomethingHit = Physics.Raycast(ray, out hit, 1000.0f, 1 << _aiBodyPartLayer);
		//Debug.Log(hit.rigidbody.GetInstanceID());
		if (isSomethingHit)
		{	
			//Destroy(hit.transform.gameObject);
            AIStateMachine stateMachine = _gameSceneManager.GetAIStateMachine(hit.rigidbody.GetInstanceID());
			if (stateMachine)
            {
				stateMachine.TakeDamage(hit.point, ray.direction * 1.0f, _gunDamage, hit.rigidbody, this, 0);
            }
        }
		if(health<=0f)
        {
			DoDeath();
        }

	}

	void Update()
	{
		Ray ray;
		RaycastHit hit;
		RaycastHit[] hits;

		// PROCESS INTERACTIVE OBJECTS
		// Is the crosshair over a usuable item or descriptive item...first get ray from centre of screen
		ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

		// Calculate Ray Length
		float rayLength = Mathf.Lerp(1.0f, 1.8f, Mathf.Abs(Vector3.Dot(_camera.transform.forward, Vector3.up)));

		// Cast Ray and collect ALL hits
		hits = Physics.RaycastAll(ray, rayLength, _interactiveMask);

		// Process the hits for the one with the highest priorty
		if (hits.Length > 0)
		{
			// Used to record the index of the highest priorty
			int highestPriority = int.MinValue;
			InteractiveItem priorityObject = null;

			// Iterate through each hit
			for (int i = 0; i < hits.Length; i++)
			{
				// Process next hit
				hit = hits[i];

				// Fetch its InteractiveItem script from the database
				InteractiveItem interactiveObject = _gameSceneManager.GetInteractiveItem(hit.collider.GetInstanceID());

				// If this is the highest priority object so far then remember it
				if (interactiveObject != null && interactiveObject.priority > highestPriority)
				{
					priorityObject = interactiveObject;
					highestPriority = priorityObject.priority;
				}
			}

			// If we found an object then display its text and process any possible activation
			if (priorityObject != null)
			{
				if (_playerHUD)
					_playerHUD.SetInteractionText(priorityObject.GetText());

				if (Input.GetButtonDown("Use"))
				{
					priorityObject.Activate(this);
				}
			}
		}
		else
		{
			if (_playerHUD)
				_playerHUD.SetInteractionText(null);
		}

		if (Input.GetMouseButtonDown(0))
		{
			DoDamage();
		}
		if (Input.GetMouseButtonDown(1))
		{
			Debug.Log("Right Mouse button");
			DoTaunt();
		}
		if (_playerHUD) _playerHUD.Invalidate(this);

	}

    private void DoTaunt()
    {
		//if (_tauntSounds==null || Time.time<_nextTauntTime)return;
		if (_soundEmitter != null)
		{
			_soundEmitter.radius = _tauntRadius;
			Invoke("ResetTauntRadius", 1);
			AudioSource audioSource = _soundEmitter.GetComponent<AudioSource>();
			audioSource.Play();
		}


        //_nextTauntTime = Time.time + _tauntSounds.length;
    }
    private void ResetTauntRadius()
    {
		_soundEmitter.radius = _initialTauntRadius;

	}
    public void DoLevelComplete()
	{
		if (_fpsController)
			_fpsController.freezeMovement = true;

		if (_playerHUD)
		{
			_playerHUD.Fade(4.0f, ScreenFadeType.FadeOut);
			_playerHUD.ShowMissionText("Mission Completed");
			_playerHUD.Invalidate(this);
		}

		Invoke("GameOver", 4.0f);
	}
	public void DoDeath()
	{
		if (_fpsController)
			_fpsController.freezeMovement = true;

		if (_playerHUD)
		{
			_playerHUD.Fade(3.0f, ScreenFadeType.FadeOut);
			_playerHUD.ShowMissionText("Mission Failed");
			_playerHUD.Invalidate(this);
		}

		Invoke("GameOver", 3.0f);
	}
	void GameOver()
	{
		// Show the cursor again
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		if (ApplicationManager.instance)
			ApplicationManager.instance.LoadMainMenu();
	}
}
