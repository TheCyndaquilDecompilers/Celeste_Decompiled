using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D2 RID: 466
	public class RumbleTrigger : Trigger
	{
		// Token: 0x06000FC0 RID: 4032 RVA: 0x00042BDC File Offset: 0x00040DDC
		public RumbleTrigger(EntityData data, Vector2 offset, EntityID id) : base(data, offset)
		{
			this.manualTrigger = data.Bool("manualTrigger", false);
			this.persistent = data.Bool("persistent", false);
			this.id = id;
			Vector2[] array = data.NodesOffset(offset);
			if (array.Length >= 2)
			{
				this.left = Math.Min(array[0].X, array[1].X);
				this.right = Math.Max(array[0].X, array[1].X);
			}
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x00042C88 File Offset: 0x00040E88
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Level level = base.Scene as Level;
			bool flag = false;
			if (this.persistent && level.Session.GetFlag(this.id.ToString()))
			{
				flag = true;
			}
			foreach (Entity entity in scene.Tracker.GetEntities<CrumbleWallOnRumble>())
			{
				CrumbleWallOnRumble crumbleWallOnRumble = (CrumbleWallOnRumble)entity;
				if (crumbleWallOnRumble.X >= this.left && crumbleWallOnRumble.X <= this.right)
				{
					if (flag)
					{
						crumbleWallOnRumble.RemoveSelf();
					}
					else
					{
						this.crumbles.Add(crumbleWallOnRumble);
					}
				}
			}
			if (!flag)
			{
				foreach (Decal decal in scene.Entities.FindAll<Decal>())
				{
					if (decal.IsCrack && decal.X >= this.left && decal.X <= this.right)
					{
						decal.Visible = false;
						this.decals.Add(decal);
					}
				}
				this.crumbles.Sort(delegate(CrumbleWallOnRumble a, CrumbleWallOnRumble b)
				{
					if (!Calc.Random.Chance(0.5f))
					{
						return 1;
					}
					return -1;
				});
			}
			if (flag)
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x00042E08 File Offset: 0x00041008
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			if (!this.manualTrigger)
			{
				this.Invoke(0f);
			}
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x00042E24 File Offset: 0x00041024
		private void Invoke(float delay = 0f)
		{
			if (!this.started)
			{
				this.started = true;
				if (this.persistent)
				{
					(base.Scene as Level).Session.SetFlag(this.id.ToString(), true);
				}
				base.Add(new Coroutine(this.RumbleRoutine(delay), true));
				base.Add(new DisplacementRenderHook(new Action(this.RenderDisplacement)));
			}
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x00042E99 File Offset: 0x00041099
		private IEnumerator RumbleRoutine(float delay)
		{
			yield return delay;
			Scene scene = base.Scene;
			this.rumble = 1f;
			Audio.Play("event:/new_content/game/10_farewell/quake_onset", this.Position);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			foreach (Decal decal in this.decals)
			{
				decal.Visible = true;
			}
			foreach (CrumbleWallOnRumble crumbleWallOnRumble in this.crumbles)
			{
				crumbleWallOnRumble.Break();
				yield return 0.05f;
			}
			List<CrumbleWallOnRumble>.Enumerator enumerator2 = default(List<CrumbleWallOnRumble>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x00042EAF File Offset: 0x000410AF
		public override void Update()
		{
			base.Update();
			this.rumble = Calc.Approach(this.rumble, 0f, Engine.DeltaTime * 0.7f);
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x00042ED8 File Offset: 0x000410D8
		private void RenderDisplacement()
		{
			if (this.rumble <= 0f || Settings.Instance.ScreenShake == ScreenshakeAmount.Off)
			{
				return;
			}
			Camera camera = (base.Scene as Level).Camera;
			int num = (int)(camera.Left / 8f) - 1;
			int num2 = (int)(camera.Right / 8f) + 1;
			for (int i = num; i <= num2; i++)
			{
				float num3 = (float)Math.Sin((double)(base.Scene.TimeActive * 60f + (float)i * 0.4f)) * 0.06f * this.rumble;
				Color color = new Color(0.5f, 0.5f + num3, 0f, 1f);
				Draw.Rect((float)(i * 8), camera.Top - 2f, 8f, 184f, color);
			}
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x00042FA8 File Offset: 0x000411A8
		public static void ManuallyTrigger(float x, float delay)
		{
			foreach (RumbleTrigger rumbleTrigger in Engine.Scene.Entities.FindAll<RumbleTrigger>())
			{
				if (rumbleTrigger.manualTrigger && x >= rumbleTrigger.left && x <= rumbleTrigger.right)
				{
					rumbleTrigger.Invoke(delay);
				}
			}
		}

		// Token: 0x04000B25 RID: 2853
		private bool manualTrigger;

		// Token: 0x04000B26 RID: 2854
		private bool started;

		// Token: 0x04000B27 RID: 2855
		private bool persistent;

		// Token: 0x04000B28 RID: 2856
		private EntityID id;

		// Token: 0x04000B29 RID: 2857
		private float rumble;

		// Token: 0x04000B2A RID: 2858
		private float left;

		// Token: 0x04000B2B RID: 2859
		private float right;

		// Token: 0x04000B2C RID: 2860
		private List<Decal> decals = new List<Decal>();

		// Token: 0x04000B2D RID: 2861
		private List<CrumbleWallOnRumble> crumbles = new List<CrumbleWallOnRumble>();
	}
}
