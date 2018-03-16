namespace ConverterSupport
{
	//  Public Structure WebFont
	// Public Name As String
	// Public StaticEXTRACSSDIV As String
	// Public StaticEXTRACSSPRE As String
	// Public StaticEXTRACSSSPAN As String
	// Public AnimEXTRACSSDIV As String
	// Public AnimEXTRACSSPRE As String
	// Public AnimEXTRACSSSPAN As String
	// Public Sub New(ByVal n As String, ByVal sDIV As String, ByVal sPRE As String, ByVal sSPAN As String, ByVal aDIV As String, ByVal aPRE As String, ByVal aSPAN As String)
	//    Name = n
	//    StaticEXTRACSSDIV = sDIV
	//    StaticEXTRACSSPRE = sPRE
	//    StaticEXTRACSSSPAN = sSPAN
	//    AnimEXTRACSSDIV = aDIV
	//    AnimEXTRACSSPRE = aPRE
	//    AnimEXTRACSSSPAN = aSPAN
	//End Sub
	// End Structure

	public class WebFontDef
	{
		private string sName;
		private string sStaticEXTRACSSDIV;
		private string sStaticEXTRACSSPRE;
		private string sStaticEXTRACSSSPAN;
		private string sAnimEXTRACSSDIV;
		private string sAnimEXTRACSSPRE;

		private string sAnimEXTRACSSSPAN;
		public WebFontDef(string n, string sDIV, string sPRE, string sSPAN, string aDIV, string aPRE, string aSPAN)
		{
			this.sName = n;
			this.sStaticEXTRACSSDIV = sDIV;
			this.sStaticEXTRACSSPRE = sPRE;
			this.sStaticEXTRACSSSPAN = sSPAN;
			this.sAnimEXTRACSSDIV = aDIV;
			this.sAnimEXTRACSSPRE = aPRE;
			this.sAnimEXTRACSSSPAN = aSPAN;
		}
		//New

		public string Name {
			get { return sName; }
		}

		public string StaticEXTRACSSDIV {
			get { return sStaticEXTRACSSDIV; }
		}
		public string StaticEXTRACSSPRE {
			get { return sStaticEXTRACSSPRE; }
		}
		public string StaticEXTRACSSSPAN {
			get { return sStaticEXTRACSSSPAN; }
		}
		public string AnimEXTRACSSDIV {
			get { return sAnimEXTRACSSDIV; }
		}
		public string AnimEXTRACSSPRE {
			get { return sAnimEXTRACSSPRE; }
		}
		public string AnimEXTRACSSSPAN {
			get { return sAnimEXTRACSSSPAN; }
		}
	}
	//WebFontDef
}