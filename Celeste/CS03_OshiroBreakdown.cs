using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000263 RID: 611
	public class CS03_OshiroBreakdown : CutsceneEntity
	{
		// Token: 0x06001302 RID: 4866 RVA: 0x000675AF File Offset: 0x000657AF
		public CS03_OshiroBreakdown(Player player, NPC oshiro) : base(true, false)
		{
			this.oshiro = oshiro;
			this.player = player;
			this.origin = oshiro.Position;
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x000675E9 File Offset: 0x000657E9
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x000675FE File Offset: 0x000657FE
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			base.Add(new Coroutine(this.player.DummyWalkTo(this.player.X - 64f, false, 1f, false), true));
			List<DustStaticSpinner> list = level.Entities.FindAll<DustStaticSpinner>();
			list.Shuffle<DustStaticSpinner>();
			foreach (DustStaticSpinner dustStaticSpinner in list)
			{
				if ((dustStaticSpinner.Position - this.oshiro.Position).Length() < 128f)
				{
					this.creatures.Add(dustStaticSpinner);
					this.creatureHomes.Add(dustStaticSpinner.Position);
					dustStaticSpinner.Visible = false;
				}
			}
			yield return this.PanCamera((float)level.Bounds.Left);
			yield return 0.2f;
			yield return this.Level.ZoomTo(new Vector2(100f, 120f), 2f, 0.5f);
			yield return Textbox.Say("CH3_OSHIRO_BREAKDOWN", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.WalkLeft),
				new Func<IEnumerator>(this.WalkRight),
				new Func<IEnumerator>(this.CreateDustA),
				new Func<IEnumerator>(this.CreateDustB)
			});
			base.Add(new Coroutine(this.oshiro.MoveTo(new Vector2((float)(level.Bounds.Left - 64), this.oshiro.Y), false, null, false), true));
			this.oshiro.Add(new SoundSource("event:/char/oshiro/move_06_04d_exit"));
			yield return 0.25f;
			yield return this.PanCamera(this.player.CameraTarget.X);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x00067614 File Offset: 0x00065814
		private IEnumerator PanCamera(float to)
		{
			float from = this.Level.Camera.X;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				this.Level.Camera.X = from + (to - from) * Ease.CubeInOut(p);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0006762A File Offset: 0x0006582A
		private IEnumerator WalkLeft()
		{
			(this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
			yield return this.oshiro.MoveTo(this.origin + new Vector2(-24f, 0f), false, null, false);
			(this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = true;
			yield break;
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x00067639 File Offset: 0x00065839
		private IEnumerator WalkRight()
		{
			(this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
			yield return this.oshiro.MoveTo(this.origin + new Vector2(0f, 0f), false, null, false);
			(this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = true;
			yield break;
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x00067648 File Offset: 0x00065848
		private IEnumerator CreateDustA()
		{
			base.Add(new SoundSource(this.oshiro.Position, "event:/game/03_resort/sequence_oshirofluff_pt1"));
			(this.oshiro.Sprite as OshiroSprite).AllowSpriteChanges = false;
			this.oshiro.Sprite.Play("fall", false, false);
			Audio.Play("event:/char/oshiro/chat_collapse", this.oshiro.Position);
			Distort.AnxietyOrigin = new Vector2(0.5f, 0.5f);
			int num;
			for (int i = 0; i < 4; i = num + 1)
			{
				base.Add(new Coroutine(this.MoveDust(this.creatures[i], this.creatureHomes[i]), true));
				Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
				if (i % 4 == 0)
				{
					Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
					this.Level.Shake(0.3f);
					Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
					yield return 0.4f;
				}
				else
				{
					yield return 0.1f;
				}
				num = i;
			}
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x00067657 File Offset: 0x00065857
		private IEnumerator CreateDustB()
		{
			base.Add(new SoundSource(this.oshiro.Position, "event:/game/03_resort/sequence_oshirofluff_pt2"));
			int num;
			for (int i = 4; i < this.creatures.Count; i = num + 1)
			{
				base.Add(new Coroutine(this.MoveDust(this.creatures[i], this.creatureHomes[i]), true));
				Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
				this.Level.Shake(0.3f);
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
				if ((i - 4) % 4 == 0)
				{
					Distort.Anxiety = 0.1f + Calc.Random.NextFloat(0.1f);
					yield return 0.4f;
				}
				else
				{
					yield return 0.1f;
				}
				num = i;
			}
			yield return 1f;
			while (Distort.Anxiety > 0f)
			{
				Distort.Anxiety -= Engine.DeltaTime;
				yield return null;
			}
			yield return this.Level.ZoomBack(0.5f);
			yield return this.player.DummyWalkToExact(this.Level.Bounds.Left + 200, false, 1f, false);
			yield return 1f;
			Audio.Play("event:/char/oshiro/chat_get_up", this.oshiro.Position);
			this.oshiro.Sprite.Play("recover", false, false);
			yield return 0.7f;
			this.oshiro.Sprite.Scale.X = 1f;
			yield return 0.5f;
			yield break;
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x00067666 File Offset: 0x00065866
		private IEnumerator MoveDust(DustStaticSpinner creature, Vector2 to)
		{
			Vector2 vector = this.oshiro.Position + new Vector2(0f, -12f);
			SimpleCurve curve = new SimpleCurve(vector, to, (to + vector) / 2f + Vector2.UnitY * (-30f + Calc.Random.NextFloat(60f)));
			for (float p = 0f; p < 1f; p += Engine.DeltaTime)
			{
				yield return null;
				creature.Sprite.Scale = 0.5f + p * 0.5f;
				creature.Position = curve.GetPoint(Ease.CubeOut(p));
				creature.Visible = true;
				if (base.Scene.OnInterval(0.02f))
				{
					base.SceneAs<Level>().ParticlesBG.Emit(DustStaticSpinner.P_Move, 1, creature.Position, Vector2.One * 4f);
				}
			}
			yield break;
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x00067684 File Offset: 0x00065884
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			if (this.WasSkipped)
			{
				this.player.X = (float)(level.Bounds.Left + 200);
				while (!this.player.OnGround(1))
				{
					Player player = this.player;
					float y = player.Y;
					player.Y = y + 1f;
				}
				for (int i = 0; i < this.creatures.Count; i++)
				{
					this.creatures[i].ForceInstantiate();
					this.creatures[i].Visible = true;
					this.creatures[i].Position = this.creatureHomes[i];
				}
			}
			level.Camera.Position = this.player.CameraTarget;
			level.Remove(this.oshiro);
			level.Session.SetFlag("oshiro_breakdown", true);
		}

		// Token: 0x04000EF7 RID: 3831
		public const string Flag = "oshiro_breakdown";

		// Token: 0x04000EF8 RID: 3832
		private const int PlayerWalkTo = 200;

		// Token: 0x04000EF9 RID: 3833
		private List<DustStaticSpinner> creatures = new List<DustStaticSpinner>();

		// Token: 0x04000EFA RID: 3834
		private List<Vector2> creatureHomes = new List<Vector2>();

		// Token: 0x04000EFB RID: 3835
		private NPC oshiro;

		// Token: 0x04000EFC RID: 3836
		private Player player;

		// Token: 0x04000EFD RID: 3837
		private Vector2 origin;

		// Token: 0x04000EFE RID: 3838
		private const int DustAmountA = 4;
	}
}
