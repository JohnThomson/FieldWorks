// Copyright (c) 2007-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using SIL.LCModel.Core.Text;
using SIL.FieldWorks.Common.FwUtils;
using SIL.LCModel;
using SIL.FieldWorks.Resources;
using SIL.LCModel.DomainServices;

namespace SIL.FieldWorks.FwCoreDlgs.Controls
{
	/// <summary />
	public partial class FwGeneralTab : UserControl, IStylesTab
	{
		#region Member variables

		private bool m_owningDialogCanceled;
		#endregion

		#region Constructor
		/// <summary />
		public FwGeneralTab()
		{
			InitializeComponent();
		}
		#endregion

		#region IStylesTab Members

		/// <summary>
		/// Saves the information on the tab to the specified style info.
		/// </summary>
		public void SaveToInfo(StyleInfo styleInfo)
		{
			// save the changes from the general tab
			// NOTE: The name has to be set last as ChangeStyleName can update the basedOn
			// and Following styles for styleInfo to its correct values.
			styleInfo.SaveBasedOn(m_cboBasedOn.Text);
			styleInfo.SaveFollowing(m_cboFollowingStyle.Text);
			if (m_txtStyleName.Text != styleInfo.Name)
			{
				ChangeStyleName(styleInfo);
			}
			styleInfo.SaveDescription(m_txtStyleUsage.Text);
		}

		/// <summary>
		/// Updates the information on the tab with the information in the specified style info.
		/// </summary>
		public void UpdateForStyle(StyleInfo styleInfo)
		{
			if (styleInfo == null)
			{
				FillForDefaultParagraphCharacters();
				Enabled = false;
				return;
			}
			Enabled = true;

			m_txtStyleName.Enabled = !styleInfo.IsBuiltIn;
			m_txtStyleUsage.ReadOnly = styleInfo.IsBuiltIn;
			m_cboBasedOn.Enabled = !styleInfo.IsBuiltIn;
			m_cboFollowingStyle.Enabled = !styleInfo.IsBuiltIn;
			m_txtShortcut.Enabled = false;
			m_txtStyleName.Text = styleInfo.Name;
			m_lblStyleType.Text = (styleInfo.IsCharacterStyle) ? Strings.kstidCharacterStyleText : Strings.kstidParagraphStyleText;
			m_txtStyleUsage.Text = styleInfo.Usage;
			m_lblStyleDescription.Text = styleInfo.ToString(ShowBiDiLabels, UserMeasurementType);

			// Handle the Based On style combo
			FillBasedOnStyles(styleInfo);
			if (styleInfo.BasedOnStyle != null)
			{
				m_cboBasedOn.SelectedItem = styleInfo.BasedOnStyle.Name;
			}
			else
			{
				if (styleInfo.IsCharacterStyle)
				{
					m_cboBasedOn.SelectedIndex = 0; // "default paragraph characters"
				}
				else
				{
					m_cboBasedOn.SelectedIndex = -1;
				}
			}

			UpdateFollowingStylesCbo(styleInfo);
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the style as typed into the style name text box.
		/// </summary>
		public string StyleName => m_txtStyleName.Text;

		/// <summary>
		/// Gets or sets the style list helper.
		/// </summary>
		public StyleListBoxHelper StyleListHelper { get; set; }

		/// <summary>
		/// Gets or sets the style table.
		/// </summary>
		public StyleInfoTable StyleTable { get; set; }

		/// <summary>
		/// Indicates whether to show labels that are meaningful for both left-to-right and
		/// right-to-left. If this value is false, then simple "Left" and "Right" labels will be
		/// used in the display, rather than "Leading" and "Trailing".
		/// </summary>
		public bool ShowBiDiLabels { get; set; }

		/// <summary>
		/// Gets or sets the type of the user measurement.
		/// </summary>
		public MsrSysType UserMeasurementType { get; set; }

		/// <summary>
		/// Sets the renamed styles collection
		/// </summary>
		public Dictionary<string, string> RenamedStyles { get; set; }

		/// <summary>
		/// Sets the application.
		/// </summary>
		public IApp Application { get; set; }
		#endregion

		#region Overrides

		/// <inheritdoc />
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			m_txtStyleName.Focus();
			m_txtStyleName.SelectAll();
		}
		#endregion

