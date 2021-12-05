using System;
using System.Collections;

namespace Monocle
{
	// Token: 0x02000102 RID: 258
	public class StateMachine : Component
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060006FA RID: 1786 RVA: 0x0000C095 File Offset: 0x0000A295
		// (set) Token: 0x060006FB RID: 1787 RVA: 0x0000C09D File Offset: 0x0000A29D
		public int PreviousState { get; private set; }

		// Token: 0x060006FC RID: 1788 RVA: 0x0000C0A8 File Offset: 0x0000A2A8
		public StateMachine(int maxStates = 10) : base(true, false)
		{
			this.PreviousState = (this.state = -1);
			this.begins = new Action[maxStates];
			this.updates = new Func<int>[maxStates];
			this.ends = new Action[maxStates];
			this.coroutines = new Func<IEnumerator>[maxStates];
			this.currentCoroutine = new Coroutine(true);
			this.currentCoroutine.RemoveOnComplete = false;
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0000C115 File Offset: 0x0000A315
		public override void Added(Entity entity)
		{
			base.Added(entity);
			if (base.Entity.Scene != null && this.state == -1)
			{
				this.State = 0;
			}
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0000C13B File Offset: 0x0000A33B
		public override void EntityAdded(Scene scene)
		{
			base.EntityAdded(scene);
			if (this.state == -1)
			{
				this.State = 0;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060006FF RID: 1791 RVA: 0x0000C154 File Offset: 0x0000A354
		// (set) Token: 0x06000700 RID: 1792 RVA: 0x0000C15C File Offset: 0x0000A35C
		public int State
		{
			get
			{
				return this.state;
			}
			set
			{
				if (!this.Locked && this.state != value)
				{
					if (this.Log)
					{
						Calc.Log(new object[]
						{
							string.Concat(new object[]
							{
								"Enter State ",
								value,
								" (leaving ",
								this.state,
								")"
							})
						});
					}
					this.ChangedStates = true;
					this.PreviousState = this.state;
					this.state = value;
					if (this.PreviousState != -1 && this.ends[this.PreviousState] != null)
					{
						if (this.Log)
						{
							Calc.Log(new object[]
							{
								"Calling End " + this.PreviousState
							});
						}
						this.ends[this.PreviousState]();
					}
					if (this.begins[this.state] != null)
					{
						if (this.Log)
						{
							Calc.Log(new object[]
							{
								"Calling Begin " + this.state
							});
						}
						this.begins[this.state]();
					}
					if (this.coroutines[this.state] != null)
					{
						if (this.Log)
						{
							Calc.Log(new object[]
							{
								"Starting Coroutine " + this.state
							});
						}
						this.currentCoroutine.Replace(this.coroutines[this.state]());
						return;
					}
					this.currentCoroutine.Cancel();
				}
			}
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0000C2F0 File Offset: 0x0000A4F0
		public void ForceState(int toState)
		{
			if (this.state != toState)
			{
				this.State = toState;
				return;
			}
			if (this.Log)
			{
				Calc.Log(new object[]
				{
					string.Concat(new object[]
					{
						"Enter State ",
						toState,
						" (leaving ",
						this.state,
						")"
					})
				});
			}
			this.ChangedStates = true;
			this.PreviousState = this.state;
			this.state = toState;
			if (this.PreviousState != -1 && this.ends[this.PreviousState] != null)
			{
				if (this.Log)
				{
					Calc.Log(new object[]
					{
						"Calling End " + this.state
					});
				}
				this.ends[this.PreviousState]();
			}
			if (this.begins[this.state] != null)
			{
				if (this.Log)
				{
					Calc.Log(new object[]
					{
						"Calling Begin " + this.state
					});
				}
				this.begins[this.state]();
			}
			if (this.coroutines[this.state] != null)
			{
				if (this.Log)
				{
					Calc.Log(new object[]
					{
						"Starting Coroutine " + this.state
					});
				}
				this.currentCoroutine.Replace(this.coroutines[this.state]());
				return;
			}
			this.currentCoroutine.Cancel();
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0000C47C File Offset: 0x0000A67C
		public void SetCallbacks(int state, Func<int> onUpdate, Func<IEnumerator> coroutine = null, Action begin = null, Action end = null)
		{
			this.updates[state] = onUpdate;
			this.begins[state] = begin;
			this.ends[state] = end;
			this.coroutines[state] = coroutine;
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x0000C4A4 File Offset: 0x0000A6A4
		public void ReflectState(Entity from, int index, string name)
		{
			this.updates[index] = (Func<int>)Calc.GetMethod<Func<int>>(from, name + "Update");
			this.begins[index] = (Action)Calc.GetMethod<Action>(from, name + "Begin");
			this.ends[index] = (Action)Calc.GetMethod<Action>(from, name + "End");
			this.coroutines[index] = (Func<IEnumerator>)Calc.GetMethod<Func<IEnumerator>>(from, name + "Coroutine");
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x0000C52C File Offset: 0x0000A72C
		public override void Update()
		{
			this.ChangedStates = false;
			if (this.updates[this.state] != null)
			{
				this.State = this.updates[this.state]();
			}
			if (this.currentCoroutine.Active)
			{
				this.currentCoroutine.Update();
				if (!this.ChangedStates && this.Log && this.currentCoroutine.Finished)
				{
					Calc.Log(new object[]
					{
						"Finished Coroutine " + this.state
					});
				}
			}
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0000C154 File Offset: 0x0000A354
		public static implicit operator int(StateMachine s)
		{
			return s.state;
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x0000C5C0 File Offset: 0x0000A7C0
		public void LogAllStates()
		{
			for (int i = 0; i < this.updates.Length; i++)
			{
				this.LogState(i);
			}
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x0000C5E8 File Offset: 0x0000A7E8
		public void LogState(int index)
		{
			Calc.Log(new object[]
			{
				string.Concat(new object[]
				{
					"State ",
					index,
					": ",
					(this.updates[index] != null) ? "U" : "",
					(this.begins[index] != null) ? "B" : "",
					(this.ends[index] != null) ? "E" : "",
					(this.coroutines[index] != null) ? "C" : ""
				})
			});
		}

		// Token: 0x04000522 RID: 1314
		private int state;

		// Token: 0x04000523 RID: 1315
		private Action[] begins;

		// Token: 0x04000524 RID: 1316
		private Func<int>[] updates;

		// Token: 0x04000525 RID: 1317
		private Action[] ends;

		// Token: 0x04000526 RID: 1318
		private Func<IEnumerator>[] coroutines;

		// Token: 0x04000527 RID: 1319
		private Coroutine currentCoroutine;

		// Token: 0x04000528 RID: 1320
		public bool ChangedStates;

		// Token: 0x04000529 RID: 1321
		public bool Log;

		// Token: 0x0400052B RID: 1323
		public bool Locked;
	}
}
