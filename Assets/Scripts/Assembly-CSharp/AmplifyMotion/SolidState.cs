using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AmplifyMotion
{
	internal class SolidState : MotionState
	{
		public MeshRenderer m_meshRenderer;

		public Matrix4x4 m_prevLocalToWorld;

		public Matrix4x4 m_currLocalToWorld;

		public Vector3 m_lastPosition;

		public Quaternion m_lastRotation;

		public Vector3 m_lastScale;

		private Mesh m_mesh;

		private MaterialDesc[] m_sharedMaterials;

		public bool m_moved;

		private bool m_wasVisible;

		private static HashSet<AmplifyMotionObjectBase> m_uniqueWarnings = new HashSet<AmplifyMotionObjectBase>();

		public SolidState(AmplifyMotionCamera owner, AmplifyMotionObjectBase obj)
			: base(owner, obj)
		{
			m_meshRenderer = m_obj.GetComponent<MeshRenderer>();
		}

		internal override void Initialize()
		{
			MeshFilter component = m_obj.GetComponent<MeshFilter>();
			if (component == null || component.mesh == null)
			{
				if (!m_uniqueWarnings.Contains(m_obj))
				{
					Debug.LogWarning("[AmplifyMotion] Invalid MeshFilter/Mesh in object " + m_obj.name + ". Skipping.");
					m_uniqueWarnings.Add(m_obj);
				}
				m_error = true;
			}
			else
			{
				base.Initialize();
				m_mesh = component.mesh;
				m_sharedMaterials = ProcessSharedMaterials(m_meshRenderer.sharedMaterials);
				m_wasVisible = false;
			}
		}

		internal override void UpdateTransform(CommandBuffer updateCB, bool starting)
		{
			if (!m_initialized)
			{
				Initialize();
				return;
			}
			if (!starting && m_wasVisible)
			{
				m_prevLocalToWorld = m_currLocalToWorld;
			}
			m_moved = true;
			if (!m_owner.Overlay)
			{
				Vector3 position = m_transform.position;
				Quaternion rotation = m_transform.rotation;
				Vector3 lossyScale = m_transform.lossyScale;
				m_moved = starting || MotionState.VectorChanged(position, m_lastPosition) || MotionState.RotationChanged(rotation, m_lastRotation) || MotionState.VectorChanged(lossyScale, m_lastScale);
				if (m_moved)
				{
					m_lastPosition = position;
					m_lastRotation = rotation;
					m_lastScale = lossyScale;
				}
			}
			m_currLocalToWorld = m_transform.localToWorldMatrix;
			if (starting || !m_wasVisible)
			{
				m_prevLocalToWorld = m_currLocalToWorld;
			}
			m_wasVisible = m_meshRenderer.isVisible;
		}

		internal override void RenderVectors(Camera camera, CommandBuffer renderCB, float scale, Quality quality)
		{
			if (!m_initialized || m_error || !m_meshRenderer.isVisible)
			{
				return;
			}
			bool flag = ((int)m_owner.Instance.CullingMask & (1 << m_obj.gameObject.layer)) != 0;
			if (flag && (!flag || !m_moved))
			{
				return;
			}
			int num = ((!flag) ? 255 : m_owner.Instance.GenerateObjectId(m_obj.gameObject));
			Matrix4x4 value = ((!m_obj.FixedStep) ? (m_owner.PrevViewProjMatrixRT * m_prevLocalToWorld) : (m_owner.PrevViewProjMatrixRT * m_currLocalToWorld));
			renderCB.SetGlobalMatrix("_AM_MATRIX_PREV_MVP", value);
			renderCB.SetGlobalFloat("_AM_OBJECT_ID", (float)num * 0.003921569f);
			renderCB.SetGlobalFloat("_AM_MOTION_SCALE", (!flag) ? 0f : scale);
			int num2 = ((quality != 0) ? 2 : 0);
			for (int i = 0; i < m_sharedMaterials.Length; i++)
			{
				MaterialDesc materialDesc = m_sharedMaterials[i];
				int shaderPass = num2 + (materialDesc.coverage ? 1 : 0);
				if (materialDesc.coverage)
				{
					Texture mainTexture = materialDesc.material.mainTexture;
					if (mainTexture != null)
					{
						materialDesc.propertyBlock.SetTexture("_MainTex", mainTexture);
					}
					if (materialDesc.cutoff)
					{
						materialDesc.propertyBlock.SetFloat("_Cutoff", materialDesc.material.GetFloat("_Cutoff"));
					}
				}
				renderCB.DrawMesh(m_mesh, m_transform.localToWorldMatrix, m_owner.Instance.SolidVectorsMaterial, i, shaderPass, materialDesc.propertyBlock);
			}
		}
	}
}
