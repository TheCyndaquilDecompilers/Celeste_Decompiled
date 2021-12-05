using System;
using System.Collections.Generic;
using System.Threading;
using Monocle;

namespace Celeste
{
	// Token: 0x02000382 RID: 898
	public static class RunThread
	{
		// Token: 0x06001D64 RID: 7524 RVA: 0x000CCA6C File Offset: 0x000CAC6C
		public static void Start(Action method, string name, bool highPriority = false)
		{
			Thread thread = new Thread(delegate()
			{
				RunThread.RunThreadWithLogging(method);
			});
			List<Thread> obj = RunThread.threads;
			lock (obj)
			{
				RunThread.threads.Add(thread);
			}
			thread.Name = name;
			thread.IsBackground = true;
			if (highPriority)
			{
				thread.Priority = ThreadPriority.Highest;
			}
			thread.Start();
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x000CCAEC File Offset: 0x000CACEC
		private static void RunThreadWithLogging(Action method)
		{
			try
			{
				method();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				ErrorLog.Write(ex);
				ErrorLog.Open();
				Engine.Instance.Exit();
			}
			finally
			{
				List<Thread> obj = RunThread.threads;
				lock (obj)
				{
					RunThread.threads.Remove(Thread.CurrentThread);
				}
			}
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x000CCB78 File Offset: 0x000CAD78
		public static void WaitAll()
		{
			for (;;)
			{
				List<Thread> obj = RunThread.threads;
				Thread thread;
				lock (obj)
				{
					if (RunThread.threads.Count == 0)
					{
						break;
					}
					thread = RunThread.threads[0];
				}
				thread.Join();
			}
		}

		// Token: 0x04001AF1 RID: 6897
		private static List<Thread> threads = new List<Thread>();
	}
}