		#region General tab handling
		/// <summary>
		/// Sets up the dialog to have default paragraph characters selected
		/// </summary>
		private void FillForDefaultParagraphCharacters()
		{
			m_txtStyleName.Text = StyleUtils.DefaultParaCharsStyleName;
			m_txtStyleName.Enabled = false;
			m_txtStyleUsage.Text = Strings.kstidDefaultParaCharsUsage;
			m_txtStyleUsage.ReadOnly = true;
			m_lblStyleType.Text = Strings.kstidCharacterStyleText;
			m_cboBasedOn.SelectedIndex = -1;
			m_cboBasedOn.Enabled = false;
			m_cboFollowingStyle.SelectedIndex = -1;
			m_cboFollowingStyle.Enabled = false;
			m_lblStyleDescription.Text = string.Empty;
		}

		/// <summary>
		/// Fills the based on styles combo for a specific style
		/// </summary>
		private void FillBasedOnStyles(BaseStyleInfo styleInfo)
		{
			m_cboBasedOn.Items.Clear();

			// If this is a character style then put in "Default Paragraph Characters"
			if (styleInfo.IsCharacterStyle)
			{
				m_cboBasedOn.Items.Add(StyleUtils.DefaultParaCharsStyleName);
			}

			// Add all of the styles that are not myself or any style that derives from me and
			// have the same context as me
			var styleList = new List<string>();
			foreach (var baseStyle in StyleTable.Values)
			{
				// If the style types are not the same, then do not allow them.
				if (baseStyle.IsCharacterStyle != styleInfo.IsCharacterStyle)
				{
					continue;
				}
				// TE-6344: If styleInfo is already based on baseStyle, then we must include baseStyle
				// in the list, even if it is not normally a style that can be a based-on
				// style. This allows a style with a context of internal (such as "Normal" in
				// TE) to appear in the list when it is the basis for a built-in or copied style.
				if (styleInfo.BasedOnStyle == baseStyle)
				{
					Debug.Assert(!DerivesFromOrSame(baseStyle, styleInfo)); // Sanity check for circular reference
					styleList.Add(baseStyle.Name);
				}
				else if (!DerivesFromOrSame(baseStyle, styleInfo) && baseStyle.CanInheritFrom && StylesCanBeABaseFor(baseStyle, styleInfo))
				{
					styleList.Add(baseStyle.Name);
				}
			}
			styleList.Sort();
			m_cboBasedOn.Items.AddRange(styleList.ToArray());
		}

		/// <summary>
		/// Fills the following styles combo for a specific style
		/// </summary>
		private void FillFollowingStyles(BaseStyleInfo styleInfo)
		{
			m_cboFollowingStyle.Items.Clear();

			// Add all of the styles of the same type
			var styleList = new List<string>();
			foreach (var style in StyleTable.Values)
			{
				// If the style types are not the same, then do not allow them.
				if (style.IsCharacterStyle != styleInfo.IsCharacterStyle)
				{
					continue;
				}
				// TE-6346: Add this style to the list if it's already the following style for the
				// given styleInfo, even if it's an internal style because internal styles can have
				// themselves as their own following style.
				if (styleInfo.NextStyle == style || !style.IsInternalStyle)
				{
					styleList.Add(style.Name);
				}
			}
			styleList.Sort();
			m_cboFollowingStyle.Items.AddRange(styleList.ToArray());
		}

		/// <summary>
		/// Updates the following styles combo box.
		/// </summary>
		private void UpdateFollowingStylesCbo(StyleInfo styleInfo)
		{
			// Handle the Following Paragraph Style combo box
			if (styleInfo.IsCharacterStyle)
			{
				m_cboFollowingStyle.Items.Clear();
				m_cboFollowingStyle.Enabled = false;
			}
			else
			{
				FillFollowingStyles(styleInfo);
				if (styleInfo.NextStyle == null)
				{
					m_cboFollowingStyle.SelectedIndex = -1;
				}
				else
				{
					m_cboFollowingStyle.SelectedItem = styleInfo.NextStyle.Name;
				}
			}
		}
		#endregion

		#region Renaming style
		/// <summary>
		/// Handles the Validating event of the m_txtStyleName control.
		/// </summary>
		private void m_txtStyleName_Validating(object sender, CancelEventArgs e)
		{
			// If the user pressed cancel, this method gets called before the owning
			// form will process the cancel which is not good if this validation
			// fails because it means the user cannot cancel the changes that caused
			// the validation failure. Therefore, do our best to determine whether
			// or not the user is here as a result of losing focus to the cancel button.
			// If so, don't bother validating but set a flag indicating the user is
			// cancelling out of the dialog so the Validated event doesn't try to save
			// the invalid change, otherwise the program will crash.
			var owningForm = FindForm();
			if (owningForm != null && owningForm.ActiveControl == owningForm.CancelButton)
			{
				m_owningDialogCanceled = true;
				return;
			}

			m_txtStyleName.Text = m_txtStyleName.Text.Trim();

			if (StyleListHelper.SelectedStyleName == m_txtStyleName.Text)
			{
				return;
			}
			if (StyleTable.ContainsKey(m_txtStyleName.Text))
			{
				e.Cancel = true;
				MessageBox.Show(this, string.Format(ResourceHelper.GetResourceString("kstidDuplicateStyleError"), m_txtStyleName.Text), Application.ApplicationName);
			}
			else if (m_txtStyleName.Text.Equals(string.Empty))
			{
				e.Cancel = true;
				MessageBox.Show(this, string.Format(ResourceHelper.GetResourceString("kstidBlankStyleNameError"), m_txtStyleName.Text), Application.ApplicationName);

				// set style name from duplicate back to name in style list box (default)
				m_txtStyleName.Text = StyleListHelper.SelectedStyleName;
			}
		}

