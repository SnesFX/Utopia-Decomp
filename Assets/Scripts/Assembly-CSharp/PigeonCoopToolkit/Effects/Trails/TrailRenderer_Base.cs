using System;
using System.Collections.Generic;
using System.Linq;
using PigeonCoopToolkit.Utillities;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	public abstract class TrailRenderer_Base : MonoBehaviour
	{
		public PCTrailRendererData TrailData;

		public bool Emit;

		protected bool _emit;

		protected bool _noDecay;

		private PCTrail _activeTrail;

		private List<PCTrail> _fadingTrails;

		protected Transform _t;

		private static Dictionary<Material, List<PCTrail>> _matToTrailList;

		private static List<Mesh> _toClean;

		private static bool _hasRenderer;

		private static int GlobalTrailRendererCount;

		protected virtual void Awake()
		{
			GlobalTrailRendererCount++;
			if (GlobalTrailRendererCount == 1)
			{
				_matToTrailList = new Dictionary<Material, List<PCTrail>>();
				_toClean = new List<Mesh>();
			}
			_fadingTrails = new List<PCTrail>();
			_t = base.transform;
			_emit = Emit;
			if (_emit)
			{
				_activeTrail = new PCTrail(GetMaxNumberOfPoints());
				_activeTrail.IsActiveTrail = true;
				OnStartEmit();
			}
		}

		protected virtual void Start()
		{
		}

		protected virtual void LateUpdate()
		{
			if (_hasRenderer)
			{
				return;
			}
			_hasRenderer = true;
			foreach (KeyValuePair<Material, List<PCTrail>> matToTrail in _matToTrailList)
			{
				CombineInstance[] array = new CombineInstance[matToTrail.Value.Count];
				for (int i = 0; i < matToTrail.Value.Count; i++)
				{
					array[i] = new CombineInstance
					{
						mesh = matToTrail.Value[i].Mesh,
						subMeshIndex = 0,
						transform = Matrix4x4.identity
					};
				}
				Mesh mesh = new Mesh();
				mesh.CombineMeshes(array, true, false);
				_toClean.Add(mesh);
				DrawMesh(mesh, matToTrail.Key);
				matToTrail.Value.Clear();
			}
		}

		protected virtual void Update()
		{
			if (_hasRenderer)
			{
				_hasRenderer = false;
				if (_toClean.Count > 0)
				{
					foreach (Mesh item in _toClean)
					{
						if (Application.isEditor)
						{
							UnityEngine.Object.DestroyImmediate(item, true);
						}
						else
						{
							UnityEngine.Object.Destroy(item);
						}
					}
				}
				_toClean.Clear();
			}
			if (!_matToTrailList.ContainsKey(TrailData.TrailMaterial))
			{
				_matToTrailList.Add(TrailData.TrailMaterial, new List<PCTrail>());
			}
			if (_activeTrail != null)
			{
				UpdatePoints(_activeTrail, Time.deltaTime);
				UpdateTrail(_activeTrail, Time.deltaTime);
				GenerateMesh(_activeTrail);
				_matToTrailList[TrailData.TrailMaterial].Add(_activeTrail);
			}
			for (int num = _fadingTrails.Count - 1; num >= 0; num--)
			{
				if (_fadingTrails[num] == null || !_fadingTrails[num].Points.Any((PCTrailPoint a) => a.TimeActive() < TrailData.Lifetime))
				{
					if (_fadingTrails[num] != null)
					{
						_fadingTrails[num].Dispose();
					}
					_fadingTrails.RemoveAt(num);
				}
				else
				{
					UpdatePoints(_fadingTrails[num], Time.deltaTime);
					UpdateTrail(_fadingTrails[num], Time.deltaTime);
					GenerateMesh(_fadingTrails[num]);
					_matToTrailList[TrailData.TrailMaterial].Add(_fadingTrails[num]);
				}
			}
			CheckEmitChange();
		}

		protected virtual void OnDestroy()
		{
			GlobalTrailRendererCount--;
			if (GlobalTrailRendererCount == 0)
			{
				if (_toClean != null && _toClean.Count > 0)
				{
					foreach (Mesh item in _toClean)
					{
						if (Application.isEditor)
						{
							UnityEngine.Object.DestroyImmediate(item, true);
						}
						else
						{
							UnityEngine.Object.Destroy(item);
						}
					}
				}
				_toClean = null;
				_matToTrailList.Clear();
				_matToTrailList = null;
			}
			if (_activeTrail != null)
			{
				_activeTrail.Dispose();
				_activeTrail = null;
			}
			if (_fadingTrails == null)
			{
				return;
			}
			foreach (PCTrail fadingTrail in _fadingTrails)
			{
				if (fadingTrail != null)
				{
					fadingTrail.Dispose();
				}
			}
			_fadingTrails.Clear();
		}

		protected virtual void OnStopEmit()
		{
		}

		protected virtual void OnStartEmit()
		{
		}

		protected virtual void OnTranslate(Vector3 t)
		{
		}

		protected abstract int GetMaxNumberOfPoints();

		protected virtual void Reset()
		{
			if (TrailData == null)
			{
				TrailData = new PCTrailRendererData();
			}
			TrailData.Lifetime = 1f;
			TrailData.UsingSimpleColor = false;
			TrailData.UsingSimpleSize = false;
			TrailData.ColorOverLife = new Gradient();
			TrailData.SimpleColorOverLifeStart = Color.white;
			TrailData.SimpleColorOverLifeEnd = new Color(1f, 1f, 1f, 0f);
			TrailData.SizeOverLife = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
			TrailData.SimpleSizeOverLifeStart = 1f;
			TrailData.SimpleSizeOverLifeEnd = 0f;
		}

		protected virtual void InitialiseNewPoint(PCTrailPoint newPoint)
		{
		}

		protected virtual void UpdateTrail(PCTrail trail, float deltaTime)
		{
		}

		protected void AddPoint(PCTrailPoint newPoint, Vector3 pos)
		{
			if (_activeTrail != null)
			{
				newPoint.Position = pos;
				newPoint.PointNumber = ((_activeTrail.Points.Count != 0) ? (_activeTrail.Points[_activeTrail.Points.Count - 1].PointNumber + 1) : 0);
				InitialiseNewPoint(newPoint);
				newPoint.SetDistanceFromStart((_activeTrail.Points.Count != 0) ? (_activeTrail.Points[_activeTrail.Points.Count - 1].GetDistanceFromStart() + Vector3.Distance(_activeTrail.Points[_activeTrail.Points.Count - 1].Position, pos)) : 0f);
				if (TrailData.UseForwardOverride)
				{
					newPoint.Forward = ((!TrailData.ForwardOverrideRelative) ? TrailData.ForwardOverride.normalized : _t.TransformDirection(TrailData.ForwardOverride.normalized));
				}
				_activeTrail.Points.Add(newPoint);
			}
		}

		private void GenerateMesh(PCTrail trail)
		{
			trail.Mesh.Clear(false);
			Vector3 vector = ((!(Camera.main != null)) ? Vector3.forward : Camera.main.transform.forward);
			if (TrailData.UseForwardOverride)
			{
				vector = TrailData.ForwardOverride.normalized;
			}
			trail.activePointCount = NumberOfActivePoints(trail);
			if (trail.activePointCount < 2)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < trail.Points.Count; i++)
			{
				PCTrailPoint pCTrailPoint = trail.Points[i];
				float num2 = pCTrailPoint.TimeActive() / TrailData.Lifetime;
				if (!(pCTrailPoint.TimeActive() > TrailData.Lifetime))
				{
					if (TrailData.UseForwardOverride && TrailData.ForwardOverrideRelative)
					{
						vector = pCTrailPoint.Forward;
					}
					Vector3 zero = Vector3.zero;
					zero = ((i >= trail.Points.Count - 1) ? Vector3.Cross((pCTrailPoint.Position - trail.Points[i - 1].Position).normalized, vector).normalized : Vector3.Cross((trail.Points[i + 1].Position - pCTrailPoint.Position).normalized, vector).normalized);
					Color color = (TrailData.StretchColorToFit ? ((!TrailData.UsingSimpleColor) ? TrailData.ColorOverLife.Evaluate(1f - (float)num / (float)trail.activePointCount / 2f) : Color.Lerp(TrailData.SimpleColorOverLifeStart, TrailData.SimpleColorOverLifeEnd, 1f - (float)num / (float)trail.activePointCount / 2f)) : ((!TrailData.UsingSimpleColor) ? TrailData.ColorOverLife.Evaluate(num2) : Color.Lerp(TrailData.SimpleColorOverLifeStart, TrailData.SimpleColorOverLifeEnd, num2)));
					float num3 = (TrailData.StretchSizeToFit ? ((!TrailData.UsingSimpleSize) ? TrailData.SizeOverLife.Evaluate(1f - (float)num / (float)trail.activePointCount / 2f) : Mathf.Lerp(TrailData.SimpleSizeOverLifeStart, TrailData.SimpleSizeOverLifeEnd, 1f - (float)num / (float)trail.activePointCount / 2f)) : ((!TrailData.UsingSimpleSize) ? TrailData.SizeOverLife.Evaluate(num2) : Mathf.Lerp(TrailData.SimpleSizeOverLifeStart, TrailData.SimpleSizeOverLifeEnd, num2)));
					trail.verticies[num] = pCTrailPoint.Position + zero * num3;
					if (TrailData.MaterialTileLength <= 0f)
					{
						trail.uvs[num] = new Vector2((float)num / (float)trail.activePointCount / 2f, 0f);
					}
					else
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / TrailData.MaterialTileLength, 0f);
					}
					trail.normals[num] = vector;
					trail.colors[num] = color;
					num++;
					trail.verticies[num] = pCTrailPoint.Position - zero * num3;
					if (TrailData.MaterialTileLength <= 0f)
					{
						trail.uvs[num] = new Vector2((float)num / (float)trail.activePointCount / 2f, 1f);
					}
					else
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / TrailData.MaterialTileLength, 1f);
					}
					trail.normals[num] = vector;
					trail.colors[num] = color;
					num++;
				}
			}
			Vector2 vector2 = trail.verticies[num - 1];
			for (int j = num; j < trail.verticies.Length; j++)
			{
				trail.verticies[j] = vector2;
			}
			int num4 = 0;
			for (int k = 0; k < 2 * (trail.activePointCount - 1); k++)
			{
				if (k % 2 == 0)
				{
					trail.indicies[num4] = k;
					num4++;
					trail.indicies[num4] = k + 1;
					num4++;
					trail.indicies[num4] = k + 2;
				}
				else
				{
					trail.indicies[num4] = k + 2;
					num4++;
					trail.indicies[num4] = k + 1;
					num4++;
					trail.indicies[num4] = k;
				}
				num4++;
			}
			int num5 = trail.indicies[num4 - 1];
			for (int l = num4; l < trail.indicies.Length; l++)
			{
				trail.indicies[l] = num5;
			}
			trail.Mesh.vertices = trail.verticies;
			trail.Mesh.SetIndices(trail.indicies, MeshTopology.Triangles, 0);
			trail.Mesh.uv = trail.uvs;
			trail.Mesh.normals = trail.normals;
			trail.Mesh.colors = trail.colors;
		}

		private void DrawMesh(Mesh trailMesh, Material trailMaterial)
		{
			Graphics.DrawMesh(trailMesh, Matrix4x4.identity, trailMaterial, base.gameObject.layer);
		}

		private void UpdatePoints(PCTrail line, float deltaTime)
		{
			for (int i = 0; i < line.Points.Count; i++)
			{
				line.Points[i].Update((!_noDecay) ? deltaTime : 0f);
			}
		}

		[Obsolete("UpdatePoint is deprecated, you should instead override UpdateTrail and loop through the individual points yourself (See Smoke or Smoke Plume scripts for how to do this).", true)]
		protected virtual void UpdatePoint(PCTrailPoint pCTrailPoint, float deltaTime)
		{
		}

		private void CheckEmitChange()
		{
			if (_emit != Emit)
			{
				_emit = Emit;
				if (_emit)
				{
					_activeTrail = new PCTrail(GetMaxNumberOfPoints());
					_activeTrail.IsActiveTrail = true;
					OnStartEmit();
				}
				else
				{
					OnStopEmit();
					_activeTrail.IsActiveTrail = false;
					_fadingTrails.Add(_activeTrail);
					_activeTrail = null;
				}
			}
		}

		private int NumberOfActivePoints(PCTrail line)
		{
			int num = 0;
			for (int i = 0; i < line.Points.Count; i++)
			{
				if (line.Points[i].TimeActive() < TrailData.Lifetime)
				{
					num++;
				}
			}
			return num;
		}

		[ContextMenu("Toggle inspector size input method")]
		protected void ToggleSizeInputStyle()
		{
			TrailData.UsingSimpleSize = !TrailData.UsingSimpleSize;
		}

		[ContextMenu("Toggle inspector color input method")]
		protected void ToggleColorInputStyle()
		{
			TrailData.UsingSimpleColor = !TrailData.UsingSimpleColor;
		}

		public void LifeDecayEnabled(bool enabled)
		{
			_noDecay = !enabled;
		}

		public void Translate(Vector3 t)
		{
			if (_activeTrail != null)
			{
				for (int i = 0; i < _activeTrail.Points.Count; i++)
				{
					_activeTrail.Points[i].Position += t;
				}
			}
			if (_fadingTrails != null)
			{
				foreach (PCTrail fadingTrail in _fadingTrails)
				{
					for (int j = 0; j < fadingTrail.Points.Count; j++)
					{
						fadingTrail.Points[j].Position += t;
					}
				}
			}
			OnTranslate(t);
		}

		public void CreateTrail(Vector3 from, Vector3 to, float distanceBetweenPoints)
		{
			float num = Vector3.Distance(from, to);
			Vector3 normalized = (to - from).normalized;
			float num2 = 0f;
			CircularBuffer<PCTrailPoint> circularBuffer = new CircularBuffer<PCTrailPoint>(GetMaxNumberOfPoints());
			int num3 = 0;
			for (; num2 < num; num2 += distanceBetweenPoints)
			{
				PCTrailPoint pCTrailPoint = new PCTrailPoint();
				pCTrailPoint.PointNumber = num3;
				pCTrailPoint.Position = from + normalized * num2;
				circularBuffer.Add(pCTrailPoint);
				InitialiseNewPoint(pCTrailPoint);
				num3++;
				if (distanceBetweenPoints <= 0f)
				{
					break;
				}
			}
			PCTrailPoint pCTrailPoint2 = new PCTrailPoint();
			pCTrailPoint2.PointNumber = num3;
			pCTrailPoint2.Position = to;
			circularBuffer.Add(pCTrailPoint2);
			InitialiseNewPoint(pCTrailPoint2);
			PCTrail pCTrail = new PCTrail(GetMaxNumberOfPoints());
			pCTrail.Points = circularBuffer;
			_fadingTrails.Add(pCTrail);
		}

		public void ClearSystem(bool emitState)
		{
			if (_activeTrail != null)
			{
				_activeTrail.Dispose();
				_activeTrail = null;
			}
			if (_fadingTrails != null)
			{
				foreach (PCTrail fadingTrail in _fadingTrails)
				{
					if (fadingTrail != null)
					{
						fadingTrail.Dispose();
					}
				}
				_fadingTrails.Clear();
			}
			Emit = emitState;
			_emit = !emitState;
			CheckEmitChange();
		}

		public int NumSegments()
		{
			int num = 0;
			if (_activeTrail != null && NumberOfActivePoints(_activeTrail) != 0)
			{
				num++;
			}
			return num + _fadingTrails.Count;
		}
	}
}
