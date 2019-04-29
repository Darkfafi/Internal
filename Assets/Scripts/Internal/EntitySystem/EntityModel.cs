using System;
using UnityEngine;

public class EntityModel : BaseModel
{
	private bool _canRegister;

	public EntityModel()
	{
		Initialize(Vector3.zero, Vector3.zero, Vector3.one, true);
	}

	public EntityModel(Vector3 position)
	{
		Initialize(position, Vector3.zero, Vector3.one, true);
	}

	public EntityModel(Vector3 position, Vector3 rotation)
	{
		Initialize(position, rotation, Vector3.one, true);
	}

	public EntityModel(Vector3 position, Vector3 rotation, Vector3 scale)
	{
		Initialize(position, rotation, scale, true);
	}

	public EntityModel(out Action finishSetupCaller)
	{
		Initialize(Vector3.zero, Vector3.zero, Vector3.one, false);
		finishSetupCaller = RegisterSelfAsEntity;
	}

	public EntityModel(Vector3 position, out Action registerAsEntityCaller)
	{
		Initialize(position, Vector3.zero, Vector3.one, false);
		registerAsEntityCaller = RegisterSelfAsEntity;
	}

	public EntityModel(Vector3 position, Vector3 rotation, out Action registerAsEntityCaller)
	{
		Initialize(position, rotation, Vector3.one, false);
		registerAsEntityCaller = RegisterSelfAsEntity;
	}

	public EntityModel(Vector3 position, Vector3 rotation, Vector3 scale, out Action registerAsEntityCaller)
	{
		Initialize(position, rotation, scale, false);
		registerAsEntityCaller = RegisterSelfAsEntity;
	}

	public ModelTags ModelTags
	{
		get; private set;
	}

	public ModelTransform ModelTransform
	{
		get; private set;
	}

	protected override bool ComponentActionValidation(ModelComponents.ModelComponentsAction action, Type componentType, BaseModelComponent componentInstance)
	{
		if(componentType == typeof(ModelTransform))
		{
			if(action == ModelComponents.ModelComponentsAction.RemoveComponent)
			{
				return IsDestroyed;
			}

			if(action == ModelComponents.ModelComponentsAction.AddComponent)
			{
				return ModelTransform == null;
			}
		}

		if(componentType == typeof(ModelTags))
		{
			if(action == ModelComponents.ModelComponentsAction.RemoveComponent)
			{
				return componentInstance != ModelTags || IsDestroyed;
			}
		}

		return true;
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();
		ModelTags = null;
		ModelTransform = null;
	}

	private void Initialize(Vector3 position, Vector3 rotation, Vector3 scale, bool register)
	{
		ModelTransform = AddComponent<ModelTransform>();
		ModelTags = AddComponent<ModelTags>();

		ModelTransform.SetPos(position);
		ModelTransform.SetRot(rotation);
		ModelTransform.SetScale(scale);

		_canRegister = true;

		if(register)
		{
			RegisterSelfAsEntity();
		}
	}

	private void RegisterSelfAsEntity()
	{
		if(_canRegister)
		{
			EntityTracker.Instance.Register(this);
		}

		_canRegister = false;
	}
}