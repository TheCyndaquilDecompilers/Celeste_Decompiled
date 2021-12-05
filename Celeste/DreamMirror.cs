using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020002D5 RID: 725
	public class DreamMirror : Entity
	{
		// Token: 0x06001664 RID: 5732 RVA: 0x00083BEC File Offset: 0x00081DEC
		public DreamMirror(Vector2 position) : base(position)
		{
			base.Depth = 9500;
			base.Add(this.breakingGlass = GFX.SpriteBank.Create("glass"));
			this.breakingGlass.Play("idle", false, false);
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			using (List<MTexture>.Enumerator enumerator = GFX.Game.GetAtlasSubtextures("objects/mirror/mirrormask").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DreamMirror.<>c__DisplayClass21_0 CS$<>8__locals1 = new DreamMirror.<>c__DisplayClass21_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.shard = enumerator.Current;
					MirrorSurface surface = new MirrorSurface(null);
					surface.OnRender = delegate()
					{
						CS$<>8__locals1.shard.DrawJustified(CS$<>8__locals1.<>4__this.Position, new Vector2(0.5f, 1f), surface.ReflectionColor * (float)(CS$<>8__locals1.<>4__this.smashEnded ? 1 : 0));
					};
					surface.ReflectionOffset = new Vector2((float)(9 + Calc.Random.Range(-4, 4)), (float)(4 + Calc.Random.Range(-2, 2)));
					base.Add(surface);
				}
			}
		}

		// Token: 0x06001665 RID: 5733 RVA: 0x00083D70 File Offset: 0x00081F70
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.smashed = base.SceneAs<Level>().Session.Inventory.DreamDash;
			if (this.smashed)
			{
				this.breakingGlass.Play("broken", false, false);
				this.smashEnded = true;
			}
			else
			{
				this.reflection = new Entity();
				this.reflectionSprite = new PlayerSprite(PlayerSpriteMode.Badeline);
				this.reflectionHair = new PlayerHair(this.reflectionSprite);
				this.reflectionHair.Color = BadelineOldsite.HairColor;
				this.reflectionHair.Border = Color.Black;
				this.reflection.Add(this.reflectionHair);
				this.reflection.Add(this.reflectionSprite);
				this.reflectionHair.Start();
				this.reflectionSprite.OnFrameChange = delegate(string anim)
				{
					if (!this.smashed && base.CollideCheck<Player>())
					{
						int currentAnimationFrame = this.reflectionSprite.CurrentAnimationFrame;
						if ((anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "runSlow" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "runFast" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)))
						{
							Audio.Play("event:/char/badeline/footstep", base.Center);
						}
					}
				};
				base.Add(this.smashCoroutine = new Coroutine(this.InteractRoutine(), true));
			}
			Entity entity = new Entity(this.Position);
			entity.Depth = 9000;
			entity.Add(this.frame = new Image(GFX.Game["objects/mirror/frame"]));
			this.frame.JustifyOrigin(0.5f, 1f);
			base.Scene.Add(entity);
			base.Collider = (this.hitbox = new Hitbox((float)((int)this.frame.Width - 16), (float)((int)this.frame.Height + 32), (float)(-(float)((int)this.frame.Width) / 2 + 8), (float)(-(float)((int)this.frame.Height) - 32)));
		}

		// Token: 0x06001666 RID: 5734 RVA: 0x00083F20 File Offset: 0x00082120
		public override void Update()
		{
			base.Update();
			if (this.reflection != null)
			{
				this.reflection.Update();
				this.reflectionHair.Facing = (Facings)Math.Sign(this.reflectionSprite.Scale.X);
				this.reflectionHair.AfterUpdate();
			}
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x00083F74 File Offset: 0x00082174
		private void BeforeRender()
		{
			if (!this.smashed)
			{
				Level level = base.Scene as Level;
				Player entity = base.Scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					if (this.autoUpdateReflection && this.reflection != null)
					{
						this.reflection.Position = new Vector2(base.X - entity.X, entity.Y - base.Y) + this.breakingGlass.Origin;
						this.reflectionSprite.Scale.X = (float)(-(float)entity.Facing) * Math.Abs(entity.Sprite.Scale.X);
						this.reflectionSprite.Scale.Y = entity.Sprite.Scale.Y;
						if (this.reflectionSprite.CurrentAnimationID != entity.Sprite.CurrentAnimationID && entity.Sprite.CurrentAnimationID != null && this.reflectionSprite.Has(entity.Sprite.CurrentAnimationID))
						{
							this.reflectionSprite.Play(entity.Sprite.CurrentAnimationID, false, false);
						}
					}
					if (this.mirror == null)
					{
						this.mirror = VirtualContent.CreateRenderTarget("dream-mirror", this.glassbg.Width, this.glassbg.Height, false, true, 0);
					}
					Engine.Graphics.GraphicsDevice.SetRenderTarget(this.mirror);
					Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
					Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
					if (this.updateShine)
					{
						this.shineOffset = (float)(this.glassfg.Height - (int)(level.Camera.Y * 0.8f % (float)this.glassfg.Height));
					}
					this.glassbg.Draw(Vector2.Zero);
					if (this.reflection != null)
					{
						this.reflection.Render();
					}
					this.glassfg.Draw(new Vector2(0f, this.shineOffset), Vector2.Zero, Color.White * this.shineAlpha);
					this.glassfg.Draw(new Vector2(0f, this.shineOffset - (float)this.glassfg.Height), Vector2.Zero, Color.White * this.shineAlpha);
					Draw.SpriteBatch.End();
				}
			}
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x000841F9 File Offset: 0x000823F9
		private IEnumerator InteractRoutine()
		{
			Player player = null;
			while (player == null)
			{
				player = base.Scene.Tracker.GetEntity<Player>();
				yield return null;
			}
			while (!this.hitbox.Collide(player))
			{
				yield return null;
			}
			this.hitbox.Width += 32f;
			Hitbox hitbox = this.hitbox;
			hitbox.Position.X = hitbox.Position.X - 16f;
			Audio.SetMusic(null, true, true);
			while (this.hitbox.Collide(player))
			{
				yield return null;
			}
			base.Scene.Add(new CS02_Mirror(player, this));
			yield break;
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x00084208 File Offset: 0x00082408
		public IEnumerator BreakRoutine(int direction)
		{
			this.autoUpdateReflection = false;
			this.reflectionSprite.Play("runFast", false, false);
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
			while (Math.Abs(this.reflection.X - this.breakingGlass.Width / 2f) > 3f)
			{
				this.reflection.X += (float)(direction * 32) * Engine.DeltaTime;
				yield return null;
			}
			this.reflectionSprite.Play("idle", false, false);
			yield return 0.65f;
			base.Add(this.sfx = new SoundSource());
			this.sfx.Play("event:/game/02_old_site/sequence_mirror", null, 0f);
			yield return 0.15f;
			base.Add(this.sfxSting = new SoundSource("event:/music/lvl2/dreamblock_sting_pt2"));
			Input.Rumble(RumbleStrength.Light, RumbleLength.FullSecond);
			this.updateShine = false;
			while (this.shineOffset != 33f || this.shineAlpha < 1f)
			{
				this.shineOffset = Calc.Approach(this.shineOffset, 33f, Engine.DeltaTime * 120f);
				this.shineAlpha = Calc.Approach(this.shineAlpha, 1f, Engine.DeltaTime * 4f);
				yield return null;
			}
			this.smashed = true;
			this.breakingGlass.Play("break", false, false);
			yield return 0.6f;
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
			(base.Scene as Level).Shake(0.3f);
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
			this.smashEnded = true;
			this.badeline = new BadelineDummy(this.reflection.Position + this.Position - this.breakingGlass.Origin);
			this.badeline.Floatness = 0f;
			for (int i = 0; i < this.badeline.Hair.Nodes.Count; i++)
			{
				this.badeline.Hair.Nodes[i] = this.reflectionHair.Nodes[i];
			}
			base.Scene.Add(this.badeline);
			this.badeline.Sprite.Play("idle", false, false);
			this.badeline.Sprite.Scale = this.reflectionSprite.Scale;
			this.reflection = null;
			yield return 1.2f;
			float speed = (float)(-(float)direction) * 32f;
			this.badeline.Sprite.Scale.X = (float)(-(float)direction);
			this.badeline.Sprite.Play("runFast", false, false);
			while (Math.Abs(this.badeline.X - base.X) < 60f)
			{
				speed += Engine.DeltaTime * (float)(-(float)direction) * 128f;
				this.badeline.X += speed * Engine.DeltaTime;
				yield return null;
			}
			this.badeline.Sprite.Play("jumpFast", false, false);
			while (Math.Abs(this.badeline.X - base.X) < 128f)
			{
				speed += Engine.DeltaTime * (float)(-(float)direction) * 128f;
				this.badeline.X += speed * Engine.DeltaTime;
				this.badeline.Y -= Math.Abs(speed) * Engine.DeltaTime * 0.8f;
				yield return null;
			}
			this.badeline.RemoveSelf();
			this.badeline = null;
			yield return 1.5f;
			yield break;
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x00084220 File Offset: 0x00082420
		public void Broken(bool wasSkipped)
		{
			this.updateShine = false;
			this.smashed = true;
			this.smashEnded = true;
			this.breakingGlass.Play("broken", false, false);
			if (wasSkipped && this.badeline != null)
			{
				this.badeline.RemoveSelf();
			}
			if (wasSkipped && this.sfx != null)
			{
				this.sfx.Stop(true);
			}
			if (wasSkipped && this.sfxSting != null)
			{
				this.sfxSting.Stop(true);
			}
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x0008429C File Offset: 0x0008249C
		public override void Render()
		{
			if (this.smashed)
			{
				this.breakingGlass.Render();
			}
			else
			{
				Draw.SpriteBatch.Draw(this.mirror.Target, this.Position - this.breakingGlass.Origin, Color.White * this.reflectionAlpha);
			}
			this.frame.Render();
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x00084304 File Offset: 0x00082504
		public override void SceneEnd(Scene scene)
		{
			this.Dispose();
			base.SceneEnd(scene);
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x00084313 File Offset: 0x00082513
		public override void Removed(Scene scene)
		{
			this.Dispose();
			base.Removed(scene);
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x00084322 File Offset: 0x00082522
		private void Dispose()
		{
			if (this.mirror != null)
			{
				this.mirror.Dispose();
			}
			this.mirror = null;
		}

		// Token: 0x040012BD RID: 4797
		public static ParticleType P_Shatter;

		// Token: 0x040012BE RID: 4798
		private Image frame;

		// Token: 0x040012BF RID: 4799
		private MTexture glassbg = GFX.Game["objects/mirror/glassbg"];

		// Token: 0x040012C0 RID: 4800
		private MTexture glassfg = GFX.Game["objects/mirror/glassfg"];

		// Token: 0x040012C1 RID: 4801
		private Sprite breakingGlass;

		// Token: 0x040012C2 RID: 4802
		private Hitbox hitbox;

		// Token: 0x040012C3 RID: 4803
		private VirtualRenderTarget mirror;

		// Token: 0x040012C4 RID: 4804
		private float shineAlpha = 0.5f;

		// Token: 0x040012C5 RID: 4805
		private float shineOffset;

		// Token: 0x040012C6 RID: 4806
		private Entity reflection;

		// Token: 0x040012C7 RID: 4807
		private PlayerSprite reflectionSprite;

		// Token: 0x040012C8 RID: 4808
		private PlayerHair reflectionHair;

		// Token: 0x040012C9 RID: 4809
		private float reflectionAlpha = 0.7f;

		// Token: 0x040012CA RID: 4810
		private bool autoUpdateReflection = true;

		// Token: 0x040012CB RID: 4811
		private BadelineDummy badeline;

		// Token: 0x040012CC RID: 4812
		private bool smashed;

		// Token: 0x040012CD RID: 4813
		private bool smashEnded;

		// Token: 0x040012CE RID: 4814
		private bool updateShine = true;

		// Token: 0x040012CF RID: 4815
		private Coroutine smashCoroutine;

		// Token: 0x040012D0 RID: 4816
		private SoundSource sfx;

		// Token: 0x040012D1 RID: 4817
		private SoundSource sfxSting;
	}
}
