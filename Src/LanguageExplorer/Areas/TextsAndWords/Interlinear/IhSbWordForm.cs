// Copyright (c) 2006-2020 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Diagnostics;
using System.Windows.Forms;

namespace LanguageExplorer.Areas.TextsAndWords.Interlinear
{
	/// <summary>
	/// The actual form of the word. Eventually we will offer a popup representing all the
	/// currently known possible analyses, and other options.
	/// </summary>
	internal class IhSbWordForm : InterlinComboHandler
	{
		public override void SetupCombo()
		{
			base.SetupCombo();
			ComboList.Items.Add(ITextStrings.ksAcceptEntireAnalysis);
			ComboList.Items.Add(ITextStrings.ksEditThisWordform);
			ComboList.Items.Add(ITextStrings.ksDeleteThisWordform);
			ComboList.DropDownStyle = ComboBoxStyle.DropDownList; // Prevents direct editing.
		}

		public override void HandleSelect(int index)
		{
			switch (index)
			{
				case 0: // Accept entire analysis
						// Todo: figure how to implement.
					break;
				case 1: // Edit this wordform.
					// Allows direct editing.
					ComboList.DropDownStyle = ComboBoxStyle.DropDown;
					// restore the combo to visibility so we can do the editing.
					m_sandbox.ShowCombo();
					break;
				case 2: // Delete this wordform.
					// Todo: figure implementation
					// Enhance JohnT: consider removing the WfiWordform, if there are no
					// analyses and no other references.
					// Comment: RandyR: Please don't delete it.
					break;
			}
		}

		public override bool HandleReturnKey()
		{
			// If it hasn't changed don't do anything.
			var newval = ComboList.Text;
			if (newval == StrFromTss(m_caches.DataAccess.get_MultiStringAlt(m_hvoSbWord, SandboxBase.ktagSbWordForm, m_sandbox.RawWordformWs)))
			{
				return true;
			}
			// Enhance JohnT: consider removing the old WfiWordform, if there are no
			// analyses and no other references.
			return true;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType().Name + " ******");
			base.Dispose(disposing);
		}
	}
}