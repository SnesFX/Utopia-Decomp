using PigeonCoopToolkit.Utillities;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	[AddComponentMenu("Pigeon Coop Toolkit/Effects/Smooth Trail")]
	public class SmoothTrail : TrailRenderer_Base
	{
		private class ControlPoint
		{
			public Vector3 p;

			public Vector3 forward;
		}

		public float MinControlPointDistance = 0.1f;

		public int MaxControlPoints = 15;

		public int PointsBetweenControlPoints = 4;

		private Vector3 _lastPosition;

		private float _distanceMoved;

		private CircularBuffer<ControlPoint> _controlPoints;

		protected override void Start()
		{
			base.Start();
			_lastPosition = _t.position;
		}

		protected override void Update()
		{
			if (_emit)
			{
				_distanceMoved += Vector3.Distance(_t.position, _lastPosition);
				if (!Mathf.Approximately(_distanceMoved, 0f) && _distanceMoved >= MinControlPointDistance)
				{
					AddControlPoint(_t.position);
					_distanceMoved = 0f;
				}
				else
				{
					_controlPoints[_controlPoints.Count - 1].p = _t.position;
					if (TrailData.UseForwardOverride)
					{
						_controlPoints[_controlPoints.Count - 1].forward = ((!TrailData.ForwardOverrideRelative) ? TrailData.ForwardOverride.normalized : _t.TransformDirection(TrailData.ForwardOverride.normalized));
					}
				}
				_lastPosition = _t.position;
			}
			base.Update();
		}

		protected override void OnStartEmit()
		{
			_lastPosition = _t.position;
			_distanceMoved = 0f;
			_controlPoints = new CircularBuffer<ControlPoint>(MaxControlPoints);
			_controlPoints.Add(new ControlPoint
			{
				p = _lastPosition
			});
			if (TrailData.UseForwardOverride)
			{
				_controlPoints[0].forward = ((!TrailData.ForwardOverrideRelative) ? TrailData.ForwardOverride.normalized : _t.TransformDirection(TrailData.ForwardOverride.normalized));
			}
			AddPoint(new PCTrailPoint(), _lastPosition);
			AddControlPoint(_lastPosition);
		}

		protected override void UpdateTrail(PCTrail trail, float deltaTime)
		{
			if (!trail.IsActiveTrail)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < _controlPoints.Count; i++)
			{
				trail.Points[num].Position = _controlPoints[i].p;
				if (TrailData.UseForwardOverride)
				{
					trail.Points[num].Forward = _controlPoints[i].forward;
				}
				num++;
				if (i >= _controlPoints.Count - 1)
				{
					continue;
				}
				float num2 = Vector3.Distance(_controlPoints[i].p, _controlPoints[i + 1].p) / 2f;
				Vector3 curveStartHandle = ((i != 0) ? (_controlPoints[i].p + (_controlPoints[i + 1].p - _controlPoints[i - 1].p).normalized * num2) : (_controlPoints[i].p + (_controlPoints[i + 1].p - _controlPoints[i].p).normalized * num2));
				int num3 = i + 1;
				Vector3 curveEndHandle = ((num3 != _controlPoints.Count - 1) ? (_controlPoints[num3].p + (_controlPoints[num3 - 1].p - _controlPoints[num3 + 1].p).normalized * num2) : (_controlPoints[num3].p + (_controlPoints[num3 - 1].p - _controlPoints[num3].p).normalized * num2));
				PCTrailPoint pCTrailPoint = trail.Points[num - 1];
				PCTrailPoint pCTrailPoint2 = trail.Points[num - 1 + PointsBetweenControlPoints + 1];
				for (int j = 0; j < PointsBetweenControlPoints; j++)
				{
					float t = ((float)j + 1f) / ((float)PointsBetweenControlPoints + 1f);
					trail.Points[num].Position = GetPointAlongCurve(_controlPoints[i].p, curveStartHandle, _controlPoints[i + 1].p, curveEndHandle, t, 0.3f);
					trail.Points[num].SetTimeActive(Mathf.Lerp(pCTrailPoint.TimeActive(), pCTrailPoint2.TimeActive(), t));
					if (TrailData.UseForwardOverride)
					{
						trail.Points[num].Forward = Vector3.Lerp(pCTrailPoint.Forward, pCTrailPoint2.Forward, t);
					}
					num++;
				}
			}
			int num4 = _controlPoints.Count - 1 + (_controlPoints.Count - 1) * PointsBetweenControlPoints;
			int num5 = num4 - PointsBetweenControlPoints - 1;
			int num6 = num4 + 1;
			float num7 = trail.Points[num5].GetDistanceFromStart();
			for (int k = num5 + 1; k < num6; k++)
			{
				num7 += Vector3.Distance(trail.Points[k - 1].Position, trail.Points[k].Position);
				trail.Points[k].SetDistanceFromStart(num7);
			}
		}

		protected override void Reset()
		{
			base.Reset();
			MinControlPointDistance = 0.1f;
			MaxControlPoints = 15;
			PointsBetweenControlPoints = 4;
		}

		protected override void OnTranslate(Vector3 t)
		{
			_lastPosition += t;
			for (int i = 0; i < _controlPoints.Count; i++)
			{
				_controlPoints[i].p += t;
			}
		}

		private void AddControlPoint(Vector3 position)
		{
			for (int i = 0; i < PointsBetweenControlPoints; i++)
			{
				AddPoint(new PCTrailPoint(), position);
			}
			AddPoint(new PCTrailPoint(), position);
			ControlPoint controlPoint = new ControlPoint();
			controlPoint.p = position;
			ControlPoint controlPoint2 = controlPoint;
			if (TrailData.UseForwardOverride)
			{
				controlPoint2.forward = ((!TrailData.ForwardOverrideRelative) ? TrailData.ForwardOverride.normalized : _t.TransformDirection(TrailData.ForwardOverride.normalized));
			}
			_controlPoints.Add(controlPoint2);
		}

		protected override int GetMaxNumberOfPoints()
		{
			return MaxControlPoints + MaxControlPoints * PointsBetweenControlPoints;
		}

		public Vector3 GetPointAlongCurve(Vector3 curveStart, Vector3 curveStartHandle, Vector3 curveEnd, Vector3 curveEndHandle, float t, float crease)
		{
			float num = 1f - t;
			float num2 = Mathf.Pow(num, 3f);
			float num3 = Mathf.Pow(num, 2f);
			float num4 = 1f - crease;
			return (num2 * curveStart * num4 + 3f * num3 * t * curveStartHandle * crease + 3f * num * Mathf.Pow(t, 2f) * curveEndHandle * crease + Mathf.Pow(t, 3f) * curveEnd * num4) / (num2 * num4 + 3f * num3 * t * crease + 3f * num * Mathf.Pow(t, 2f) * crease + Mathf.Pow(t, 3f) * num4);
		}
	}
}
