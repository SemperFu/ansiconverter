﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



namespace My
{

	[System.Runtime.CompilerServices.CompilerGeneratedAttribute(), System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0"), System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
	internal sealed partial class MySettings : global::System.Configuration.ApplicationSettingsBase
	{

		private static MySettings defaultInstance = (MySettings)global::System.Configuration.ApplicationSettingsBase.Synchronized(new MySettings());

		#Region "My.Settings Auto-Save Functionality"
		#If _MyType = "WindowsForms" Then

		private static bool addedHandler;

		private static object addedHandlerLockObject = new object();
		[System.Diagnostics.DebuggerNonUserCodeAttribute(), System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		private static void AutoSaveSettings(global::System.Object sender, global::System.EventArgs e)
		{
			if (My.Application.SaveMySettingsOnExit) {
				My.Settings.Save();
			}
		}
		#End If
		#End Region

		public static MySettings Default {
			get {

				#If _MyType = "WindowsForms" Then
				if (!addedHandler) {
					lock (addedHandlerLockObject) {
						if (!addedHandler) {
							My.Application.Shutdown += AutoSaveSettings;
							addedHandler = true;
						}
					}
				}
				#End If
				return defaultInstance;
			}
		}
	}
}

namespace My
{

	[Microsoft.VisualBasic.HideModuleNameAttribute(), System.Diagnostics.DebuggerNonUserCodeAttribute(), System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class MySettingsProperty
	{

		[System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")]
		internal global::AnsiCPMaps.My.MySettings Settings {
			get { return global::AnsiCPMaps.My.MySettings.Default; }
		}
	}
}