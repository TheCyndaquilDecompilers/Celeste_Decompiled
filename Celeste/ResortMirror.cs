using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D1 RID: 721
	public class ResortMirror : Entity
	{
		// Token: 0x06001642 RID: 5698 RVA: 0x00082A08 File Offset: 0x00080C08
		public ResortMirror(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			base.Depth = 9500;
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x00082A74 File Offset: 0x00080C74
		public override void Added(Scene scene)
		{
			ResortMirror.<>c__DisplayClass11_0 CS$<>8__locals1 = new ResortMirror.<>c__DisplayClass11_0();
			CS$<>8__locals1.<>4__this = this;
			base.Added(scene);
			this.smashed = base.SceneAs<Level>().Session.GetFlag("oshiro_resort_suite");
			Entity entity = new Entity(this.Position);
			entity.Depth = 9000;
			entity.Add(this.frame = new Image(GFX.Game["objects/mirror/resortframe"]));
			this.frame.JustifyOrigin(0.5f, 1f);
			base.Scene.Add(entity);
			CS$<>8__locals1.glassbg = GFX.Game["objects/mirror/glassbg"];
			CS$<>8__locals1.w = (int)this.frame.Width - 2;
			CS$<>8__locals1.h = (int)this.frame.Height - 12;
			if (!this.smashed)
			{
				this.mirror = VirtualContent.CreateRenderTarget("resort-mirror", CS$<>8__locals1.w, CS$<>8__locals1.h, false, true, 0);
			}
			else
			{
				CS$<>8__locals1.glassbg = GFX.Game["objects/mirror/glassbreak09"];
			}
			base.Add(this.bg = new Image(CS$<>8__locals1.glassbg.GetSubtexture((CS$<>8__locals1.glassbg.Width - CS$<>8__locals1.w) / 2, CS$<>8__locals1.glassbg.Height - CS$<>8__locals1.h, CS$<>8__locals1.w, CS$<>8__locals1.h, null)));
			this.bg.JustifyOrigin(0.5f, 1f);
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/mirror/mirrormask");
			CS$<>8__locals1.temp = new MTexture();
			using (List<MTexture>.Enumerator enumerator = atlasSubtextures.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ResortMirror.<>c__DisplayClass11_1 CS$<>8__locals2 = new ResortMirror.<>c__DisplayClass11_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.shard = enumerator.Current;
					MirrorSurface surface = new MirrorSurface(null);
					surface.OnRender = delegate()
					{
						CS$<>8__locals2.shard.GetSubtexture((CS$<>8__locals2.CS$<>8__locals1.glassbg.Width - CS$<>8__locals2.CS$<>8__locals1.w) / 2, CS$<>8__locals2.CS$<>8__locals1.glassbg.Height - CS$<>8__locals2.CS$<>8__locals1.h, CS$<>8__locals2.CS$<>8__locals1.w, CS$<>8__locals2.CS$<>8__locals1.h, CS$<>8__locals2.CS$<>8__locals1.temp).DrawJustified(CS$<>8__locals2.CS$<>8__locals1.<>4__this.Position, new Vector2(0.5f, 1f), surface.ReflectionColor * (float)(CS$<>8__locals2.CS$<>8__locals1.<>4__this.shardReflection ? 1 : 0));
					};
					surface.ReflectionOffset = new Vector2((float)(9 + Calc.Random.Range(-4, 4)), (float)(4 + Calc.Random.Range(-2, 2)));
					base.Add(surface);
				}
			}
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x00082CD4 File Offset: 0x00080ED4
		public void EvilAppear()
		{
			base.Add(new Coroutine(this.EvilAppearRoutine(), true));
			base.Add(new Coroutine(this.FadeLights(), true));
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x00082CFA File Offset: 0x00080EFA
		private IEnumerator EvilAppearRoutine()
		{
			this.evil = new BadelineDummy(new Vector2((float)(this.mirror.Width + 8), (float)this.mirror.Height));
			yield return this.evil.WalkTo((float)(this.mirror.Width / 2), 64f);
			yield break;
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x00082D09 File Offset: 0x00080F09
		private IEnumerator FadeLights()
		{
			Level level = base.SceneAs<Level>();
			while (level.Lighting.Alpha != 0.35f)
			{
				level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, 0.35f, Engine.DeltaTime * 0.1f);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x00082D18 File Offset: 0x00080F18
		public IEnumerator SmashRoutine()
		{
			yield return this.evil.FloatTo(new Vector2((float)(this.mirror.Width / 2), (float)(this.mirror.Height - 8)), null, true, false, false);
			this.breakingGlass = GFX.SpriteBank.Create("glass");
			this.breakingGlass.Position = new Vector2((float)(this.mirror.Width / 2), (float)this.mirror.Height);
			this.breakingGlass.Play("break", false, false);
			this.breakingGlass.Color = Color.White * this.shineAlpha;
			Input.Rumble(RumbleStrength.Light, RumbleLength.FullSecond);
			while (this.breakingGlass.CurrentAnimationID == "break")
			{
				if (this.breakingGlass.CurrentAnimationFrame == 7)
				{
					base.SceneAs<Level>().Shake(0.3f);
				}
				this.shineAlpha = Calc.Approach(this.shineAlpha, 1f, Engine.DeltaTime * 2f);
				this.mirrorAlpha = Calc.Approach(this.mirrorAlpha, 1f, Engine.DeltaTime * 2f);
				yield return null;
			}
			base.SceneAs<Level>().Shake(0.3f);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			for (float num = -this.breakingGlass.Width / 2f; num < this.breakingGlass.Width / 2f; num += 8f)
			{
				for (float num2 = -this.breakingGlass.Height; num2 < 0f; num2 += 8f)
				{
					if (Calc.Random.Chance(0.5f))
					{
						(base.Scene as Level).Particles.Emit(DreamMirror.P_Shatter, 2, this.Position + new Vector2(num + 4f, num2 + 4f), new Vector2(8f, 8f), new Vector2(num, num2).Angle());
					}
				}
			}
			this.shardReflection = true;
			this.evil = null;
			yield break;
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x00082D28 File Offset: 0x00080F28
		public void Broken()
		{
			this.evil = null;
			this.smashed = true;
			this.shardReflection = true;
			MTexture mtexture = GFX.Game["objects/mirror/glassbreak09"];
			this.bg.Texture = mtexture.GetSubtexture((int)((float)mtexture.Width - this.bg.Width) / 2, mtexture.Height - (int)this.bg.Height, (int)this.bg.Width, (int)this.bg.Height, null);
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x00082DB0 File Offset: 0x00080FB0
		private void BeforeRender()
		{
			if (!this.smashed && this.mirror != null)
			{
				Level level = base.SceneAs<Level>();
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.mirror);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
				NPC npc = base.Scene.Entities.FindFirst<NPC>();
				if (npc != null)
				{
					Vector2 renderPosition = npc.Sprite.RenderPosition;
					npc.Sprite.RenderPosition = renderPosition - this.Position + new Vector2((float)(this.mirror.Width / 2), (float)this.mirror.Height) + new Vector2(8f, -4f);
					npc.Sprite.Render();
					npc.Sprite.RenderPosition = renderPosition;
				}
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					Vector2 position = entity.Position;
					entity.Position = position - this.Position + new Vector2((float)(this.mirror.Width / 2), (float)this.mirror.Height) + new Vector2(8f, 0f);
					Vector2 value = entity.Position - position;
					for (int i = 0; i < entity.Hair.Nodes.Count; i++)
					{
						List<Vector2> nodes = entity.Hair.Nodes;
						int index = i;
						nodes[index] += value;
					}
					entity.Render();
					for (int j = 0; j < entity.Hair.Nodes.Count; j++)
					{
						List<Vector2> nodes = entity.Hair.Nodes;
						int index = j;
						nodes[index] -= value;
					}
					entity.Position = position;
				}
				if (this.evil != null)
				{
					this.evil.Update();
					this.evil.Hair.Facing = (Facings)Math.Sign(this.evil.Sprite.Scale.X);
					this.evil.Hair.AfterUpdate();
					this.evil.Render();
				}
				if (this.breakingGlass != null)
				{
					this.breakingGlass.Color = Color.White * this.shineAlpha;
					this.breakingGlass.Update();
					this.breakingGlass.Render();
				}
				else
				{
					int num = -(int)(level.Camera.Y * 0.8f % (float)this.glassfg.Height);
					this.glassfg.DrawJustified(new Vector2((float)(this.mirror.Width / 2), (float)num), new Vector2(0.5f, 1f), Color.White * this.shineAlpha);
					this.glassfg.DrawJustified(new Vector2((float)(this.mirror.Height / 2), (float)(num - this.glassfg.Height)), new Vector2(0.5f, 1f), Color.White * this.shineAlpha);
				}
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x00083114 File Offset: 0x00081314
		public override void Render()
		{
			this.bg.Render();
			if (!this.smashed)
			{
				Draw.SpriteBatch.Draw(this.mirror, this.Position + new Vector2((float)(-(float)this.mirror.Width / 2), (float)(-(float)this.mirror.Height)), Color.White * this.mirrorAlpha);
			}
			this.frame.Render();
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x00083190 File Offset: 0x00081390
		public override void SceneEnd(Scene scene)
		{
			this.Dispose();
			base.SceneEnd(scene);
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x0008319F File Offset: 0x0008139F
		public override void Removed(Scene scene)
		{
			this.Dispose();
			base.Removed(scene);
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x000831AE File Offset: 0x000813AE
		private void Dispose()
		{
			if (this.mirror != null)
			{
				this.mirror.Dispose();
			}
			this.mirror = null;
		}

		// Token: 0x0400129D RID: 4765
		private bool smashed;

		// Token: 0x0400129E RID: 4766
		private Image bg;

		// Token: 0x0400129F RID: 4767
		private Image frame;

		// Token: 0x040012A0 RID: 4768
		private MTexture glassfg = GFX.Game["objects/mirror/glassfg"];

		// Token: 0x040012A1 RID: 4769
		private Sprite breakingGlass;

		// Token: 0x040012A2 RID: 4770
		private VirtualRenderTarget mirror;

		// Token: 0x040012A3 RID: 4771
		private float shineAlpha = 0.7f;

		// Token: 0x040012A4 RID: 4772
		private float mirrorAlpha = 0.7f;

		// Token: 0x040012A5 RID: 4773
		private BadelineDummy evil;

		// Token: 0x040012A6 RID: 4774
		private bool shardReflection;
	}
}
