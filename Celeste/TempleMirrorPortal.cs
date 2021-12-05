using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D8 RID: 728
	public class TempleMirrorPortal : Entity
	{
		// Token: 0x06001679 RID: 5753 RVA: 0x00084AFC File Offset: 0x00082CFC
		public TempleMirrorPortal(Vector2 position) : base(position)
		{
			base.Depth = 2000;
			base.Collider = new Hitbox(120f, 64f, -60f, -32f);
			base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x00084BA0 File Offset: 0x00082DA0
		public TempleMirrorPortal(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x00084BB4 File Offset: 0x00082DB4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.curtain = new TempleMirrorPortal.Curtain(this.Position));
			scene.Add(new TempleMirrorPortal.Bg(this.Position));
			scene.Add(this.leftTorch = new TemplePortalTorch(this.Position + new Vector2(-90f, 0f)));
			scene.Add(this.rightTorch = new TemplePortalTorch(this.Position + new Vector2(90f, 0f)));
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x00084C4F File Offset: 0x00082E4F
		public void OnSwitchHit(int side)
		{
			base.Add(new Coroutine(this.OnSwitchRoutine(side), true));
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x00084C64 File Offset: 0x00082E64
		private IEnumerator OnSwitchRoutine(int side)
		{
			yield return 0.4f;
			if (side < 0)
			{
				this.leftTorch.Light(this.switchCounter);
			}
			else
			{
				this.rightTorch.Light(this.switchCounter);
			}
			this.switchCounter++;
			if ((base.Scene as Level).Session.Area.Mode == AreaMode.Normal)
			{
				LightingRenderer lighting = (base.Scene as Level).Lighting;
				float lightTarget = Math.Max(0f, lighting.Alpha - 0.2f);
				while ((lighting.Alpha -= Engine.DeltaTime) > lightTarget)
				{
					yield return null;
				}
				lighting = null;
			}
			yield return 0.15f;
			if (this.switchCounter >= 2)
			{
				yield return 0.1f;
				Audio.Play("event:/game/05_mirror_temple/mainmirror_reveal", this.Position);
				this.curtain.Drop();
				this.canTrigger = true;
				yield return 0.1f;
				Level level = base.SceneAs<Level>();
				for (int i = 0; i < 120; i += 12)
				{
					for (int j = 0; j < 60; j += 6)
					{
						level.Particles.Emit(TempleMirrorPortal.P_CurtainDrop, 1, this.curtain.Position + new Vector2((float)(-57 + i), (float)(-27 + j)), new Vector2(6f, 3f));
					}
				}
			}
			yield break;
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x00084C7A File Offset: 0x00082E7A
		public void Activate()
		{
			base.Add(new Coroutine(this.ActivateRoutine(), true));
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x00084C8E File Offset: 0x00082E8E
		private IEnumerator ActivateRoutine()
		{
			Level level = base.Scene as Level;
			LightingRenderer light = level.Lighting;
			float debrisStart = 0f;
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			base.Add(new DisplacementRenderHook(new Action(this.RenderDisplacement)));
			for (;;)
			{
				this.bufferAlpha = Calc.Approach(this.bufferAlpha, 1f, Engine.DeltaTime);
				this.bufferTimer += 4f * Engine.DeltaTime;
				light.Alpha = Calc.Approach(light.Alpha, 0.2f, Engine.DeltaTime * 0.25f);
				if (debrisStart < (float)this.debris.Length)
				{
					int num = (int)debrisStart;
					this.debris[num].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
					this.debris[num].Enabled = true;
					this.debris[num].Duration = 0.5f + Calc.Random.NextFloat(0.7f);
				}
				debrisStart += Engine.DeltaTime * 10f;
				for (int i = 0; i < this.debris.Length; i++)
				{
					if (this.debris[i].Enabled)
					{
						TempleMirrorPortal.Debris[] array = this.debris;
						int num2 = i;
						array[num2].Percent = array[num2].Percent % 1f;
						TempleMirrorPortal.Debris[] array2 = this.debris;
						int num3 = i;
						array2[num3].Percent = array2[num3].Percent + Engine.DeltaTime / this.debris[i].Duration;
					}
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x00084CA0 File Offset: 0x00082EA0
		private void BeforeRender()
		{
			if (this.buffer == null)
			{
				this.buffer = VirtualContent.CreateRenderTarget("temple-portal", 120, 64, false, true, 0);
			}
			Vector2 position = new Vector2((float)this.buffer.Width, (float)this.buffer.Height) / 2f;
			MTexture mtexture = GFX.Game["objects/temple/portal/portal"];
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.buffer);
			Engine.Graphics.GraphicsDevice.Clear(Color.Black);
			Draw.SpriteBatch.Begin();
			int num = 0;
			while ((float)num < 10f)
			{
				float num2 = this.bufferTimer % 1f * 0.1f + (float)num / 10f;
				Color color = Color.Lerp(Color.Black, Color.Purple, num2);
				float scale = num2;
				float rotation = 6.2831855f * num2;
				mtexture.DrawCentered(position, color, scale, rotation);
				num++;
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00084DA0 File Offset: 0x00082FA0
		private void RenderDisplacement()
		{
			Draw.Rect(base.X - 60f, base.Y - 32f, 120f, 64f, new Color(0.5f, 0.5f, 0.25f * this.DistortionFade * this.bufferAlpha, 1f));
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x00084DFC File Offset: 0x00082FFC
		public override void Render()
		{
			base.Render();
			if (this.buffer != null)
			{
				Draw.SpriteBatch.Draw(this.buffer, this.Position + new Vector2(-base.Collider.Width / 2f, -base.Collider.Height / 2f), Color.White * this.bufferAlpha);
			}
			GFX.Game["objects/temple/portal/portalframe"].DrawCentered(this.Position);
			Level level = base.Scene as Level;
			for (int i = 0; i < this.debris.Length; i++)
			{
				TempleMirrorPortal.Debris debris = this.debris[i];
				if (debris.Enabled)
				{
					float num = Ease.SineOut(debris.Percent);
					Vector2 position = this.Position + debris.Direction * (1f - num) * (190f - level.Zoom * 30f);
					Color color = Color.Lerp(this.debrisColorFrom, this.debrisColorTo, num);
					float scale = Calc.LerpClamp(1f, 0.2f, num);
					this.debrisTexture.DrawCentered(position, color, scale, (float)i * 0.05f);
				}
			}
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x00084F4D File Offset: 0x0008314D
		private void OnPlayer(Player player)
		{
			if (this.canTrigger)
			{
				this.canTrigger = false;
				base.Scene.Add(new CS04_MirrorPortal(player, this));
			}
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x00084F70 File Offset: 0x00083170
		public override void Removed(Scene scene)
		{
			this.Dispose();
			base.Removed(scene);
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x00084F7F File Offset: 0x0008317F
		public override void SceneEnd(Scene scene)
		{
			this.Dispose();
			base.SceneEnd(scene);
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x00084F8E File Offset: 0x0008318E
		private void Dispose()
		{
			if (this.buffer != null)
			{
				this.buffer.Dispose();
			}
			this.buffer = null;
		}

		// Token: 0x040012E8 RID: 4840
		public static ParticleType P_CurtainDrop;

		// Token: 0x040012E9 RID: 4841
		public float DistortionFade = 1f;

		// Token: 0x040012EA RID: 4842
		private bool canTrigger;

		// Token: 0x040012EB RID: 4843
		private int switchCounter;

		// Token: 0x040012EC RID: 4844
		private VirtualRenderTarget buffer;

		// Token: 0x040012ED RID: 4845
		private float bufferAlpha;

		// Token: 0x040012EE RID: 4846
		private float bufferTimer;

		// Token: 0x040012EF RID: 4847
		private TempleMirrorPortal.Debris[] debris = new TempleMirrorPortal.Debris[50];

		// Token: 0x040012F0 RID: 4848
		private Color debrisColorFrom = Calc.HexToColor("f442d4");

		// Token: 0x040012F1 RID: 4849
		private Color debrisColorTo = Calc.HexToColor("000000");

		// Token: 0x040012F2 RID: 4850
		private MTexture debrisTexture = GFX.Game["particles/blob"];

		// Token: 0x040012F3 RID: 4851
		private TempleMirrorPortal.Curtain curtain;

		// Token: 0x040012F4 RID: 4852
		private TemplePortalTorch leftTorch;

		// Token: 0x040012F5 RID: 4853
		private TemplePortalTorch rightTorch;

		// Token: 0x02000671 RID: 1649
		private struct Debris
		{
			// Token: 0x04002ABB RID: 10939
			public Vector2 Direction;

			// Token: 0x04002ABC RID: 10940
			public float Percent;

			// Token: 0x04002ABD RID: 10941
			public float Duration;

			// Token: 0x04002ABE RID: 10942
			public bool Enabled;
		}

		// Token: 0x02000672 RID: 1650
		private class Bg : Entity
		{
			// Token: 0x06002B92 RID: 11154 RVA: 0x00117514 File Offset: 0x00115714
			public Bg(Vector2 position) : base(position)
			{
				base.Depth = 9500;
				this.textures = GFX.Game.GetAtlasSubtextures("objects/temple/portal/reflection");
				Vector2 value = new Vector2(10f, 4f);
				this.offsets = new Vector2[this.textures.Count];
				for (int i = 0; i < this.offsets.Length; i++)
				{
					this.offsets[i] = value + new Vector2((float)Calc.Random.Range(-4, 4), (float)Calc.Random.Range(-4, 4));
				}
				base.Add(this.surface = new MirrorSurface(null));
				this.surface.OnRender = delegate()
				{
					for (int j = 0; j < this.textures.Count; j++)
					{
						this.surface.ReflectionOffset = this.offsets[j];
						this.textures[j].DrawCentered(this.Position, this.surface.ReflectionColor);
					}
				};
			}

			// Token: 0x06002B93 RID: 11155 RVA: 0x001175E1 File Offset: 0x001157E1
			public override void Render()
			{
				GFX.Game["objects/temple/portal/surface"].DrawCentered(this.Position);
			}

			// Token: 0x04002ABF RID: 10943
			private MirrorSurface surface;

			// Token: 0x04002AC0 RID: 10944
			private Vector2[] offsets;

			// Token: 0x04002AC1 RID: 10945
			private List<MTexture> textures;
		}

		// Token: 0x02000673 RID: 1651
		private class Curtain : Solid
		{
			// Token: 0x06002B95 RID: 11157 RVA: 0x0011765C File Offset: 0x0011585C
			public Curtain(Vector2 position) : base(position, 140f, 12f, true)
			{
				base.Add(this.Sprite = GFX.SpriteBank.Create("temple_portal_curtain"));
				base.Depth = 1999;
				base.Collider.Position.X = -70f;
				base.Collider.Position.Y = 33f;
				this.Collidable = false;
				this.SurfaceSoundIndex = 17;
			}

			// Token: 0x06002B96 RID: 11158 RVA: 0x001176E0 File Offset: 0x001158E0
			public override void Update()
			{
				base.Update();
				if (this.Collidable)
				{
					Player player;
					if ((player = base.CollideFirst<Player>(this.Position + new Vector2(-1f, 0f))) != null && player.OnGround(1) && Input.Aim.Value.X > 0f)
					{
						player.MoveV(base.Top - player.Bottom, null, null);
						player.MoveH(1f, null, null);
						return;
					}
					if ((player = base.CollideFirst<Player>(this.Position + new Vector2(1f, 0f))) != null && player.OnGround(1) && Input.Aim.Value.X < 0f)
					{
						player.MoveV(base.Top - player.Bottom, null, null);
						player.MoveH(-1f, null, null);
					}
				}
			}

			// Token: 0x06002B97 RID: 11159 RVA: 0x001177D0 File Offset: 0x001159D0
			public void Drop()
			{
				this.Sprite.Play("fall", false, false);
				base.Depth = -8999;
				this.Collidable = true;
				bool flag = false;
				Player player;
				while ((player = base.CollideFirst<Player>(this.Position)) != null && !flag)
				{
					this.Collidable = false;
					flag = player.MoveV(-1f, null, null);
					this.Collidable = true;
				}
			}

			// Token: 0x04002AC2 RID: 10946
			public Sprite Sprite;
		}
	}
}
