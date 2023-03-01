using System;

public class CharacterMotorEventArgs : EventArgs
{
	protected object _data;

	public object data
	{
		get
		{
			return _data;
		}
	}

	public CharacterMotorEventArgs(object oData)
	{
		_data = oData;
	}
}
