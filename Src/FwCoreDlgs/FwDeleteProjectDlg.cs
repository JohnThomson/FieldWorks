// Copyright (c) 2004-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Resources;
using SIL.LCModel;

namespace SIL.FieldWorks.FwCoreDlgs
{
	/// <summary />
	public class FwDeleteProjectDlg : Form
	{
		private ListBox m_lstProjects;
		private ListBox m_lstProjectsInUse;
		private IHelpTopicProvider m_helpTopicProvider;
		private Button m_btnDelete;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary />
		public FwDeleteProjectDlg(ICollection<string> projectsOpen)
		{
			AccessibleName = GetType().Name;
			InitializeComponent();

			// Get information on all the projects in the current server.
			var projectList = GetLocalProjects(projectsOpen);
			// Fill in the list controls
			m_lstProjects.Items.Clear();
			m_lstProjectsInUse.Items.Clear();
			foreach (var info in projectList)
			{
				if (info.InUse)
				{
					m_lstProjectsInUse.Items.Add(info);
				}
				else
				{
					m_lstProjects.Items.Add(info);
				}
			}
		}

		private static IEnumerable<ProjectInfo> GetLocalProjects(ICollection<string> projectsOpen)
		{
			// ProjectInfo.AllProjects doesn't set the InUse flag, which is why we
			// pass a list of open projects to the dialog constructor.
			var projectList = ProjectInfo.GetAllProjects(FwDirectoryFinder.ProjectsDirectory);
			foreach (var info in projectList.Where(info => projectsOpen.Contains(info.DatabaseName)))
			{
				info.InUse = true;
			}
			return projectList;
		}

		/// <summary>
		/// Set the dialog properties object for dialogs that are created.
		/// </summary>
		public void SetDialogProperties(IHelpTopicProvider helpTopicProvider)
		{
			m_helpTopicProvider = helpTopicProvider;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType().Name + ". ****** ");
			if (IsDisposed)
			{
				// No need to run it more than once.
				return;
			}

