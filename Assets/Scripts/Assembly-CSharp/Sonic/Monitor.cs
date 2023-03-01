using System;
using ImpactEffects;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class Monitor : Actor
	{
		[SerializeField]
		private SpecialEffectHandler _bustEffect = new SpecialEffectHandler();

		[SerializeField]
		private ImpactEffectSetupData _containedEffect = new ImpactEffectSetupData();

		[SerializeField]
		private InventoryItemEntry _containedItem = new InventoryItemEntry();

		private bool _active = true;

		[SerializeField]
		private GameObject _intactObject;

		[SerializeField]
		private GameObject _destroyedObject;

		public ImpactEffectSetupData containedEffect
		{
			get
			{
				return _containedEffect;
			}
		}

		public InventoryItemEntry containedItem
		{
			get
			{
				return _containedItem;
			}
		}

		public bool active
		{
			get
			{
				return _active;
			}
			protected set
			{
				_active = value;
			}
		}

		public GameObject intactObject
		{
			get
			{
				return _intactObject;
			}
		}

		public GameObject destroyedObject
		{
			get
			{
				return _destroyedObject;
			}
		}

		public SpecialEffectHandler bustEffect
		{
			get
			{
				return _bustEffect;
			}
		}

		public override void OnDeath()
		{
			if ((bool)intactObject)
			{
				intactObject.SetActive(false);
			}
			if ((bool)destroyedObject)
			{
				destroyedObject.SetActive(true);
			}
			bustEffect.Spawn();
			Collider[] components = GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = false;
			}
		}

		public override void OnRespawn()
		{
			if ((bool)intactObject)
			{
				intactObject.SetActive(true);
			}
			if ((bool)destroyedObject)
			{
				destroyedObject.SetActive(false);
			}
			active = true;
			base.alive = true;
			Collider[] components = GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
		}

		public override void OnImpactEffectApply(ImpactEffectApplicationData oData)
		{
			if (!active || oData == null || !(oData.behaviour != null) || !(oData.behaviour is Damage))
			{
				return;
			}
			if ((bool)oData.instigator)
			{
				if (containedEffect.behaviour != null)
				{
					ImpactEffectApplicationData impactEffectApplicationData = new ImpactEffectApplicationData(containedEffect);
					impactEffectApplicationData.target = oData.instigator;
					impactEffectApplicationData.instigator = this;
					impactEffectApplicationData.source = this;
					ImpactEffectApplicationData oData2 = impactEffectApplicationData;
					oData.instigator.ImpactEffect(oData2);
				}
				if ((bool)containedItem.item && (bool)oData.instigator.inventory)
				{
					oData.instigator.inventory.AddItem(containedItem);
				}
			}
			active = false;
		}
	}
}
