using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityBehaviour : ScriptableObject
{
	public enum SpeedRequirement
	{
		None = 0,
		Less = 1,
		Greater = 2
	}

	public enum GroundRequirement
	{
		None = 0,
		Grounded = 1,
		Free = 2
	}

	[SerializeField]
	private string _displayName = "Ability";

	[SerializeField]
	private Sprite _icon;

	[SerializeField]
	private string _description = "Description";

	[SerializeField]
	private TagContainer _tags = new TagContainer();

	[SerializeField]
	private TagContainer _blockedByTags = new TagContainer();

	[SerializeField]
	private TagContainer _cancelWithTags = new TagContainer();

	[SerializeField]
	private TagContainer _requiredTags = new TagContainer();

	[SerializeField]
	private List<VitalRequirement> _vitalRequirements = new List<VitalRequirement>();

	[SerializeField]
	private List<ItemRequirement> _itemRequirements = new List<ItemRequirement>();

	[SerializeField]
	private GroundRequirement _groundRequirement;

	[SerializeField]
	private SpeedRequirement _speedRequirement;

	[SerializeField]
	private float _speedRequirementValue;

	[SerializeField]
	private bool _canCancel;

	[SerializeField]
	private bool _canInterrupt;

	public string displayName
	{
		get
		{
			return _displayName;
		}
	}

	public Sprite icon
	{
		get
		{
			return _icon;
		}
	}

	public string description
	{
		get
		{
			return _description;
		}
	}

	public TagContainer tags
	{
		get
		{
			return _tags;
		}
	}

	public TagContainer blockedByTags
	{
		get
		{
			return _blockedByTags;
		}
	}

	public TagContainer cancelWithTags
	{
		get
		{
			return _cancelWithTags;
		}
	}

	public TagContainer requiredTags
	{
		get
		{
			return _requiredTags;
		}
	}

	public List<VitalRequirement> vitalRequirements
	{
		get
		{
			return _vitalRequirements;
		}
	}

	public List<ItemRequirement> itemRequirements
	{
		get
		{
			return _itemRequirements;
		}
	}

	public GroundRequirement groundRequirement
	{
		get
		{
			return _groundRequirement;
		}
	}

	public SpeedRequirement speedRequirement
	{
		get
		{
			return _speedRequirement;
		}
	}

	public float speedRequirementValue
	{
		get
		{
			return Mathf.Clamp(_speedRequirementValue, 0f, float.MaxValue);
		}
	}

	public bool canCancel
	{
		get
		{
			return _canCancel;
		}
	}

	public bool canInterrupt
	{
		get
		{
			return _canInterrupt;
		}
	}

	public virtual bool IsTriggered(Ability o)
	{
		return false;
	}

	public bool HasRequiredVitals(Ability o)
	{
		if (!o.actor)
		{
			return false;
		}
		if (vitalRequirements.Count > 0 && !o.actor.statistics)
		{
			return false;
		}
		return true;
	}

	public bool HasRequiredItems(Ability o)
	{
		if (!o.actor)
		{
			return false;
		}
		if (itemRequirements.Count > 0)
		{
			if (!o.actor.inventory)
			{
				return false;
			}
			for (int i = 0; i < itemRequirements.Count; i++)
			{
				int itemTotalQuantity = o.actor.inventory.GetItemTotalQuantity(itemRequirements[i].item);
				switch (itemRequirements[i].requirement)
				{
				case AbilityRequirementType.Present:
					if (itemTotalQuantity < itemRequirements[i].quantity)
					{
						return false;
					}
					break;
				case AbilityRequirementType.Full:
					if (itemTotalQuantity < itemRequirements[i].quantity)
					{
						return false;
					}
					break;
				case AbilityRequirementType.Partial:
					if (itemTotalQuantity == 0)
					{
						return false;
					}
					break;
				default:
					return false;
				}
			}
		}
		return true;
	}

	public bool HasRequiredSpeed(Ability o)
	{
		if (!o.actor)
		{
			return false;
		}
		float num = ((!o.actor.physics) ? 0f : o.actor.physics.velocity.magnitude);
		switch (speedRequirement)
		{
		case SpeedRequirement.None:
			return true;
		case SpeedRequirement.Less:
			return num < speedRequirementValue;
		case SpeedRequirement.Greater:
			return num > speedRequirementValue;
		default:
			return false;
		}
	}

	public bool HasRequiredGround(Ability o)
	{
		if (!o.actor)
		{
			return false;
		}
		if (groundRequirement == GroundRequirement.None)
		{
			return true;
		}
		if (!o.motor)
		{
			return false;
		}
		if (groundRequirement == GroundRequirement.Grounded)
		{
			return o.motor.grounded;
		}
		if (groundRequirement == GroundRequirement.Free)
		{
			return !o.motor.grounded;
		}
		return true;
	}

	public virtual bool HasCustomRequirements(Ability o)
	{
		return true;
	}

	public void Activate(Ability o)
	{
		if ((o.active | !o.actor | !o.actorAbilityComponent | (Time.timeScale == 0f)) || !HasRequiredGround(o) || !HasRequiredSpeed(o) || !HasCustomRequirements(o) || !o.actorAbilityComponent.tags.AllTagsMatch(requiredTags))
		{
			return;
		}
		o.actorAbilityComponent.CancelWithTags(cancelWithTags);
		if (o.actorAbilityComponent.tags.AnyTagsMatch(blockedByTags) || !HasRequiredVitals(o) || !HasRequiredItems(o))
		{
			return;
		}
		if (vitalRequirements.Count > 0)
		{
		}
		if ((bool)o.actor.inventory && itemRequirements.Count > 0)
		{
			for (int i = 0; i < itemRequirements.Count; i++)
			{
				int itemTotalQuantity = o.actor.inventory.GetItemTotalQuantity(itemRequirements[i].item);
				switch (itemRequirements[i].requirement)
				{
				case AbilityRequirementType.Full:
					o.actor.inventory.RemoveItem(itemRequirements[i].item, itemRequirements[i].quantity);
					break;
				case AbilityRequirementType.Partial:
					o.actor.inventory.RemoveItem(itemRequirements[i].item, itemRequirements[i].quantity);
					break;
				}
			}
		}
		o.actorAbilityComponent.tags.AppendTagsFromContainer(tags);
		o.SetActive(true);
	}

	public void Deactivate(Ability o)
	{
		if (!(!o.active | !o.actorAbilityComponent))
		{
			o.actorAbilityComponent.tags.RemoveTagsFromContainer(tags);
			o.SetActive(false);
		}
	}

	public virtual void Engage(Ability o)
	{
	}

	public virtual void Disengage(Ability o)
	{
	}

	public void Interrupt(Ability o)
	{
		if (!(!o.active | !o.actorAbilityComponent) && canInterrupt)
		{
			Deactivate(o);
		}
	}

	public void Cancel(Ability o)
	{
		if (!(!o.active | !o.actorAbilityComponent) && canCancel)
		{
			Deactivate(o);
		}
	}

	public virtual void OnInitialize(Ability o)
	{
	}

	public virtual void OnActivate(Ability o)
	{
	}

	public virtual void OnDeactivate(Ability o)
	{
	}

	public virtual void OnUpdate(Ability o)
	{
	}

	public virtual void OnTrigger(Ability o, ActorEventArgs e)
	{
	}
}
