// Copyright (c) 2003-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Windows.Forms;

namespace LanguageExplorer.Works
{
	/// <summary>
	/// Parameter object used by ListUpdateHelper in its construction.
	/// </summary>
	internal sealed class ListUpdateHelperOptions
	{
		/// <summary>
		/// Set/reset WaitCursor during operation
		/// </summary>
		internal Control ParentForWaitCursor { get; set; }
		/// <summary>
		/// Indicate that we want to clear browse items while we are
		/// waiting for a pending reload, so that the display will not
		/// try to access invalid objects.
		/// </summary>
		internal bool ClearBrowseListUntilReload { get; set; }
		/// <summary>
		/// Some user actions (e.g. editing) should not result in record navigation
		/// because it may cause the editing pane to disappear
		/// (thus losing the user's place in editing). This is used by ListUpdateHelper
		/// to skip record navigations while such user actions are taking place.
		/// </summary>
		internal bool SkipShowRecord { get; set; }
		/// <summary>
		/// </summary>
		internal bool SuppressSaveOnChangeRecord { get; set; }
		/// <summary>
		/// Set to false if you don't want to automatically reload pending reload OnDispose.
		/// </summary>
		internal bool SuspendPendingReloadOnDispose { get; set; }
		/// <summary>
		/// Use to suspend PropChanged while modifying list.
		/// </summary>
		internal bool SuspendPropChangedDuringModification { get; set; }
	}
}