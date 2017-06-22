﻿// Copyright (c) 2015-2018 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using LanguageExplorer.Controls;
using LanguageExplorer.Controls.PaneBar;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Application;
using SIL.FieldWorks.Filters;
using SIL.FieldWorks.Resources;
using SIL.FieldWorks.XWorks;

namespace LanguageExplorer.Areas.Lexicon.Tools.CollectWords
{
	/// <summary>
	/// ITool implementation for the "rapidDataEntry" tool in the "lexicon" area.
	/// </summary>
	internal sealed class RapidDataEntryTool : ITool
	{
		private CollapsingSplitContainer _collapsingSplitContainer;
		private RecordBrowseView _recordBrowseView;
		private RecordClerk _recordClerk;
		private RecordClerk _nestedRecordClerk;

		#region Implementation of IPropertyTableProvider

		/// <summary>
		/// Placement in the IPropertyTableProvider interface lets FwApp call IPropertyTable.DoStuff.
		/// </summary>
		public IPropertyTable PropertyTable { get; private set; }

		#endregion

		#region Implementation of IPublisherProvider

		/// <summary>
		/// Get the IPublisher.
		/// </summary>
		public IPublisher Publisher { get; private set; }

		#endregion

		#region Implementation of ISubscriberProvider

		/// <summary>
		/// Get the ISubscriber.
		/// </summary>
		public ISubscriber Subscriber { get; private set; }

		#endregion

		#region Implementation of IFlexComponent

		/// <summary>
		/// Initialize a FLEx component with the basic interfaces.
		/// </summary>
		/// <param name="flexComponentParameters">Parameter object that contains the required three interfaces.</param>
		public void InitializeFlexComponent(FlexComponentParameters flexComponentParameters)
		{
			FlexComponentCheckingService.CheckInitializationValues(flexComponentParameters, new FlexComponentParameters(PropertyTable, Publisher, Subscriber));

			PropertyTable = flexComponentParameters.PropertyTable;
			Publisher = flexComponentParameters.Publisher;
			Subscriber = flexComponentParameters.Subscriber;

#if RANDYTODO
			// TODO: Came from Fork commit: "Get all tree-based tools to switch to selected item." 2016-08-12 14:29:42
			// TODO: Wait on other changes from fork that add all the guts to this method.
			var recordBar = new RecordBar(PropertyTable)
			{
				IsFlatList = false,
				Dock = DockStyle.Fill
			};
#endif
		}

		#endregion

		#region Implementation of IMajorFlexComponent

		/// <summary>
		/// Deactivate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the outgoing component, when the user switches to a component.
		/// </remarks>
		public void Deactivate(MajorFlexComponentParameters majorFlexComponentParameters)
		{
			PropertyTable.SetProperty("RecordListWidthGlobal", _collapsingSplitContainer.SplitterDistance, SettingsGroup.GlobalSettings, true, false);

			PropertyTable.RemoveProperty(RecordClerk.ClerkSelectedObjectPropertyId(_nestedRecordClerk.Id));
			PropertyTable.RemoveProperty(RecordClerk.ClerkSelectedObjectPropertyId(_recordClerk.Id));

			PropertyTable.RemoveProperty("ActiveClerkOwningObject");
			PropertyTable.RemoveProperty("ActiveClerkSelectedObject");

			CollapsingSplitContainerFactory.RemoveFromParentAndDispose(
				majorFlexComponentParameters.MainCollapsingSplitContainer,
				ref _collapsingSplitContainer,
				ref _recordClerk);

			_recordBrowseView = null;
			_nestedRecordClerk = null;
		}

