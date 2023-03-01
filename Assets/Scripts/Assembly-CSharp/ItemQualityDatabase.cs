using System;
using System.Collections.Generic;

[Serializable]
public class ItemQualityDatabase : ScriptableObjectDatabase<ItemQuality>
{
	public List<ItemQuality> derp = new List<ItemQuality>();
}
