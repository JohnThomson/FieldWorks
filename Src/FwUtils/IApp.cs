// Copyright (c) 2003-2020 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Windows.Forms;
using Microsoft.Win32;
using SIL.FieldWorks.Common.ViewsInterfaces;
using SIL.LCModel;

namespace SIL.FieldWorks.Common.FwUtils
{
	/// <summary>
	/// Interface for application.
	/// </summary>
	public interface IApp : IDisposable, IHelpTopicProvider, IFeedbackInfoProvider, IMessageFilter
	{
		/// <summary>
		/// Return a string from a resource ID.
		/// </summary>
		/// <param name="stid">String resource id</param>
		string ResourceString(string stid);

		/// <summary>
		/// Gets/Sets the measurement system used in the application
		/// </summary>
		MsrSysType MeasurementSystem { get; set; }

		/// <summary>
		/// Get the active form. This is usually the same as Form.ActiveForm, but sometimes
		/// the official active form is something other than one of our main windows, for
		/// example, a dialog or popup menu. This is always one of our real main windows,
		/// which should be something that has a taskbar icon. It is often useful as the
		/// appropriate parent window for a dialog that otherwise doesn't have one.
		/// </summary>
		Form ActiveMainWindow { get; }

		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		string ApplicationName { get; }

		/// <summary>
		/// Get the LCM cache.
		/// </summary>
		LcmCache Cache { get; }

		/// <summary>
		/// A place to get various pictures.
		/// </summary>
		PictureHolder PictureHolder { get; }

		/// <summary>
		/// Refreshes all the views in all of the Main Windows of the app.
		/// </summary>
		void RefreshAllViews();

		/// <summary>
		/// Restart the spell-checking process (e.g. when dictionary changed)
		/// </summary>
		void RestartSpellChecking();

		/// <summary>
		/// Returns a key in the registry where "Persistence" should store settings.
		/// </summary>
		RegistryKey SettingsKey { get; }

		/// <summary>
		/// Cycle through the applications main windows and synchronize them with database
		/// changes.
		/// </summary>
		/// <param name="sync">synchronization information record</param>
		/// <returns><c>true</c> to continue processing; set to <c>false</c> to prevent
		/// processing of subsequent sync messages. </returns>
		bool Synchronize(SyncMsg sync);

		/// <summary>
		/// Enable or disable all top-level windows. This allows nesting. In other words,
		/// calling EnableMainWindows(false) twice requires 2 calls to EnableMainWindows(true)
		/// before the top level windows are actually enabled.
		/// </summary>
		/// <param name="fEnable">Enable (true) or disable (false).</param>
		void EnableMainWindows(bool fEnable);

		/// <summary>
		/// Closes and disposes of the find replace dialog.
		/// </summary>
		void RemoveFindReplaceDialog();

		/// <summary>
		/// Display the Find/Replace modeless dialog
		/// </summary>
		/// <param name="fReplace"><c>true</c> to make the replace tab active</param>
		/// <param name="rootsite">The view where the find will be conducted</param>
		/// <param name="cache"></param>
		/// <param name="mainForm"></param>
		/// <returns><c>true</c> if the dialog is successfully displayed</returns>
		bool ShowFindReplaceDialog(bool fReplace, IVwRootSite rootsite, LcmCache cache, Form mainForm);

		/// <summary>
		/// Handle incoming links.
		/// </summary>
		/// <param name="link">The link to handle.</param>
		/// <remarks>
		/// This method is  called from FieldWorks when a link is requested. It is guaranteed to be on the
		/// correct thread (the thread this application is on) so invoking should not be needed.
		///
		/// See the class comment on FwLinkArgs for details on how all the parts of hyperlinking work.
		/// </remarks>
		void HandleIncomingLink(FwLinkArgs link);

		/// <summary>
		/// Handles an outgoing link request from this application.
		/// </summary>
		void HandleOutgoingLink(FwAppArgs link);

		/// <summary>
		/// Handle changes to the LinkedFiles root directory for a language project.
		/// </summary>
		/// <param name="oldLinkedFilesRootDir">The old LinkedFiles root directory.</param>
		bool UpdateExternalLinks(string oldLinkedFilesRootDir);
	}
}