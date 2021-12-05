using System;

namespace FMOD
{
	// Token: 0x02000038 RID: 56
	public struct CREATESOUNDEXINFO
	{
		// Token: 0x040001A2 RID: 418
		public int cbsize;

		// Token: 0x040001A3 RID: 419
		public uint length;

		// Token: 0x040001A4 RID: 420
		public uint fileoffset;

		// Token: 0x040001A5 RID: 421
		public int numchannels;

		// Token: 0x040001A6 RID: 422
		public int defaultfrequency;

		// Token: 0x040001A7 RID: 423
		public SOUND_FORMAT format;

		// Token: 0x040001A8 RID: 424
		public uint decodebuffersize;

		// Token: 0x040001A9 RID: 425
		public int initialsubsound;

		// Token: 0x040001AA RID: 426
		public int numsubsounds;

		// Token: 0x040001AB RID: 427
		public IntPtr inclusionlist;

		// Token: 0x040001AC RID: 428
		public int inclusionlistnum;

		// Token: 0x040001AD RID: 429
		public SOUND_PCMREADCALLBACK pcmreadcallback;

		// Token: 0x040001AE RID: 430
		public SOUND_PCMSETPOSCALLBACK pcmsetposcallback;

		// Token: 0x040001AF RID: 431
		public SOUND_NONBLOCKCALLBACK nonblockcallback;

		// Token: 0x040001B0 RID: 432
		public IntPtr dlsname;

		// Token: 0x040001B1 RID: 433
		public IntPtr encryptionkey;

		// Token: 0x040001B2 RID: 434
		public int maxpolyphony;

		// Token: 0x040001B3 RID: 435
		public IntPtr userdata;

		// Token: 0x040001B4 RID: 436
		public SOUND_TYPE suggestedsoundtype;

		// Token: 0x040001B5 RID: 437
		public FILE_OPENCALLBACK fileuseropen;

		// Token: 0x040001B6 RID: 438
		public FILE_CLOSECALLBACK fileuserclose;

		// Token: 0x040001B7 RID: 439
		public FILE_READCALLBACK fileuserread;

		// Token: 0x040001B8 RID: 440
		public FILE_SEEKCALLBACK fileuserseek;

		// Token: 0x040001B9 RID: 441
		public FILE_ASYNCREADCALLBACK fileuserasyncread;

		// Token: 0x040001BA RID: 442
		public FILE_ASYNCCANCELCALLBACK fileuserasynccancel;

		// Token: 0x040001BB RID: 443
		public IntPtr fileuserdata;

		// Token: 0x040001BC RID: 444
		public int filebuffersize;

		// Token: 0x040001BD RID: 445
		public CHANNELORDER channelorder;

		// Token: 0x040001BE RID: 446
		public CHANNELMASK channelmask;

		// Token: 0x040001BF RID: 447
		public IntPtr initialsoundgroup;

		// Token: 0x040001C0 RID: 448
		public uint initialseekposition;

		// Token: 0x040001C1 RID: 449
		public TIMEUNIT initialseekpostype;

		// Token: 0x040001C2 RID: 450
		public int ignoresetfilesystem;

		// Token: 0x040001C3 RID: 451
		public uint audioqueuepolicy;

		// Token: 0x040001C4 RID: 452
		public uint minmidigranularity;

		// Token: 0x040001C5 RID: 453
		public int nonblockthreadid;

		// Token: 0x040001C6 RID: 454
		public IntPtr fsbguid;
	}
}
