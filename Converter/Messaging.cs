namespace Messaging
{
	class Messaging
	{
		public struct fmts
		{
			public string elem;
			public fnts fnt;
			public Stack<int> start;
			fmts(string e, fnts f)
			{
				elem = e;
				fnt = f;
				start = new Stack<int>();
			}
		}
		public enum fnts
		{
			bold = 1,
			italic = 2,
			underline = 3,
			err = 4
		}
		public struct sel
		{
			public fnts format;
			public int fromval;
			public int toval;
			sel(fnts fn, int f, int t)
			{
				format = fn;
				fromval = f;
				toval = t;
			}
		}

	}
}