		/// <summary>
		/// Handles the Validated event of the m_txtStyleName control.
		/// </summary>
		private void m_txtStyleName_Validated(object sender, EventArgs e)
		{
			if (m_owningDialogCanceled)
			{
				return;
			}

			if (StyleListHelper.SelectedStyle != null)
			{
				var styleInfo = (StyleInfo)StyleListHelper.SelectedStyle.StyleInfo;
				if (styleInfo == null || m_txtStyleName.Text == styleInfo.Name)
				{
					// We DON'T want to go on to try to re-select this style in the list
					// because if there's another style that differs only by case, we might find
					// the wrong one and accidentally think we're renaming this style.
					return;
				}

				SaveToInfo(styleInfo);
				UpdateFollowingStylesCbo(styleInfo);
			}
			StyleListHelper.SelectedStyleName = m_txtStyleName.Text;
		}

		/// <summary>
		/// Changes the name of the style
		/// </summary>
		private void ChangeStyleName(StyleInfo styleInfo)
		{
			var newName = m_txtStyleName.Text;
			var oldName = styleInfo.Name;
			// fix any styles that refer to this one
			foreach (StyleInfo updateStyle in StyleTable.Values)
			{
				if (updateStyle.BasedOnStyle != null && updateStyle.BasedOnStyle.Name == oldName)
				{
					updateStyle.SaveBasedOn(newName);
				}
				if (updateStyle.NextStyle != null && updateStyle.NextStyle.Name == oldName)
				{
					updateStyle.SaveFollowing(newName);
				}
			}

			// save the new name and update the entry in the style table
			styleInfo.SaveName(newName);
			StyleTable.Remove(oldName);
			StyleTable.Add(newName, styleInfo);

			// Change the displayed entry
			StyleListHelper.Rename(oldName, newName);

			// Save an entry to rename the style if it is a real style
			if (styleInfo.RealStyle != null)
			{
				SaveRenamedStyle(oldName, newName);
			}
		}
		#endregion

		/// <summary>
		/// Determines whether or not the specified base style can be used as a base for the
		/// specified style
		/// </summary>
		private static bool StylesCanBeABaseFor(BaseStyleInfo baseStyle, BaseStyleInfo styleInfo)
		{
			// If the style is not in the DB yet, then we want to allow any style to be a base
			// so the user can select something
			if (styleInfo.RealStyle == null)
			{
				return true;
			}
			// Styles can always be based on general styles
			if (baseStyle.Context == ContextValues.General)
			{
				return true;
			}
			// If the base style is actually the base style of the style, then show it in the
			// list
			if (styleInfo.BasedOnStyle == baseStyle)
			{
				return true;
			}
			// Otherwise, the context, structure and function of the style must match for a
			// style to be based on it.
			return baseStyle.Context == styleInfo.Context && baseStyle.Structure == styleInfo.Structure && baseStyle.Function == styleInfo.Function;
		}

		/// <summary>
		/// Determines if style1 derives from style2 or if the styles are the same
		/// </summary>
		private static bool DerivesFromOrSame(BaseStyleInfo style1, BaseStyleInfo style2)
		{
			while (style1 != null)
			{
				if (style2.Name == style1.Name)
				{
					return true;
				}
				style1 = style1.BasedOnStyle;
			}
			return false;
		}

		/// <summary>
		/// Saves the renamed style information
		/// </summary>
		protected void SaveRenamedStyle(string oldName, string newName)
		{
			// Save the style name change in a list so the change can be applied to the
			// database later. If the style has already been renamed, then just replace the
			// existing entry. This list is keyed on the new name with the old name as the value.
			string originalName;
			if (RenamedStyles.TryGetValue(oldName, out originalName))
			{
				RenamedStyles.Remove(oldName);
				if (originalName != newName)
				{
					RenamedStyles[newName] = originalName;
				}
			}
			else
			{
				RenamedStyles.Add(newName, oldName);
			}
		}
	}
}