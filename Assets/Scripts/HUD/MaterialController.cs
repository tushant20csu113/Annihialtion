using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialController 
{
	// Inspector Assigned
	[SerializeField] protected Material		Material			=	null;

	[SerializeField] protected Texture		_diffuseTexture		=	null;		
	[SerializeField] protected Color		_diffuseColor		=	Color.white;
	[SerializeField] protected Texture		_normalMap			=	null;
	[SerializeField] protected float		_normalStrength		=	1.0f;

	[SerializeField] protected Texture		_emissiveTexture		=	null;
	[SerializeField] protected Color		_emissionColor		=	Color.black;
	[SerializeField] protected float		_emissionScale		=	1.0f;

	// Private / Protected
	protected MaterialController _backup	=	null;
	protected bool				 _started	=	false;

	// Property to fetch the underlying material
	public Material material{ get{ return Material;}}

	public void			OnStart()
	{
		if (Material==null || _started) return;

		_started = true;
		_backup = new MaterialController();

		// Backup settings in a temp controller
		_backup._diffuseColor 		= Material.GetColor("_Color");
		_backup._diffuseTexture		= Material.GetTexture("_MainTex");
		_backup._emissionColor		= Material.GetColor("_EmissionColor") ;
		_backup._emissionScale		= 1;
		_backup._emissiveTexture	= Material.GetTexture("_EmissionMap");
		_backup._normalMap			= Material.GetTexture("_BumpMap");
		_backup._normalStrength		= Material.GetFloat("_BumpScale");

		// Register this controller with the game scene manager using material instance ID. The GameScene manager will reset
		// all registered materials when the scene closes
		if (GameSceneManager.instance) GameSceneManager.instance.RegisterMaterialController( Material.GetInstanceID(), this );
	}

	public void Activate( bool activate )
	{
		// Can't call this function until it's start has been called
		if (!_started || Material==null) return;

		// Set the material to the assigned properties
		if (activate)
		{
			Material.SetColor	("_Color", _diffuseColor);
			Material.SetTexture	("_MainTex", _diffuseTexture );
			Material.SetColor	("_EmissionColor", _emissionColor * _emissionScale );
			Material.SetTexture	("_EmissionMap", _emissiveTexture );
			Material.SetTexture ("_BumpMap", _normalMap );
			Material.SetFloat   ("_BumpScale", _normalStrength);
		}
		else
		{
			Material.SetColor	("_Color", _backup._diffuseColor);
			Material.SetTexture	("_MainTex", _backup._diffuseTexture );
			Material.SetColor	("_EmissionColor", _backup._emissionColor * _backup._emissionScale );
			Material.SetTexture	("_EmissionMap", _backup._emissiveTexture );
			Material.SetTexture ("_BumpMap", _backup._normalMap );
			Material.SetFloat   ("_BumpScale", _backup._normalStrength);
		}
	}

	// ------------------------------------------------------------------------------------------------
	// Name	:	OnReset
	// Desc	:	Called to reset the material. This should be called only by the game scene manager
	//			otherwise you could overwrite the properties of your material asset
	// ------------------------------------------------------------------------------------------------
	public void 		OnReset()
	{

		if (_backup==null || Material==null) return;

		Material.SetColor	("_Color", 			_backup._diffuseColor);
		Material.SetTexture	("_MainTex", 		_backup._diffuseTexture);
		Material.SetColor	("_EmissionColor", 	_backup._emissionColor * _backup._emissionScale);
		Material.SetTexture	("_EmissionMap", 	_backup._emissiveTexture);
		Material.SetTexture ("_BumpMap", _backup._normalMap );
		Material.SetFloat   ("_BumpScale", _backup._normalStrength);
	}

	// ------------------------------------------------------------------------------------------------
	// Name	:	GetInstanceID
	// Desc	:	Returns the instance ID of the managed material
	// ------------------------------------------------------------------------------------------------
	public int GetInstanceID()
	{
		if (Material==null) return -1;
		return Material.GetInstanceID();
	}
	
}