			if (disposing)
			{
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.Button m_btnExit;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FwDeleteProjectDlg));
			System.Windows.Forms.Button m_btnHelp;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.HelpProvider m_helpProvider;
			this.m_lstProjects = new System.Windows.Forms.ListBox();
			this.m_btnDelete = new System.Windows.Forms.Button();
			this.m_lstProjectsInUse = new System.Windows.Forms.ListBox();
			m_btnExit = new System.Windows.Forms.Button();
			m_btnHelp = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			m_helpProvider = new System.Windows.Forms.HelpProvider();
			this.SuspendLayout();
			//
			// m_btnExit
			//
			resources.ApplyResources(m_btnExit, "m_btnExit");
			m_btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			m_btnExit.Name = "m_btnExit";
			m_helpProvider.SetShowHelp(m_btnExit, ((bool)(resources.GetObject("m_btnExit.ShowHelp"))));
			//
			// m_btnHelp
			//
			resources.ApplyResources(m_btnHelp, "m_btnHelp");
			m_helpProvider.SetHelpString(m_btnHelp, resources.GetString("m_btnHelp.HelpString"));
			m_btnHelp.Name = "m_btnHelp";
			m_helpProvider.SetShowHelp(m_btnHelp, ((bool)(resources.GetObject("m_btnHelp.ShowHelp"))));
			m_btnHelp.Click += new System.EventHandler(this.m_btnHelp_Click);
			//
			// label1
			//
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			m_helpProvider.SetShowHelp(label1, ((bool)(resources.GetObject("label1.ShowHelp"))));
			//
			// label2
			//
			resources.ApplyResources(label2, "label2");
			label2.Name = "label2";
			m_helpProvider.SetShowHelp(label2, ((bool)(resources.GetObject("label2.ShowHelp"))));
			//
			// m_lstProjects
			//
			resources.ApplyResources(this.m_lstProjects, "m_lstProjects");
			this.m_lstProjects.Name = "m_lstProjects";
			this.m_lstProjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			m_helpProvider.SetShowHelp(this.m_lstProjects, ((bool)(resources.GetObject("m_lstProjects.ShowHelp"))));
			this.m_lstProjects.Sorted = true;
			this.m_lstProjects.SelectedIndexChanged += new System.EventHandler(this.m_lstProjects_SelectedIndexChanged);
			//
			// m_btnDelete
			//
			resources.ApplyResources(this.m_btnDelete, "m_btnDelete");
			this.m_btnDelete.Name = "m_btnDelete";
			m_helpProvider.SetShowHelp(this.m_btnDelete, ((bool)(resources.GetObject("m_btnDelete.ShowHelp"))));
			this.m_btnDelete.Click += new System.EventHandler(this.m_btnDelete_Click);
			//
			// m_lstProjectsInUse
			//
			resources.ApplyResources(this.m_lstProjectsInUse, "m_lstProjectsInUse");
			this.m_lstProjectsInUse.Name = "m_lstProjectsInUse";
			this.m_lstProjectsInUse.SelectionMode = System.Windows.Forms.SelectionMode.None;
			m_helpProvider.SetShowHelp(this.m_lstProjectsInUse, ((bool)(resources.GetObject("m_lstProjectsInUse.ShowHelp"))));
			this.m_lstProjectsInUse.Sorted = true;
			//
			// FwDeleteProjectDlg
			//
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = m_btnExit;
			this.Controls.Add(label2);
			this.Controls.Add(label1);
			this.Controls.Add(this.m_lstProjectsInUse);
			this.Controls.Add(m_btnHelp);
			this.Controls.Add(m_btnExit);
			this.Controls.Add(this.m_btnDelete);
			this.Controls.Add(this.m_lstProjects);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FwDeleteProjectDlg";
			m_helpProvider.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary />
		private void m_lstProjects_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			m_btnDelete.Enabled = (((ListBox)sender).SelectedItems.Count > 0);
		}

		/// <summary>
		/// Display help for this dialog box.
		/// </summary>
		private void m_btnHelp_Click(object sender, System.EventArgs e)
		{
			ShowHelp.ShowHelpTopic(m_helpTopicProvider, "khtpDeleteProj");
		}

		/// <summary>
		/// Handle a click on the Delete button - delete a project.
		/// </summary>
		private void m_btnDelete_Click(object sender, System.EventArgs e)
		{
			if (m_lstProjects.SelectedItems.Count == 0)
			{
				return;
			}
			// Make a copy of the selected items so we can iterate through them, deleting
			// from the list box and maintaining the integrity of the collection being iterated.
			var itemsToDelete = new List<ProjectInfo>();
			foreach (ProjectInfo info in m_lstProjects.SelectedItems)
			{
				itemsToDelete.Add(info);
			}
			foreach (var info in itemsToDelete)
			{
				var folder = Path.Combine(FwDirectoryFinder.ProjectsDirectory, info.DatabaseName);
				var fExtraData = CheckForExtraData(info, folder);
				string msg;
				MessageBoxButtons buttons;
				if (fExtraData)
				{
					msg = ResourceHelper.FormatResourceString("kstidDeleteProjFolder", info.DatabaseName);
					buttons = MessageBoxButtons.YesNoCancel;
				}
				else
				{
					msg = ResourceHelper.FormatResourceString("kstidConfirmDeleteProject", info.DatabaseName);
					buttons = MessageBoxButtons.OKCancel;
				}
				var result = MessageBox.Show(msg, ResourceHelper.GetResourceString("kstidDeleteProjCaption"), buttons);
				if (result == DialogResult.Cancel)
				{
					continue;
				}
				try
				{
					if (result == DialogResult.Yes || result == DialogResult.OK)
					{
						Directory.Delete(folder, true);
					}
					else
					{
						var path = Path.Combine(folder, info.DatabaseName + LcmFileHelper.ksFwDataXmlFileExtension);
						if (File.Exists(path))
						{
							File.Delete(path);
						}
						path = Path.ChangeExtension(path, LcmFileHelper.ksFwDataFallbackFileExtension);
						if (File.Exists(path))
						{
							File.Delete(path);
						}
						path = Path.Combine(folder, LcmFileHelper.ksWritingSystemsDir);
						if (Directory.Exists(path))
						{
							Directory.Delete(path, true);
						}
						path = Path.Combine(folder, LcmFileHelper.ksBackupSettingsDir);
						if (Directory.Exists(path))
						{
							Directory.Delete(path, true);
						}
						path = Path.Combine(folder, LcmFileHelper.ksConfigurationSettingsDir);
						if (Directory.Exists(path))
						{
							Directory.Delete(path, true);
						}
						path = Path.Combine(folder, LcmFileHelper.ksSortSequenceTempDir);
						if (Directory.Exists(path))
						{
							Directory.Delete(path, true);
						}
						var folders = Directory.GetDirectories(folder);
						foreach (var dir in folders)
						{
							if (!FolderContainsFiles(dir))
							{
								Directory.Delete(dir, true);
							}
						}
					}
				}
				catch
				{
					MessageBox.Show(this, string.Format(ResourceHelper.GetResourceString("kstidDeleteProjError"), info.DatabaseName),
						ResourceHelper.GetResourceString("kstidDeleteProjCaption"), MessageBoxButtons.OK);
				}
				m_lstProjects.Items.Remove(info);
			}
		}

		private static bool CheckForExtraData(ProjectInfo info, string folder)
		{
			var folders = Directory.GetDirectories(folder);
			foreach (var dir in folders)
			{
				var name = Path.GetFileName(dir);
				if (name == LcmFileHelper.ksWritingSystemsDir ||
					name == LcmFileHelper.ksBackupSettingsDir ||
					name == LcmFileHelper.ksConfigurationSettingsDir ||
					name == LcmFileHelper.ksSortSequenceTempDir)
				{
					continue;
				}
				if (FolderContainsFiles(dir))
				{
					return true;
				}
			}
			var files = Directory.GetFiles(folder);
			if (files.Length > 3)
			{
				return true;
			}
			foreach (var filepath in files)
			{
				var file = Path.GetFileName(filepath);
				if (file != info.DatabaseName + LcmFileHelper.ksFwDataXmlFileExtension && file != info.DatabaseName + LcmFileHelper.ksFwDataFallbackFileExtension)
				{
					return true;
				}
			}
			return false;
		}

		private static bool FolderContainsFiles(string folder)
		{
			var files = Directory.GetFiles(folder);
			if (files.Length > 0)
			{
				return true;
			}
			var folders = Directory.GetDirectories(folder);
			foreach (var dir in folders)
			{
				if (FolderContainsFiles(dir))
				{
					return true;
				}
			}
			return false;
		}
	}
}