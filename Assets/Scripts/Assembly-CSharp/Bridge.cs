using System.Linq;
using UnityEngine;

public class Bridge : MonoBehaviour
{
	private Transform start;

	private Transform end;

	private Transform[] logs;

	private Vector3[] originalPos;

	private Vector3[] targetPos;

	private Vector3 startPos;

	private Vector3 endPos;

	private int logAmount;

	private float length;

	private float dist;

	private float lerp = 1f;

	private float lerp2;

	private Character player;

	private Transform playerTransform;

	private void Start()
	{
		start = base.transform.GetChild(0);
		end = base.transform.GetChild(1);
		length = (end.position - start.position).magnitude;
		logAmount = Mathf.FloorToInt(length / 2f);
		logs = new Transform[logAmount + 1];
		originalPos = new Vector3[logAmount + 1];
		targetPos = new Vector3[logAmount + 1];
		startPos = start.position;
		endPos = end.position;
		dist = (startPos - endPos).magnitude;
		logs[0] = base.transform;
		playerTransform = GameObject.Find("Saturn Sonic").transform;
		for (int i = 1; i < logAmount; i++)
		{
			Vector3 vector = Vector3.Lerp(startPos, endPos, (float)i / (float)logAmount);
			originalPos[i] = vector;
			targetPos[i] = vector;
			logs[i] = (Object.Instantiate(Resources.Load("Bridge Log", typeof(GameObject)), vector, start.rotation, base.transform) as GameObject).transform;
			logs[i].rotation = Quaternion.Euler(0f, logs[i].rotation.eulerAngles.y, 0f);
			logs[i].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		}
		logs[logAmount] = end;
	}

	private void FixedUpdate()
	{
		if ((playerTransform.position - base.transform.position).magnitude > dist * 2f)
		{
			return;
		}
		Character character = (from p in Physics.OverlapBox(Vector3.Lerp(startPos, endPos, 0.5f), new Vector3(8f, 2f, logAmount), Quaternion.Euler(0f, start.rotation.eulerAngles.y, 0f))
			select p.gameObject.GetComponent<Character>()).FirstOrDefault((Character p) => p != null);
		if (character != null)
		{
			lerp = 0f;
			Vector3 position = character.transform.position;
			float num = float.MaxValue;
			int num2 = 0;
			float num3 = Mathf.PingPong((startPos - position).magnitude, dist / 2f) / (dist / 2f);
			for (int i = 1; i < logAmount; i++)
			{
				Transform transform = logs[i];
				Vector3 vector = position - transform.position;
				vector.Scale(new Vector3(1f, 0f, 1f));
				float magnitude = vector.magnitude;
				if (magnitude < num)
				{
					num = magnitude;
					num2 = i;
				}
			}
			targetPos[num2] = originalPos[num2] - new Vector3(0f, 1.5f * num3, 0f);
			logs[num2].position = new Vector3(originalPos[num2].x, Mathf.Lerp(originalPos[num2].y, targetPos[num2].y, lerp2), originalPos[num2].z);
			lerp2 = Mathf.Min(lerp2 + Time.deltaTime * 10f, 1f);
			for (int j = 1; j < logAmount; j++)
			{
				if (j != num2)
				{
					Transform transform2 = logs[j];
					if (j < num2)
					{
						float num4 = (float)j / (float)num2;
						num4 = Mathf.Sin(num4 * 1.57f);
						transform2.position = new Vector3(originalPos[j].x, Mathf.Lerp(startPos.y, logs[num2].position.y, num4), originalPos[j].z);
					}
					else
					{
						float num4 = (float)(j - num2) / (float)(logAmount - num2);
						num4 = Mathf.Cos(num4 * 1.57f);
						transform2.position = new Vector3(originalPos[j].x, Mathf.Lerp(endPos.y, logs[num2].position.y, num4), originalPos[j].z);
					}
					targetPos[j] = transform2.position;
				}
			}
			return;
		}
		if (lerp < 1f)
		{
			lerp = Mathf.Min(lerp + Time.deltaTime * 10f, 1f);
			for (int k = 1; k < logAmount; k++)
			{
				Transform transform3 = logs[k];
				transform3.position = new Vector3(originalPos[k].x, Mathf.Lerp(targetPos[k].y, originalPos[k].y, lerp), originalPos[k].z);
			}
		}
		lerp2 = 0f;
	}
}
