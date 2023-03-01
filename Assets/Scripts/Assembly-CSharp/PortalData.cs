using System.Collections.Generic;

public sealed class PortalData
{
	private List<PortalDataEntry> _portals = new List<PortalDataEntry>();

	public List<PortalDataEntry> portals
	{
		get
		{
			return _portals;
		}
		set
		{
			_portals = value;
		}
	}
}
