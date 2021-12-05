using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000206 RID: 518
	public class ReflectionHeartStatue : Entity
	{
		// Token: 0x060010F0 RID: 4336 RVA: 0x00050C1C File Offset: 0x0004EE1C
		public ReflectionHeartStatue(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			this.offset = offset;
			this.nodes = data.Nodes;
			base.Depth = 8999;
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x00050C70 File Offset: 0x0004EE70
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Session session = (base.Scene as Level).Session;
			Image image = new Image(GFX.Game["objects/reflectionHeart/statue"]);
			image.JustifyOrigin(0.5f, 1f);
			Image image2 = image;
			image2.Origin.Y = image2.Origin.Y - 1f;
			base.Add(image);
			List<string[]> list = new List<string[]>();
			list.Add(ReflectionHeartStatue.Code);
			list.Add(this.FlipCode(true, false));
			list.Add(this.FlipCode(false, true));
			list.Add(this.FlipCode(true, true));
			for (int i = 0; i < 4; i++)
			{
				ReflectionHeartStatue.Torch torch = new ReflectionHeartStatue.Torch(session, this.offset + this.nodes[i], i, list[i]);
				base.Scene.Add(torch);
				this.torches.Add(torch);
			}
			int num = ReflectionHeartStatue.Code.Length;
			Vector2 value = this.nodes[4] + this.offset - this.Position;
			for (int j = 0; j < num; j++)
			{
				Image image3 = new Image(GFX.Game["objects/reflectionHeart/gem"]);
				image3.CenterOrigin();
				image3.Color = ForsakenCitySatellite.Colors[ReflectionHeartStatue.Code[j]];
				image3.Position = value + new Vector2(((float)j - (float)(num - 1) / 2f) * 24f, 0f);
				base.Add(image3);
				base.Add(new BloomPoint(image3.Position, 0.3f, 12f));
			}
			this.enabled = !session.HeartGem;
			if (this.enabled)
			{
				base.Add(this.dashListener = new DashListener());
				this.dashListener.OnDash = delegate(Vector2 dir)
				{
					string text = "";
					if (dir.Y < 0f)
					{
						text = "U";
					}
					else if (dir.Y > 0f)
					{
						text = "D";
					}
					if (dir.X < 0f)
					{
						text += "L";
					}
					else if (dir.X > 0f)
					{
						text += "R";
					}
					int num2 = 0;
					if (dir.X < 0f && dir.Y == 0f)
					{
						num2 = 1;
					}
					else if (dir.X < 0f && dir.Y < 0f)
					{
						num2 = 2;
					}
					else if (dir.X == 0f && dir.Y < 0f)
					{
						num2 = 3;
					}
					else if (dir.X > 0f && dir.Y < 0f)
					{
						num2 = 4;
					}
					else if (dir.X > 0f && dir.Y == 0f)
					{
						num2 = 5;
					}
					else if (dir.X > 0f && dir.Y > 0f)
					{
						num2 = 6;
					}
					else if (dir.X == 0f && dir.Y > 0f)
					{
						num2 = 7;
					}
					else if (dir.X < 0f && dir.Y > 0f)
					{
						num2 = 8;
					}
					Player entity = base.Scene.Tracker.GetEntity<Player>();
					Audio.Play("event:/game/06_reflection/supersecret_dashflavour", (entity != null) ? entity.Position : Vector2.Zero, "dash_direction", (float)num2);
					this.currentInputs.Add(text);
					if (this.currentInputs.Count > ReflectionHeartStatue.Code.Length)
					{
						this.currentInputs.RemoveAt(0);
					}
					foreach (ReflectionHeartStatue.Torch torch2 in this.torches)
					{
						if (!torch2.Activated && this.CheckCode(torch2.Code))
						{
							torch2.Activate();
						}
					}
					this.CheckIfAllActivated(false);
				};
				this.CheckIfAllActivated(true);
			}
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x00050E78 File Offset: 0x0004F078
		private string[] FlipCode(bool h, bool v)
		{
			string[] array = new string[ReflectionHeartStatue.Code.Length];
			for (int i = 0; i < ReflectionHeartStatue.Code.Length; i++)
			{
				string text = ReflectionHeartStatue.Code[i];
				if (h)
				{
					text = (text.Contains('L') ? text.Replace('L', 'R') : text.Replace('R', 'L'));
				}
				if (v)
				{
					text = (text.Contains('U') ? text.Replace('U', 'D') : text.Replace('D', 'U'));
				}
				array[i] = text;
			}
			return array;
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x00050EFC File Offset: 0x0004F0FC
		private bool CheckCode(string[] code)
		{
			if (this.currentInputs.Count < code.Length)
			{
				return false;
			}
			for (int i = 0; i < code.Length; i++)
			{
				if (!this.currentInputs[i].Equals(code[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00050F44 File Offset: 0x0004F144
		private void CheckIfAllActivated(bool skipActivateRoutine = false)
		{
			if (this.enabled)
			{
				bool flag = true;
				using (List<ReflectionHeartStatue.Torch>.Enumerator enumerator = this.torches.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Activated)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					this.Activate(skipActivateRoutine);
				}
			}
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x00050FAC File Offset: 0x0004F1AC
		public void Activate(bool skipActivateRoutine)
		{
			this.enabled = false;
			if (skipActivateRoutine)
			{
				base.Scene.Add(new HeartGem(this.Position + new Vector2(0f, -52f)));
				return;
			}
			base.Add(new Coroutine(this.ActivateRoutine(), true));
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x00051000 File Offset: 0x0004F200
		private IEnumerator ActivateRoutine()
		{
			yield return 0.533f;
			Audio.Play("event:/game/06_reflection/supersecret_heartappear");
			Entity dummy = new Entity(this.Position + new Vector2(0f, -52f));
			dummy.Depth = 1;
			base.Scene.Add(dummy);
			Image white = new Image(GFX.Game["collectables/heartgem/white00"]);
			white.CenterOrigin();
			white.Scale = Vector2.Zero;
			dummy.Add(white);
			BloomPoint glow = new BloomPoint(0f, 16f);
			dummy.Add(glow);
			List<Entity> absorbs = new List<Entity>();
			int num;
			for (int i = 0; i < 20; i = num + 1)
			{
				AbsorbOrb absorbOrb = new AbsorbOrb(this.Position + new Vector2(0f, -20f), dummy, null);
				base.Scene.Add(absorbOrb);
				absorbs.Add(absorbOrb);
				yield return null;
				num = i;
			}
			yield return 0.8f;
			float duration = 0.6f;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
			{
				white.Scale = Vector2.One * p;
				glow.Alpha = p;
				(base.Scene as Level).Shake(0.3f);
				yield return null;
			}
			foreach (Entity entity in absorbs)
			{
				entity.RemoveSelf();
			}
			(base.Scene as Level).Flash(Color.White, false);
			base.Scene.Remove(dummy);
			base.Scene.Add(new HeartGem(this.Position + new Vector2(0f, -52f)));
			yield break;
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x0005100F File Offset: 0x0004F20F
		public override void Update()
		{
			if (this.dashListener != null && !this.enabled)
			{
				base.Remove(this.dashListener);
				this.dashListener = null;
			}
			base.Update();
		}

		// Token: 0x04000C90 RID: 3216
		private static readonly string[] Code = new string[]
		{
			"U",
			"L",
			"DR",
			"UR",
			"L",
			"UL"
		};

		// Token: 0x04000C91 RID: 3217
		private const string FlagPrefix = "heartTorch_";

		// Token: 0x04000C92 RID: 3218
		private List<string> currentInputs = new List<string>();

		// Token: 0x04000C93 RID: 3219
		private List<ReflectionHeartStatue.Torch> torches = new List<ReflectionHeartStatue.Torch>();

		// Token: 0x04000C94 RID: 3220
		private Vector2 offset;

		// Token: 0x04000C95 RID: 3221
		private Vector2[] nodes;

		// Token: 0x04000C96 RID: 3222
		private DashListener dashListener;

		// Token: 0x04000C97 RID: 3223
		private bool enabled;

		// Token: 0x02000511 RID: 1297
		public class Torch : Entity
		{
			// Token: 0x17000443 RID: 1091
			// (get) Token: 0x06002518 RID: 9496 RVA: 0x000F71D8 File Offset: 0x000F53D8
			public string Flag
			{
				get
				{
					return "heartTorch_" + this.Index;
				}
			}

			// Token: 0x17000444 RID: 1092
			// (get) Token: 0x06002519 RID: 9497 RVA: 0x000F71EF File Offset: 0x000F53EF
			public bool Activated
			{
				get
				{
					return this.session.GetFlag(this.Flag);
				}
			}

			// Token: 0x17000445 RID: 1093
			// (get) Token: 0x0600251A RID: 9498 RVA: 0x000F7202 File Offset: 0x000F5402
			// (set) Token: 0x0600251B RID: 9499 RVA: 0x000F720A File Offset: 0x000F540A
			public int Index { get; private set; }

			// Token: 0x0600251C RID: 9500 RVA: 0x000F7214 File Offset: 0x000F5414
			public Torch(Session session, Vector2 position, int index, string[] code) : base(position)
			{
				this.Index = index;
				this.Code = code;
				base.Depth = 8999;
				this.session = session;
				Image image = new Image(GFX.Game.GetAtlasSubtextures("objects/reflectionHeart/hint")[index]);
				image.CenterOrigin();
				image.Position = new Vector2(0f, 28f);
				base.Add(image);
				base.Add(this.sprite = new Sprite(GFX.Game, "objects/reflectionHeart/torch"));
				this.sprite.AddLoop("idle", "", 0f, new int[1]);
				this.sprite.AddLoop("lit", "", 0.08f, new int[]
				{
					1,
					2,
					3,
					4,
					5,
					6
				});
				this.sprite.Play("idle", false, false);
				this.sprite.Origin = new Vector2(32f, 64f);
			}

			// Token: 0x0600251D RID: 9501 RVA: 0x000F731C File Offset: 0x000F551C
			public override void Added(Scene scene)
			{
				base.Added(scene);
				if (this.Activated)
				{
					this.PlayLit();
				}
			}

			// Token: 0x0600251E RID: 9502 RVA: 0x000F7333 File Offset: 0x000F5533
			public void Activate()
			{
				this.session.SetFlag(this.Flag, true);
				Alarm.Set(this, 0.2f, delegate
				{
					Audio.Play("event:/game/06_reflection/supersecret_torch_" + (this.Index + 1), this.Position);
					this.PlayLit();
				}, Alarm.AlarmMode.Oneshot);
			}

			// Token: 0x0600251F RID: 9503 RVA: 0x000F7360 File Offset: 0x000F5560
			private void PlayLit()
			{
				this.sprite.Play("lit", false, false);
				this.sprite.SetAnimationFrame(Calc.Random.Next(this.sprite.CurrentAnimationTotalFrames));
				base.Add(new VertexLight(Color.LightSeaGreen, 1f, 24, 48));
				base.Add(new BloomPoint(0.6f, 16f));
			}

			// Token: 0x040024D6 RID: 9430
			public string[] Code;

			// Token: 0x040024D7 RID: 9431
			private Sprite sprite;

			// Token: 0x040024D8 RID: 9432
			private Session session;
		}
	}
}
