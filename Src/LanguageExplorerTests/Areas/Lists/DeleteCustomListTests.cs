// Copyright (c) 2010-2020 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Windows.Forms;
using LanguageExplorer.Areas;
using NUnit.Framework;
using SIL.LCModel;
using SIL.LCModel.Core.Cellar;
using SIL.LCModel.Core.Text;

namespace LanguageExplorerTests.Areas.Lists
{
	/// <summary>
	/// Tests for the method object DeleteCustomList.
	/// </summary>
	[TestFixture]
	public class DeleteCustomListTests : DeleteCustomListTestsBase
	{
		#region Setup and Helper Methods

		private FieldDescription CreateCustomAtomicReferenceFieldInLexEntry(Guid listGuid)
		{
			// create new custom field for a LexEntry for testing
			var fd = new FieldDescription(Cache)
			{
				Userlabel = "New Test Custom Field",
				HelpString = string.Empty,
				Class = LexEntryTags.kClassId,
				Type = CellarPropertyType.ReferenceAtomic,
				WsSelector = 0,
				DstCls = CmPossibilityTags.kClassId,
				ListRootId = listGuid
			};
			fd.UpdateCustomField();
			return fd;
		}

		private FieldDescription CreateCustomAtomicReferenceFieldToCustomListInLexEntry(Guid listGuid)
		{
			// create new custom field for a LexEntry for testing
			var fd = new FieldDescription(Cache)
			{
				Userlabel = "New Test Custom Field",
				HelpString = string.Empty,
				Class = LexEntryTags.kClassId,
				Type = CellarPropertyType.ReferenceAtomic,
				WsSelector = 0,
				DstCls = CmPossibilityTags.kClassId,
				ListRootId = listGuid
			};
			fd.UpdateCustomField();
			return fd;
		}

		private FieldDescription CreateCustomMultipleRefFieldInLexSense(Guid listGuid)
		{
			// create new custom field for a LexSense for testing
			var fd = new FieldDescription(Cache)
			{
				Userlabel = "New Test Custom Field",
				HelpString = string.Empty,
				Class = LexSenseTags.kClassId,
				Type = CellarPropertyType.ReferenceCollection,
				WsSelector = 0,
				DstCls = CmPossibilityTags.kClassId,
				ListRootId = listGuid
			};
			fd.UpdateCustomField();
			return fd;
		}

		private void DeleteCustomField(FieldDescription fd)
		{
			// delete custom field that was added to a LexEntry for testing
			if (fd.IsCustomField && fd.IsInstalled)
			{
				fd.MarkForDeletion = true;
				AreaServices.UpdateCachedObjects(Cache, fd);
				Cache.ActionHandlerAccessor.BeginUndoTask("UndoUpdateCustomField", "RedoUpdateCustomField");
				fd.UpdateCustomField();
				Cache.ActionHandlerAccessor.EndUndoTask();
			}
			FieldDescription.ClearDataAbout();
		}

		private ILexEntry CreateLexicalEntry(ILexDb lexDb)
		{
			return Cache.ServiceLocator.GetInstance<ILexEntryFactory>().Create(new Guid(), lexDb);
		}

		private ILexSense CreateLexicalEntryWithSense(ILexDb lexDb)
		{
			var entry = Cache.ServiceLocator.GetInstance<ILexEntryFactory>().Create(new Guid(), lexDb);
			return Cache.ServiceLocator.GetInstance<ILexSenseFactory>().Create(new Guid(), entry);
		}

		#endregion

		/// <summary>
		/// Tests deleting a custom list which has no possibilities.
		/// </summary>
		[Test]
		public void DeleteCustomList_NoPossibilities()
		{
			// Setup
			var clists = m_listRepo.Count; // with custom list added in CreateTestData()

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists - 1, m_listRepo.Count, "List should have been deleted.");
		}

