using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200034A RID: 842
	public class SwitchGate : Solid
	{
		// Token: 0x06001A6B RID: 6763 RVA: 0x000AA1AC File Offset: 0x000A83AC
		public SwitchGate(Vector2 position, float width, float height, Vector2 node, bool persistent, string spriteName) : base(position, width, height, false)
		{
			this.node = node;
			this.persistent = persistent;
			base.Add(this.icon = new Sprite(GFX.Game, "objects/switchgate/icon"));
			this.icon.Add("spin", "", 0.1f, "spin");
			this.icon.Play("spin", false, false);
			this.icon.Rate = 0f;
			this.icon.Color = this.inactiveColor;
			this.icon.Position = (this.iconOffset = new Vector2(width / 2f, height / 2f));
			this.icon.CenterOrigin();
			base.Add(this.wiggler = Wiggler.Create(0.5f, 4f, delegate(float f)
			{
				this.icon.Scale = Vector2.One * (1f + f);
			}, false, false));
			MTexture mtexture = GFX.Game["objects/switchgate/" + spriteName];
			this.nineSlice = new MTexture[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					this.nineSlice[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
				}
			}
			base.Add(this.openSfx = new SoundSource());
			base.Add(new LightOcclude(0.5f));
		}

		// Token: 0x06001A6C RID: 6764 RVA: 0x000AA35C File Offset: 0x000A855C
		public SwitchGate(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Nodes[0] + offset, data.Bool("persistent", false), data.Attr("sprite", "block"))
		{
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x000AA3B8 File Offset: 0x000A85B8
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (Switch.CheckLevelFlag(base.SceneAs<Level>()))
			{
				base.MoveTo(this.node);
				this.icon.Rate = 0f;
				this.icon.SetAnimationFrame(0);
				this.icon.Color = this.finishColor;
				return;
			}
			base.Add(new Coroutine(this.Sequence(this.node), true));
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x000AA42C File Offset: 0x000A862C
		public override void Render()
		{
			float num = base.Collider.Width / 8f - 1f;
			float num2 = base.Collider.Height / 8f - 1f;
			int num3 = 0;
			while ((float)num3 <= num)
			{
				int num4 = 0;
				while ((float)num4 <= num2)
				{
					int num5 = ((float)num3 < num) ? Math.Min(num3, 1) : 2;
					int num6 = ((float)num4 < num2) ? Math.Min(num4, 1) : 2;
					this.nineSlice[num5, num6].Draw(this.Position + base.Shake + new Vector2((float)(num3 * 8), (float)(num4 * 8)));
					num4++;
				}
				num3++;
			}
			this.icon.Position = this.iconOffset + base.Shake;
			this.icon.DrawOutline(1);
			base.Render();
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x000AA50A File Offset: 0x000A870A
		private IEnumerator Sequence(Vector2 node)
		{
			SwitchGate.<>c__DisplayClass16_0 CS$<>8__locals1 = new SwitchGate.<>c__DisplayClass16_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.node = node;
			CS$<>8__locals1.start = this.Position;
			while (!Switch.Check(base.Scene))
			{
				yield return null;
			}
			if (this.persistent)
			{
				Switch.SetLevelFlag(base.SceneAs<Level>());
			}
			yield return 0.1f;
			this.openSfx.Play("event:/game/general/touchswitch_gate_open", null, 0f);
			base.StartShaking(0.5f);
			while (this.icon.Rate < 1f)
			{
				this.icon.Color = Color.Lerp(this.inactiveColor, this.activeColor, this.icon.Rate);
				this.icon.Rate += Engine.DeltaTime * 2f;
				yield return null;
			}
			yield return 0.1f;
			int particleAt = 0;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 2f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				CS$<>8__locals1.<>4__this.MoveTo(Vector2.Lerp(CS$<>8__locals1.start, CS$<>8__locals1.node, t.Eased));
				if (CS$<>8__locals1.<>4__this.Scene.OnInterval(0.1f))
				{
					int particleAt = particleAt;
					particleAt++;
					particleAt %= 2;
					int num6 = 0;
					while ((float)num6 < CS$<>8__locals1.<>4__this.Width / 8f)
					{
						int num7 = 0;
						while ((float)num7 < CS$<>8__locals1.<>4__this.Height / 8f)
						{
							if ((num6 + num7) % 2 == particleAt)
							{
								CS$<>8__locals1.<>4__this.SceneAs<Level>().ParticlesBG.Emit(SwitchGate.P_Behind, CS$<>8__locals1.<>4__this.Position + new Vector2((float)(num6 * 8), (float)(num7 * 8)) + Calc.Random.Range(Vector2.One * 2f, Vector2.One * 6f));
							}
							num7++;
						}
						num6++;
					}
				}
			};
			base.Add(tween);
			yield return 1.8f;
			bool collidable = this.Collidable;
			this.Collidable = false;
			if (CS$<>8__locals1.node.X <= CS$<>8__locals1.start.X)
			{
				Vector2 value = new Vector2(0f, 2f);
				int num = 0;
				while ((float)num < base.Height / 8f)
				{
					Vector2 vector = new Vector2(base.Left - 1f, base.Top + 4f + (float)(num * 8));
					Vector2 point = vector + Vector2.UnitX;
					if (base.Scene.CollideCheck<Solid>(vector) && !base.Scene.CollideCheck<Solid>(point))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, vector + value, 3.1415927f);
						base.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, vector - value, 3.1415927f);
					}
					num++;
				}
			}
			if (CS$<>8__locals1.node.X >= CS$<>8__locals1.start.X)
			{
				Vector2 value2 = new Vector2(0f, 2f);
				int num2 = 0;
				while ((float)num2 < base.Height / 8f)
				{
					Vector2 vector2 = new Vector2(base.Right + 1f, base.Top + 4f + (float)(num2 * 8));
					Vector2 point2 = vector2 - Vector2.UnitX * 2f;
					if (base.Scene.CollideCheck<Solid>(vector2) && !base.Scene.CollideCheck<Solid>(point2))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, vector2 + value2, 0f);
						base.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, vector2 - value2, 0f);
					}
					num2++;
				}
			}
			if (CS$<>8__locals1.node.Y <= CS$<>8__locals1.start.Y)
			{
				Vector2 value3 = new Vector2(2f, 0f);
				int num3 = 0;
				while ((float)num3 < base.Width / 8f)
				{
					Vector2 vector3 = new Vector2(base.Left + 4f + (float)(num3 * 8), base.Top - 1f);
					Vector2 point3 = vector3 + Vector2.UnitY;
					if (base.Scene.CollideCheck<Solid>(vector3) && !base.Scene.CollideCheck<Solid>(point3))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, vector3 + value3, -1.5707964f);
						base.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, vector3 - value3, -1.5707964f);
					}
					num3++;
				}
			}
			if (CS$<>8__locals1.node.Y >= CS$<>8__locals1.start.Y)
			{
				Vector2 value4 = new Vector2(2f, 0f);
				int num4 = 0;
				while ((float)num4 < base.Width / 8f)
				{
					Vector2 vector4 = new Vector2(base.Left + 4f + (float)(num4 * 8), base.Bottom + 1f);
					Vector2 point4 = vector4 - Vector2.UnitY * 2f;
					if (base.Scene.CollideCheck<Solid>(vector4) && !base.Scene.CollideCheck<Solid>(point4))
					{
						base.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, vector4 + value4, 1.5707964f);
						base.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, vector4 - value4, 1.5707964f);
					}
					num4++;
				}
			}
			this.Collidable = collidable;
			Audio.Play("event:/game/general/touchswitch_gate_finish", this.Position);
			base.StartShaking(0.2f);
			while (this.icon.Rate > 0f)
			{
				this.icon.Color = Color.Lerp(this.activeColor, this.finishColor, 1f - this.icon.Rate);
				this.icon.Rate -= Engine.DeltaTime * 4f;
				yield return null;
			}
			this.icon.Rate = 0f;
			this.icon.SetAnimationFrame(0);
			this.wiggler.Start();
			bool collidable2 = this.Collidable;
			this.Collidable = false;
			if (!base.Scene.CollideCheck<Solid>(base.Center))
			{
				for (int i = 0; i < 32; i++)
				{
					float num5 = Calc.Random.NextFloat(6.2831855f);
					base.SceneAs<Level>().ParticlesFG.Emit(TouchSwitch.P_Fire, this.Position + this.iconOffset + Calc.AngleToVector(num5, 4f), num5);
				}
			}
			this.Collidable = collidable2;
			yield break;
		}

		// Token: 0x04001709 RID: 5897
		public static ParticleType P_Behind;

		// Token: 0x0400170A RID: 5898
		public static ParticleType P_Dust;

		// Token: 0x0400170B RID: 5899
		private MTexture[,] nineSlice;

		// Token: 0x0400170C RID: 5900
		private Sprite icon;

		// Token: 0x0400170D RID: 5901
		private Vector2 iconOffset;

		// Token: 0x0400170E RID: 5902
		private Wiggler wiggler;

		// Token: 0x0400170F RID: 5903
		private Vector2 node;

		// Token: 0x04001710 RID: 5904
		private SoundSource openSfx;

		// Token: 0x04001711 RID: 5905
		private bool persistent;

		// Token: 0x04001712 RID: 5906
		private Color inactiveColor = Calc.HexToColor("5fcde4");

		// Token: 0x04001713 RID: 5907
		private Color activeColor = Color.White;

		// Token: 0x04001714 RID: 5908
		private Color finishColor = Calc.HexToColor("f141df");
	}
}