		/// <summary>
		/// Activate the component.
		/// </summary>
		/// <remarks>
		/// This is called on the component that is becoming active.
		/// </remarks>
		public void Activate(MajorFlexComponentParameters majorFlexComponentParameters)
		{
			var mainCollapsingSplitContainerAsControl = (Control)majorFlexComponentParameters.MainCollapsingSplitContainer;
			mainCollapsingSplitContainerAsControl.SuspendLayout();

			var doc = XDocument.Parse(LexiconResources.RapidDataEntryToolParameters);
			// The RecordBar uses the "SemanticDomainList" clerk
			var cache = PropertyTable.GetValue<FdoCache>("cache");
			var decorator = new DictionaryPublicationDecorator(cache, cache.ServiceLocator.GetInstance<ISilDataAccessManaged>(), CmPossibilityListTags.kflidPossibilities);
			var recordList = new PossibilityRecordList(decorator, cache.LanguageProject.SemanticDomainListOA);
			var semanticDomainRdeTreeBarHandler = new SemanticDomainRdeTreeBarHandler(PropertyTable, doc.Root.Element("treeBarHandler"), new PaneBar());
			_recordClerk = new RecordClerk("SemanticDomainList", recordList, new PropertyRecordSorter("ShortName"), "Default", null, false, false, semanticDomainRdeTreeBarHandler);
			// The browse view a clerk called "RDEwords" that depends on the main "SemanticDomainList" clerk.
			_recordClerk.InitializeFlexComponent(majorFlexComponentParameters.FlexComponentParameters);
			var recordBar = new RecordBar(PropertyTable)
			{
				IsFlatList = false,
				Dock = DockStyle.Fill
			};
			_collapsingSplitContainer = new CollapsingSplitContainer();
			_collapsingSplitContainer.SuspendLayout();
			_collapsingSplitContainer.SecondCollapseZone = CollapsingSplitContainerFactory.BasicSecondCollapseZoneWidth;
			_collapsingSplitContainer.Dock = DockStyle.Fill;
			_collapsingSplitContainer.Orientation = Orientation.Vertical;
			_collapsingSplitContainer.FirstLabel = AreaResources.ksRecordListLabel;
			_collapsingSplitContainer.FirstControl = recordBar;
			_collapsingSplitContainer.SecondLabel = AreaResources.ksMainContentLabel;
			_collapsingSplitContainer.SplitterDistance = PropertyTable.GetValue<int>("RecordListWidthGlobal", SettingsGroup.GlobalSettings);

			var recordEditViewPaneBar = new PaneBar();
			var panelButton = new PanelButton(PropertyTable, null, PaneBarContainerFactory.CreateShowHiddenFieldsPropertyName(MachineName), LanguageExplorerResources.ksHideFields, LanguageExplorerResources.ksShowHiddenFields)
			{
				Dock = DockStyle.Right
			};
			recordEditViewPaneBar.AddControls(new List<Control> { panelButton });

			var dataTreeMenuHandler = new LexEntryMenuHandler();
			dataTreeMenuHandler.InitializeFlexComponent(majorFlexComponentParameters.FlexComponentParameters);
			var recordEditView = new RecordEditView(doc.Root.Element("recordeditview").Element("parameters"), XDocument.Parse(LexiconResources.BasicFilter), _recordClerk, dataTreeMenuHandler);
			_nestedRecordClerk = new RecordClerk("RDEwords", new RecordList(cache.ServiceLocator.GetInstance<ISilDataAccessManaged>(), true, cache.MetaDataCacheAccessor.GetFieldId2(CmSemanticDomainTags.kClassId, "ReferringSenses", false), cache.LanguageProject.SemanticDomainListOA, "ReferringSenses"), new PropertyRecordSorter("ShortName"), "Default", null, false, false, _recordClerk);
			_nestedRecordClerk.InitializeFlexComponent(majorFlexComponentParameters.FlexComponentParameters);
			_recordBrowseView = new RecordBrowseView(doc.Root.Element("recordbrowseview").Element("parameters"), _nestedRecordClerk);
			var mainMultiPaneParameters = new MultiPaneParameters
			{
				Orientation = Orientation.Horizontal,
				AreaMachineName = AreaMachineName,
				Id = "SemanticCategoryAndItems",
				ToolMachineName = MachineName,
				DefaultFocusControl = "RecordBrowseView",
				FirstControlParameters = new SplitterChildControlParameters { Control = recordEditView, Label = "Semantic Domain" },
				SecondControlParameters = new SplitterChildControlParameters { Control = _recordBrowseView, Label = "Details" }
			};
			var nestedMultiPane = MultiPaneFactory.CreateNestedMultiPane(majorFlexComponentParameters.FlexComponentParameters, mainMultiPaneParameters);
			nestedMultiPane.SplitterDistance = PropertyTable.GetValue<int>(string.Format("MultiPaneSplitterDistance_{0}_{1}_{2}", AreaMachineName, MachineName, mainMultiPaneParameters.Id));
			_collapsingSplitContainer.SecondControl = PaneBarContainerFactory.Create(majorFlexComponentParameters.FlexComponentParameters, recordEditViewPaneBar, nestedMultiPane);
			majorFlexComponentParameters.MainCollapsingSplitContainer.SecondControl = _collapsingSplitContainer;
			_collapsingSplitContainer.ResumeLayout();
			mainCollapsingSplitContainerAsControl.ResumeLayout();
			recordEditView.BringToFront();
			recordBar.BringToFront();

			panelButton.DatTree = recordEditView.DatTree;

			// Too early before now.
			semanticDomainRdeTreeBarHandler.FinishInitialization();
			recordEditView.FinishInitialization();
			majorFlexComponentParameters.DataNavigationManager.Clerk = _recordClerk;
		}

		/// <summary>
		/// Do whatever might be needed to get ready for a refresh.
		/// </summary>
		public void PrepareToRefresh()
		{
			_recordBrowseView.BrowseViewer.BrowseView.PrepareToRefresh();
		}

		/// <summary>
		/// Finish the refresh.
		/// </summary>
		public void FinishRefresh()
		{
			_recordClerk.ReloadIfNeeded();
			((DomainDataByFlidDecoratorBase)_recordClerk.VirtualListPublisher).Refresh();
		}

		/// <summary>
		/// The properties are about to be saved, so make sure they are all current.
		/// Add new ones, as needed.
		/// </summary>
		public void EnsurePropertiesAreCurrent()
		{
			PropertyTable.SetProperty("RecordListWidthGlobal", _collapsingSplitContainer.SplitterDistance, SettingsGroup.GlobalSettings, true, false);
		}

#endregion

#region Implementation of IMajorFlexUiComponent

		/// <summary>
		/// Get the internal name of the component.
		/// </summary>
		/// <remarks>NB: This is the machine friendly name, not the user friendly name.</remarks>
		public string MachineName => "rapidDataEntry";

		/// <summary>
		/// User-visible localizable component name.
		/// </summary>
		public string UiName => "Collect Words";
#endregion

#region Implementation of ITool

		/// <summary>
		/// Get the area machine name the tool is for.
		/// </summary>
		public string AreaMachineName => "lexicon";

		/// <summary>
		/// Get the image for the area.
		/// </summary>
		public Image Icon => Images.BrowseView.SetBackgroundColor(Color.Magenta);

#endregion
	}
}