using System;
using System.Drawing;
using System.Windows.Forms;
using static Data;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.Interaction;
/// <summary>
/// Represents a list of files in list <see cref="ListInputFiles"/> to be processed by the converter
/// </summary>
/// <remarks></remarks>
public class FileListItem : System.Windows.Forms.Control
{

	private string m_Name;
	private string m_FullPath;
	private FFormats m_Format;
	private Color m_Color;
	private System.Windows.Forms.Label m_Label;
	private bool m_Selected;

	private FTypes m_FType;
	/// <summary>
	///  Gets or Sets the Current <see cref="MForm"/> where the converter assembly is attached to
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public System.Windows.Forms.Form Form {
		get { return Data.MForm; }
		set { MForm = value; }
	}
	/// <summary>
	/// Gets or Sets the Current <see cref="ToolTip"/> Extender Control
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public System.Windows.Forms.ToolTip TT {
		get { return Data.ToolTip; }
		set { Data.ToolTip = value; }
	}
	/// <summary>
	/// Main Constructor for this class, pre-setting vital properties right away
	/// </summary>
	/// <param name="Name">File Name (String)</param>
	/// <param name="FullPath">Full Path of File (String)</param>
	/// <param name="Format">File Format as <see cref="FFormats"/></param>
	/// <param name="FTyp">Optional File Type as <see cref="FTypes"/>, Default = 'FTypes.ASCII'</param>
	/// <remarks></remarks>
	public FileListItem(string Name, string FullPath, FFormats Format, FTypes FTyp = FTypes.ASCII)
	{
		this.m_Name = Name;
		this.m_FullPath = FullPath;
		this.m_Format = Format;
		switch (Format) {
			case FFormats.us_ascii:
				this.m_Color = Color.Black;
				break;
			case FFormats.utf_16:
				this.m_Color = Color.Blue;
				break;
			case FFormats.utf_8:
				this.m_Color = Color.Red;
				break;
			default:
				this.m_Color = Color.Black;
				break;
		}
		this.m_Label = new System.Windows.Forms.Label();
		this.m_Label.Text = this.m_Name;
		this.m_Label.ForeColor = this.m_Color;
		this.m_Label.BackColor = Color.White;
		this.m_FType = FTyp;
		MForm.Controls.Add(this.Label);
		//AddHandler Me.m_Label.Click, AddressOf filelistitem_click
		this.m_Selected = false;
		MForm.Controls.Add(this);
		ToolTipAndEvents(this, this.FullPath);
		ToolTipAndEvents(this.Label, this.FullPath);
	}
	//New
	/// <summary>
	/// Default constructor without any pre-set parameters
	/// </summary>
	/// <remarks></remarks>
	public FileListItem()
	{
		this.m_Name = "";
		this.m_FullPath = "";
		this.m_Format = FFormats.us_ascii;
		this.m_Color = Color.Red;
		this.m_Label = new System.Windows.Forms.Label();
		this.m_Label.Text = this.m_Name;
		this.m_Label.ForeColor = this.m_Color;
		this.m_Label.BackColor = Color.White;
		this.m_FType = FTypes.ASCII;
		//AddHandler Me.m_Label.Click, AddressOf filelistitem_click
		this.m_Selected = false;
		MForm.Controls.Add(this.Label);
		MForm.Controls.Add(this);
		ToolTipAndEvents(this, this.FullPath);
		ToolTipAndEvents(this.Label, this.FullPath);
	}

	/// <summary>
	/// Gets or Sets if the Current Item is Selected or Not
	/// Toggles to Background Color of the Item between <see cref="System.Drawing.Color.White"/> and <see cref="System.Drawing.Color.Yellow"/>
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public bool Selected {
		get { return this.m_Selected; }
		set {
			if (value != this.m_Selected) {
				this.m_Selected = value;
				if (this.m_Label.BackColor == Color.White) {
					this.m_Label.BackColor = Color.Yellow;
				} else {
					this.m_Label.BackColor = Color.White;
				}
			}

		}
	}
	/// <summary>
	/// returns the default ForeColor used for the control depending of <see cref="Format"/> of the item
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public Color Color {
		get { return this.m_Color; }
	}
	/// <summary>
	/// The <see cref="Windows.Forms.Label"/> Control that represents the item
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public System.Windows.Forms.Label Label {
		get { return this.m_Label; }
	}
	/// <summary>
	/// returns the Full Name and Path of the File
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public string FullPath {
		get { return this.m_FullPath; }
	}
	/// <summary>
	/// returns the File Name of the File
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public string Name {
		get { return this.m_Name; }
	}
	/// <summary>
	/// Returns the detected <see cref="FFormats"/> of Item
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public FFormats Format {
		get { return this.m_Format; }
	}
	/// <summary>
	/// Returns the detected <see cref="FTypes"/> of the Item
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public FTypes Type {
		get { return this.m_FType; }
	}

	/// <summary>
	/// Sets the new Tooltip Value for the specified <paramref>fctrl</paramref> on <see cref="ToolTip"/>
	/// </summary>
	/// <param name="fctrl"></param>
	/// <param name="sStr"></param>
	/// <remarks></remarks>
	public void ToolTipAndEvents(System.Windows.Forms.Control fctrl, string sStr)
	{
		Data.ToolTip.SetToolTip(fctrl, sStr);
		fctrl.MouseHover += tooltip_MouseHover;
		fctrl.MouseLeave += tooltip_MouseLeave;
	}
	public void tooltip_MouseHover(System.Object sender, System.EventArgs e)
	{
		if (sender.Cursor != Cursors.Hand) {
			sender.Cursor = Cursors.Help;
		}
	}
	public void tooltip_MouseLeave(System.Object sender, System.EventArgs e)
	{
		if (sender.cursor == Cursors.Help) {
			sender.Cursor = Cursors.Default;
		}
	}
}