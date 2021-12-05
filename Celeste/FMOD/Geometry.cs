using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x02000049 RID: 73
	public class Geometry : HandleBase
	{
		// Token: 0x060002CA RID: 714 RVA: 0x0000492B File Offset: 0x00002B2B
		public RESULT release()
		{
			RESULT result = Geometry.FMOD_Geometry_Release(base.getRaw());
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00004946 File Offset: 0x00002B46
		public RESULT addPolygon(float directocclusion, float reverbocclusion, bool doublesided, int numvertices, VECTOR[] vertices, out int polygonindex)
		{
			return Geometry.FMOD_Geometry_AddPolygon(this.rawPtr, directocclusion, reverbocclusion, doublesided, numvertices, vertices, out polygonindex);
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000495C File Offset: 0x00002B5C
		public RESULT getNumPolygons(out int numpolygons)
		{
			return Geometry.FMOD_Geometry_GetNumPolygons(this.rawPtr, out numpolygons);
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000496A File Offset: 0x00002B6A
		public RESULT getMaxPolygons(out int maxpolygons, out int maxvertices)
		{
			return Geometry.FMOD_Geometry_GetMaxPolygons(this.rawPtr, out maxpolygons, out maxvertices);
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00004979 File Offset: 0x00002B79
		public RESULT getPolygonNumVertices(int index, out int numvertices)
		{
			return Geometry.FMOD_Geometry_GetPolygonNumVertices(this.rawPtr, index, out numvertices);
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00004988 File Offset: 0x00002B88
		public RESULT setPolygonVertex(int index, int vertexindex, ref VECTOR vertex)
		{
			return Geometry.FMOD_Geometry_SetPolygonVertex(this.rawPtr, index, vertexindex, ref vertex);
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00004998 File Offset: 0x00002B98
		public RESULT getPolygonVertex(int index, int vertexindex, out VECTOR vertex)
		{
			return Geometry.FMOD_Geometry_GetPolygonVertex(this.rawPtr, index, vertexindex, out vertex);
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x000049A8 File Offset: 0x00002BA8
		public RESULT setPolygonAttributes(int index, float directocclusion, float reverbocclusion, bool doublesided)
		{
			return Geometry.FMOD_Geometry_SetPolygonAttributes(this.rawPtr, index, directocclusion, reverbocclusion, doublesided);
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x000049BA File Offset: 0x00002BBA
		public RESULT getPolygonAttributes(int index, out float directocclusion, out float reverbocclusion, out bool doublesided)
		{
			return Geometry.FMOD_Geometry_GetPolygonAttributes(this.rawPtr, index, out directocclusion, out reverbocclusion, out doublesided);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x000049CC File Offset: 0x00002BCC
		public RESULT setActive(bool active)
		{
			return Geometry.FMOD_Geometry_SetActive(this.rawPtr, active);
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000049DA File Offset: 0x00002BDA
		public RESULT getActive(out bool active)
		{
			return Geometry.FMOD_Geometry_GetActive(this.rawPtr, out active);
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000049E8 File Offset: 0x00002BE8
		public RESULT setRotation(ref VECTOR forward, ref VECTOR up)
		{
			return Geometry.FMOD_Geometry_SetRotation(this.rawPtr, ref forward, ref up);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x000049F7 File Offset: 0x00002BF7
		public RESULT getRotation(out VECTOR forward, out VECTOR up)
		{
			return Geometry.FMOD_Geometry_GetRotation(this.rawPtr, out forward, out up);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00004A06 File Offset: 0x00002C06
		public RESULT setPosition(ref VECTOR position)
		{
			return Geometry.FMOD_Geometry_SetPosition(this.rawPtr, ref position);
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00004A14 File Offset: 0x00002C14
		public RESULT getPosition(out VECTOR position)
		{
			return Geometry.FMOD_Geometry_GetPosition(this.rawPtr, out position);
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00004A22 File Offset: 0x00002C22
		public RESULT setScale(ref VECTOR scale)
		{
			return Geometry.FMOD_Geometry_SetScale(this.rawPtr, ref scale);
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00004A30 File Offset: 0x00002C30
		public RESULT getScale(out VECTOR scale)
		{
			return Geometry.FMOD_Geometry_GetScale(this.rawPtr, out scale);
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00004A3E File Offset: 0x00002C3E
		public RESULT save(IntPtr data, out int datasize)
		{
			return Geometry.FMOD_Geometry_Save(this.rawPtr, data, out datasize);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00004A4D File Offset: 0x00002C4D
		public RESULT setUserData(IntPtr userdata)
		{
			return Geometry.FMOD_Geometry_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00004A5B File Offset: 0x00002C5B
		public RESULT getUserData(out IntPtr userdata)
		{
			return Geometry.FMOD_Geometry_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x060002DE RID: 734
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_Release(IntPtr geometry);

		// Token: 0x060002DF RID: 735
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, VECTOR[] vertices, out int polygonindex);

		// Token: 0x060002E0 RID: 736
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetNumPolygons(IntPtr geometry, out int numpolygons);

		// Token: 0x060002E1 RID: 737
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetMaxPolygons(IntPtr geometry, out int maxpolygons, out int maxvertices);

		// Token: 0x060002E2 RID: 738
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int numvertices);

		// Token: 0x060002E3 RID: 739
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, ref VECTOR vertex);

		// Token: 0x060002E4 RID: 740
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, out VECTOR vertex);

		// Token: 0x060002E5 RID: 741
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);

		// Token: 0x060002E6 RID: 742
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetPolygonAttributes(IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided);

		// Token: 0x060002E7 RID: 743
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_SetActive(IntPtr geometry, bool active);

		// Token: 0x060002E8 RID: 744
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetActive(IntPtr geometry, out bool active);

		// Token: 0x060002E9 RID: 745
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_SetRotation(IntPtr geometry, ref VECTOR forward, ref VECTOR up);

		// Token: 0x060002EA RID: 746
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetRotation(IntPtr geometry, out VECTOR forward, out VECTOR up);

		// Token: 0x060002EB RID: 747
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_SetPosition(IntPtr geometry, ref VECTOR position);

		// Token: 0x060002EC RID: 748
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetPosition(IntPtr geometry, out VECTOR position);

		// Token: 0x060002ED RID: 749
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_SetScale(IntPtr geometry, ref VECTOR scale);

		// Token: 0x060002EE RID: 750
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetScale(IntPtr geometry, out VECTOR scale);

		// Token: 0x060002EF RID: 751
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_Save(IntPtr geometry, IntPtr data, out int datasize);

		// Token: 0x060002F0 RID: 752
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_SetUserData(IntPtr geometry, IntPtr userdata);

		// Token: 0x060002F1 RID: 753
		[DllImport("fmod")]
		private static extern RESULT FMOD_Geometry_GetUserData(IntPtr geometry, out IntPtr userdata);

		// Token: 0x060002F2 RID: 754 RVA: 0x00003A27 File Offset: 0x00001C27
		public Geometry(IntPtr raw) : base(raw)
		{
		}
	}
}