		/// <summary>
		/// Tests trying to delete a regular (non-Custom) list.
		/// </summary>
		[Test]
		public void DeleteCustomList_NotCustom()
		{
			// Setup
			var clists = m_listRepo.Count;
			var annDefList = Cache.LangProject.AnnotationDefsOA;

			// SUT
			m_helper.Run(annDefList);

			// Verify
			Assert.AreEqual(clists, m_listRepo.Count, "Should not delete an owned list.");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests trying to delete a Custom list with one Possibility, but with nothing
		/// referencing it.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void DeleteCustomList_OnePossibilityNoReference()
		{
			// Setup
			var clists = m_listRepo.Count;
			var newPossHvo = m_possFact.Create(Guid.NewGuid(), m_testList).Hvo;
			m_helper.ExpectedTestResponse = DialogResult.No;

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists - 1, m_listRepo.Count, "Possibility not referenced by anything. Should just delete the list.");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests trying to delete a Custom list with one Possibility referenced by something
		/// else. 'User' responds to dialog with 'No'.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void DeleteCustomList_OnePossibilityRef_No()
		{
			// Setup
			const string newPossName = "Test Possibility";
			var clists = m_listRepo.Count;
			var newPoss = m_possFact.Create(Guid.NewGuid(), m_testList);
			newPoss.Name.set_String(m_userWs, TsStringUtils.MakeString(newPossName, m_userWs));
			m_helper.ExpectedTestResponse = DialogResult.No;

			// Create a reference to the possibility
			Cache.LangProject.AnnotationDefsOA.PossibilitiesOS[0].ConfidenceRA = newPoss;

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists, m_listRepo.Count,
				"'User' responded 'No'. Should not delete the list.");
			Assert.AreEqual(newPossName, m_helper.PossNameInDlg,
				"Name of possibility found is not the one we put in there!");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests trying to delete a Custom list with one Possibility referenced by something
		/// else. 'User' responds to dialog with 'Yes'.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void DeleteCustomList_OnePossibilityRef_Yes()
		{
			// Setup
			const string newPossName = "Test Possibility";
			var clists = m_listRepo.Count;
			var newPoss = m_possFact.Create(Guid.NewGuid(), m_testList);
			newPoss.Name.set_String(m_userWs, TsStringUtils.MakeString(newPossName, m_userWs));
			m_helper.ExpectedTestResponse = DialogResult.Yes;

			// Create a reference to the possibility
			Cache.LangProject.AnnotationDefsOA.PossibilitiesOS[0].ConfidenceRA = newPoss;

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists - 1, m_listRepo.Count,
				"'User' responded 'Yes'. Should have deleted the list.");
			Assert.AreEqual(newPossName, m_helper.PossNameInDlg,
				"Name of possibility found is not the one we put in there!");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests trying to delete a Custom list with one Possibility referenced by a Custom field.
		/// 'User' responds to dialog with 'Yes'.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void DeleteCustomList_OnePossibilityReferencingCustomField_Yes()
		{
			// Setup
			const string newPossName = "Test Possibility";
			var clists = m_listRepo.Count;
			var cfields = Cache.MetaDataCacheAccessor.FieldCount;
			var newPoss = m_possFact.Create(Guid.NewGuid(), m_testList);
			newPoss.Name.set_String(m_userWs, TsStringUtils.MakeString(newPossName, m_userWs));
			m_helper.ExpectedTestResponse = DialogResult.Yes;

			// Create a custom field in LexEntry
			var fd = CreateCustomAtomicReferenceFieldInLexEntry(m_testList.Guid);
			// Create a lexical entry
			var lexEntry = CreateLexicalEntry(Cache.LangProject.LexDbOA);
			// Create a reference to the possibility
			Cache.DomainDataByFlid.SetObjProp(lexEntry.Hvo, fd.Id, newPoss.Hvo);

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists - 1, m_listRepo.Count,
				"'User' responded 'Yes'. Should have deleted the list.");
			Assert.AreEqual(cfields, Cache.MetaDataCacheAccessor.FieldCount,
				"Custom Field should get deleted.");
			Assert.AreEqual(newPossName, m_helper.PossNameInDlg,
				"Name of possibility found is not the one we put in there!");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests trying to delete a Custom list with one Possibility referenced by a Custom field.
		/// The Custom field is in Entry and is of type Reference Atomic.
		/// 'User' responds to dialog with 'Yes'.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void DeleteCustomList_OnePossibilityReferencingDeletedCustomField_Yes()
		{
			// Setup
			const string newPossName = "Test Possibility";
			var clists = m_listRepo.Count;
			var cfields = Cache.MetaDataCacheAccessor.FieldCount;
			var newPoss = CreateCustomItemAddToList(m_testList, newPossName);
			m_helper.ExpectedTestResponse = DialogResult.Yes;

			// Create a custom field in LexEntry
			var fd = CreateCustomAtomicReferenceFieldToCustomListInLexEntry(m_testList.Guid);
			// Create a lexical entry
			var lexEntry = CreateLexicalEntry(Cache.LangProject.LexDbOA);
			// Create a reference to the possibility
			Cache.DomainDataByFlid.SetObjProp(lexEntry.Hvo, fd.Id, newPoss.Hvo);
			Cache.ActionHandlerAccessor.EndUndoTask();
			// The delete custom field has its own task
			// Delete the custom field
			DeleteCustomField(fd);
			// apparently the delete field and delete list need to be separate tasks
			// in order to make the bug appear (LT-12251)
			Cache.ActionHandlerAccessor.BeginUndoTask("UndoDeleteList", "RedoDeleteList");
			Assert.AreEqual(cfields, Cache.MetaDataCacheAccessor.FieldCount,
				"Custom Field should have been deleted.");

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists - 1, m_listRepo.Count,
				"'User' responded 'Yes'. Should have deleted the list.");
			Assert.AreEqual(String.Empty, m_helper.PossNameInDlg,
				"This test shouldn't go through the dialog.");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests trying to delete a Custom list with one Possibility referenced by a Custom field.
		/// The Custom field is in Sense and is of type Reference Collection.
		/// 'User' responds to dialog with 'Yes'.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void DeleteCustomList_OnePossibilityReferencingDeletedCustomField_RefMultipleField_Yes()
		{
			// Setup
			const string newPossName = "Test Possibility";
			var clists = m_listRepo.Count;
			var cfields = Cache.MetaDataCacheAccessor.FieldCount;
			var newPoss = CreateCustomItemAddToList(m_testList, newPossName);
			//m_helper.ExpectedTestResponse = DialogResult.Yes; // Doesn't go through the dialog

			// Create a custom field in LexSense
			var fd = CreateCustomMultipleRefFieldInLexSense(m_testList.Guid);
			// Create a lexical entry
			var lexSense = CreateLexicalEntryWithSense(Cache.LangProject.LexDbOA);
			// Create a reference to the possibility
			Cache.DomainDataByFlid.Replace(lexSense.Hvo, fd.Id, 0, 0, new [] { newPoss.Hvo }, 1);
			Cache.ActionHandlerAccessor.EndUndoTask();
			// The delete custom field has its own task
			// Delete the custom field
			DeleteCustomField(fd);
			// apparently the delete field and delete list need to be separate tasks
			// in order to make the bug appear (LT-12251)
			Cache.ActionHandlerAccessor.BeginUndoTask("UndoDeleteList", "RedoDeleteList");
			Assert.AreEqual(cfields, Cache.MetaDataCacheAccessor.FieldCount,
				"Custom Field should have been deleted.");

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists - 1, m_listRepo.Count,
				"'User' responded 'Yes'. Should have deleted the list.");
			Assert.AreEqual(String.Empty, m_helper.PossNameInDlg,
				"This test shouldn't go through the dialog.");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests trying to delete a Custom list with one Possibility referenced by a Custom field.
		/// 'User' responds to dialog with 'No'.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void DeleteCustomList_OnePossibilityReferencingCustomField_No()
		{
			// Setup
			const string newPossName = "Test Possibility";
			var clists = m_listRepo.Count;
			var cfields = Cache.MetaDataCacheAccessor.FieldCount;
			var newPoss = m_possFact.Create(Guid.NewGuid(), m_testList);
			newPoss.Name.set_String(m_userWs, TsStringUtils.MakeString(newPossName, m_userWs));
			m_helper.ExpectedTestResponse = DialogResult.No;

			// Create a custom field in LexEntry
			var fd = CreateCustomAtomicReferenceFieldInLexEntry(m_testList.Guid);
			// Create a lexical entry
			var lexEntry = CreateLexicalEntry(Cache.LangProject.LexDbOA);
			// Create a reference to the possibility
			Cache.DomainDataByFlid.SetObjProp(lexEntry.Hvo, fd.Id, newPoss.Hvo);

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists, m_listRepo.Count,
				"'User' responded 'No'. Should not have deleted the list.");
			Assert.AreEqual(cfields + 1, Cache.MetaDataCacheAccessor.FieldCount,
				"Custom Field should not get deleted.");
			Assert.AreEqual(newPossName, m_helper.PossNameInDlg,
				"Name of possibility found is not the one we put in there!");
			// Remove field from mdc so it doesn't mess up other tests!
			fd.MarkForDeletion = true;
			fd.UpdateCustomField();
			FieldDescription.ClearDataAbout();
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests trying to delete a Custom list with several Possibilities referenced by something
		/// else. 'User' responds to dialog with 'Yes'.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void DeleteCustomList_MultiPossibilityRef_Yes()
		{
			// Setup
			const string newPossName = "Test Possibility";
			var clists = m_listRepo.Count;
			var newPoss1 = m_possFact.Create(Guid.NewGuid(), m_testList);
			newPoss1.Name.set_String(m_userWs, TsStringUtils.MakeString(newPossName + "1", m_userWs));
			var newPoss2 = m_possFact.Create(Guid.NewGuid(), m_testList);
			newPoss2.Name.set_String(m_userWs, TsStringUtils.MakeString(newPossName + "2", m_userWs));
			var newPoss3 = m_possFact.Create(Guid.NewGuid(), m_testList);
			newPoss3.Name.set_String(m_userWs, TsStringUtils.MakeString(newPossName + "3", m_userWs));
			m_helper.ExpectedTestResponse = DialogResult.Yes;

			// Create references to the possibilities
			Cache.LangProject.AnnotationDefsOA.PossibilitiesOS[0].ConfidenceRA = newPoss1;
			Cache.LangProject.AnnotationDefsOA.PossibilitiesOS[1].ConfidenceRA = newPoss2;
			Cache.LangProject.AnnotationDefsOA.PossibilitiesOS[2].ConfidenceRA = newPoss3;

			// SUT
			m_helper.Run(m_testList);

			// Verify
			Assert.AreEqual(clists - 1, m_listRepo.Count,
				"'User' responded 'Yes'. Should have deleted the list.");
			Assert.AreEqual(newPossName + "1", m_helper.PossNameInDlg,
				"Name of possibility found is not the one we put in there!");
		}
	}
}