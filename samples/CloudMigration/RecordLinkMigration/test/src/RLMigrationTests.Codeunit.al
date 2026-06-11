// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration.Tests;

using MSFT.DataMigration;
using Microsoft.DataMigration;
using System.Environment.Configuration;
using System.TestLibraries.Utilities;

codeunit 57550 "RL Migration Tests"
{
    Subtype = Test;
    TestPermissions = Disabled;

    var
        Assert: Codeunit Assert;

    // ============================================================
    // CSV Import Helper
    // ============================================================

    local procedure ImportTestDataFromCsv()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        ResourceInStream: InStream;
        LineText: Text;
    begin
        RLMigrationBuffer.DeleteAll();

        NavApp.GetResource('RecordLinkTestData.csv', ResourceInStream);
        // Skip the header line
        ResourceInStream.ReadText(LineText);
        // Import remaining lines via XMLport
        Xmlport.Import(Xmlport::"RL Migration Buffer Import", ResourceInStream);
    end;

    // ============================================================
    // Transfer Tests
    // ============================================================

    [Test]
    procedure Transfer_EmptyBuffer_NoRecordLinksCreated()
    var
        RLMigrationProgress: Record "RL Migration Progress";
        RecordLink: Record "Record Link";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
        InitialCount: Integer;
    begin
        // [SCENARIO] When buffer is empty, transfer creates nothing
        CleanupTestData();
        InitialCount := RecordLink.Count();

        RLMigrationMgt.TransferBufferToRecordLinks();

        Assert.AreEqual(InitialCount, RecordLink.Count(), 'No record links should be created from empty buffer');
    end;

    [Test]
    procedure Transfer_ReplicatedCompany_CreatesRecordLinks()
    var
        RLMigrationMapping: Record "RL Migration Mapping";
        RecordLink: Record "Record Link";
        RLMigrationProgress: Record "RL Migration Progress";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
        InitialCount: Integer;
    begin
        // [SCENARIO] Buffer records for replicated companies are transferred
        CleanupTestData();
        ImportTestDataFromCsv();
        InitialCount := RecordLink.Count();

        SetupReplicatedCompany('CRONUS');
        SetupReplicatedCompany('FABRIKAM');

        RLMigrationMgt.TransferBufferToRecordLinks();

        // 8 buffer records should create 8 new Record Links (including blank company)
        Assert.AreEqual(InitialCount + 20, RecordLink.Count(), 'Expected 20 new Record Links');

        // Verify mapping created for each
        Assert.AreEqual(20, RLMigrationMapping.Count(), 'Expected 20 mapping records');

        // Verify progress for CRONUS
        RLMigrationProgress.Get('CRONUS');
        Assert.AreEqual(RLMigrationProgress.Status::Completed, RLMigrationProgress.Status, 'CRONUS should be completed');
    end;

    [Test]
    procedure Transfer_NonReplicatedCompany_Skipped()
    var
        RLMigrationMapping: Record "RL Migration Mapping";
        RecordLink: Record "Record Link";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
        InitialCount: Integer;
    begin
        // [SCENARIO] Buffer records for non-replicated companies are skipped
        CleanupTestData();
        ImportTestDataFromCsv();
        InitialCount := RecordLink.Count();

        // Only CRONUS replicated, not FABRIKAM
        SetupReplicatedCompany('CRONUS');

        RLMigrationMgt.TransferBufferToRecordLinks();

        // CRONUS has 4 records (IDs 1,2,3,6) + blank company has 2 (IDs 7,8) = 6
        Assert.AreEqual(InitialCount + 14, RecordLink.Count(), 'Expected 14 new Record Links (CRONUS + blank company)');

        // FABRIKAM records should not be mapped
        RLMigrationMapping.SetRange(Company, 'FABRIKAM');
        Assert.RecordIsEmpty(RLMigrationMapping);
    end;

    [Test]
    procedure Transfer_BlankCompany_AlwaysMigrated()
    var
        RLMigrationMapping: Record "RL Migration Mapping";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] Records with blank company are always migrated regardless of replicated companies
        CleanupTestData();
        ImportTestDataFromCsv();

        // No companies replicated at all
        RLMigrationMgt.TransferBufferToRecordLinks();

        // Only blank company records (IDs 7,8) should be migrated
        RLMigrationMapping.SetRange(Company, '');
        Assert.AreEqual(5, RLMigrationMapping.Count(), 'Expected 5 blank-company records migrated');
    end;

    [Test]
    procedure Transfer_AlreadyMigrated_Skipped()
    var
        RLMigrationMapping: Record "RL Migration Mapping";
        RecordLink: Record "Record Link";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
        InitialCount: Integer;
    begin
        // [SCENARIO] Running transfer twice does not create duplicates
        CleanupTestData();
        ImportTestDataFromCsv();
        SetupReplicatedCompany('CRONUS');
        SetupReplicatedCompany('FABRIKAM');

        // First transfer
        RLMigrationMgt.TransferBufferToRecordLinks();
        InitialCount := RecordLink.Count();

        // Reset progress to allow re-run (but mappings remain)
        ResetProgressOnly();

        // Second transfer
        RLMigrationMgt.TransferBufferToRecordLinks();

        // No new records created
        Assert.AreEqual(InitialCount, RecordLink.Count(), 'Second transfer should not create duplicates');
    end;

    [Test]
    procedure Transfer_ResumesFromLastProcessed()
    var
        RLMigrationProgress: Record "RL Migration Progress";
        RLMigrationMapping: Record "RL Migration Mapping";
        RecordLink: Record "Record Link";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
        InitialCount: Integer;
    begin
        // [SCENARIO] Transfer resumes from last processed Link ID
        CleanupTestData();
        ImportTestDataFromCsv();
        SetupReplicatedCompany('CRONUS');
        InitialCount := RecordLink.Count();

        // Simulate partial progress: CRONUS processed up to ID 2
        RLMigrationProgress."Company Name" := 'CRONUS';
        RLMigrationProgress."Last Processed Link ID" := 2;
        RLMigrationProgress."Migrated Records" := 2;
        RLMigrationProgress.Status := RLMigrationProgress.Status::"In Progress";
        RLMigrationProgress.Insert();

        RLMigrationMgt.TransferBufferToRecordLinks();

        // CRONUS IDs > 2 are: 3, 6, 9, 10, 15, 17, 18 = 7 records
        RLMigrationMapping.SetRange(Company, 'CRONUS');
        Assert.AreEqual(7, RLMigrationMapping.Count(), 'Expected 7 CRONUS records migrated after resume (IDs > 2)');
    end;

    // ============================================================
    // User Mapping Tests
    // ============================================================

    [Test]
    procedure UserMapping_PopulateFromBuffer_FindsAllUsers()
    var
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] PopulateUserMappings extracts all distinct users from buffer
        CleanupTestData();
        ImportTestDataFromCsv();

        RLMigrationMgt.PopulateUserMappings(false);

        // Distinct users: JOHN.ONPREM (User+ToUser), JANE.ONPREM (User), ADMIN.ONPREM (User)
        Assert.IsTrue(RLMigrationUserMapping.Get('JOHN.ONPREM'), 'JOHN.ONPREM should be populated');
        Assert.IsTrue(RLMigrationUserMapping.Get('JANE.ONPREM'), 'JANE.ONPREM should be populated');
        Assert.IsTrue(RLMigrationUserMapping.Get('ADMIN.ONPREM'), 'ADMIN.ONPREM should be populated');
        Assert.AreEqual(3, RLMigrationUserMapping.Count(), 'Expected 3 distinct users');
    end;

    [Test]
    procedure UserMapping_ApplyMapping_UpdatesBuffer()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] Applying user mapping updates User ID and To User ID in buffer
        CleanupTestData();
        ImportTestDataFromCsv();

        // Set up mappings
        RLMigrationUserMapping."Source User Name" := 'JOHN.ONPREM';
        RLMigrationUserMapping."Dest User Name" := 'JOHN@CONTOSO.COM';
        RLMigrationUserMapping."Is Mapped" := true;
        RLMigrationUserMapping.Insert();

        RLMigrationUserMapping."Source User Name" := 'JANE.ONPREM';
        RLMigrationUserMapping."Dest User Name" := 'JANE@CONTOSO.COM';
        RLMigrationUserMapping."Is Mapped" := true;
        RLMigrationUserMapping.Insert();

        RLMigrationMgt.ApplyUserMapping();

        // Verify User ID updated
        RLMigrationBuffer.Get(1, 'CRONUS');
        Assert.AreEqual('JOHN@CONTOSO.COM', RLMigrationBuffer."User ID", 'User ID should be mapped for record 1');

        // Verify To User ID updated
        RLMigrationBuffer.Get(2, 'CRONUS');
        Assert.AreEqual('JANE@CONTOSO.COM', RLMigrationBuffer."User ID", 'User ID should be mapped for record 2');
        Assert.AreEqual('JOHN@CONTOSO.COM', RLMigrationBuffer."To User ID", 'To User ID should be mapped for record 2');

        // Verify record in FABRIKAM also got mapped
        RLMigrationBuffer.Get(11, 'FABRIKAM');
        Assert.AreEqual('JOHN@CONTOSO.COM', RLMigrationBuffer."User ID", 'User ID should be mapped cross-company');
        Assert.AreEqual('JANE@CONTOSO.COM', RLMigrationBuffer."To User ID", 'To User ID should be mapped cross-company');
    end;

    [Test]
    procedure UserMapping_UnmappedUsers_NotChanged()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] Users without mapping (Is Mapped = false) are not changed
        CleanupTestData();
        ImportTestDataFromCsv();

        // Only map JOHN, leave ADMIN unmapped
        RLMigrationUserMapping."Source User Name" := 'JOHN.ONPREM';
        RLMigrationUserMapping."Dest User Name" := 'JOHN@CONTOSO.COM';
        RLMigrationUserMapping."Is Mapped" := true;
        RLMigrationUserMapping.Insert();

        RLMigrationUserMapping."Source User Name" := 'ADMIN.ONPREM';
        RLMigrationUserMapping."Dest User Name" := '';
        RLMigrationUserMapping."Is Mapped" := false;
        RLMigrationUserMapping.Insert();

        RLMigrationMgt.ApplyUserMapping();

        // ADMIN should remain unchanged
        RLMigrationBuffer.Get(4, 'FABRIKAM');
        Assert.AreEqual('ADMIN.ONPREM', RLMigrationBuffer."User ID", 'Unmapped user should not be changed');

        // Also verify blank-company record with ADMIN is unchanged
        RLMigrationBuffer.Get(7, '');
        Assert.AreEqual('ADMIN.ONPREM', RLMigrationBuffer."User ID", 'Unmapped user in blank company should not be changed');
    end;

    [Test]
    procedure UserMapping_BlankUserId_NotMapped()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] Records with blank User ID are not affected by mapping
        CleanupTestData();
        ImportTestDataFromCsv();

        RLMigrationMgt.ApplyUserMapping();

        // Record 8 has blank user - should remain blank
        RLMigrationBuffer.Get(8, '');
        Assert.AreEqual('', RLMigrationBuffer."User ID", 'Blank User ID should remain blank');

        // Record 19 also has blank user - should remain blank
        RLMigrationBuffer.Get(19, '');
        Assert.AreEqual('', RLMigrationBuffer."User ID", 'Blank User ID should remain blank for row 19');
    end;

    [Test]
    procedure UserMapping_ClearMappings_RemovesAll()
    var
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] ClearUserMappings removes all entries
        CleanupTestData();
        ImportTestDataFromCsv();

        RLMigrationMgt.PopulateUserMappings(false);
        Assert.AreNotEqual(0, RLMigrationUserMapping.Count(), 'Should have mappings before clear');

        RLMigrationMgt.ClearUserMappings();
        Assert.AreEqual(0, RLMigrationUserMapping.Count(), 'All mappings should be cleared');
    end;

    // ============================================================
    // Duplicate Detection Tests
    // ============================================================

    [Test]
    procedure Duplicates_IdentifyDuplicates_FindsMatchingLinks()
    var
        RLMigrationMapping: Record "RL Migration Mapping";
        RecordLink: Record "Record Link";
        RLMigrationDuplicateFinder: Codeunit "RL Migration Duplicate Finder";
    begin
        // [SCENARIO] IdentifyDuplicates finds buffer records matching existing Record Links
        CleanupTestData();
        ImportTestDataFromCsv();

        // Create an existing record link that matches buffer rows 1 and 6
        RecordLink."Link ID" := 99990;
        RecordLink.Description := 'BC Documentation';
        RecordLink.Type := RecordLink.Type::Link;
        RecordLink.URL1 := 'https://docs.microsoft.com/en-us/dynamics365/business-central/';
        RecordLink.Company := 'CRONUS';
        RecordLink.Insert();

        RLMigrationDuplicateFinder.IdentifyDuplicates();

        // Buffer rows 1 and 6 both match the existing record link (same Description+Type+Company+URL)
        RLMigrationMapping.SetRange("Is Duplicate", true);
        Assert.AreEqual(2, RLMigrationMapping.Count(), 'Expected 2 duplicates (rows 1 and 6 match)');

        // Clean up
        RecordLink.Delete();
    end;

    [Test]
    procedure Duplicates_IsDuplicate_NoteComparison()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RecordLink: Record "Record Link";
        RLMigrationDuplicateFinder: Codeunit "RL Migration Duplicate Finder";
        NoteOutStream: OutStream;
        IsDup: Boolean;
    begin
        // [SCENARIO] Note duplicates are detected by comparing first 2048 chars
        CleanupTestData();
        ImportTestDataFromCsv();

        // Create existing note that matches buffer row 3
        RecordLink."Link ID" := 99991;
        RecordLink.Description := 'Customer follow-up note';
        RecordLink.Type := RecordLink.Type::Note;
        RecordLink.Company := 'CRONUS';
        RecordLink.Note.CreateOutStream(NoteOutStream);
        NoteOutStream.WriteText('Called customer RE: order #1042. They requested delivery postponed to March 15. Need to update shipping schedule and notify warehouse team.');
        RecordLink.Insert();

        RLMigrationBuffer.Get(3, 'CRONUS');
        IsDup := RLMigrationDuplicateFinder.IsDuplicate(RLMigrationBuffer);

        Assert.IsTrue(IsDup, 'Note with same content should be detected as duplicate');

        // Clean up
        RecordLink.Delete();
    end;

    [Test]
    procedure Duplicates_DifferentContent_NotDuplicate()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RecordLink: Record "Record Link";
        RLMigrationDuplicateFinder: Codeunit "RL Migration Duplicate Finder";
    begin
        // [SCENARIO] Record links with different content are not duplicates
        CleanupTestData();
        ImportTestDataFromCsv();

        // Create existing link with same description but different URL
        RecordLink."Link ID" := 99992;
        RecordLink.Description := 'BC Documentation';
        RecordLink.Type := RecordLink.Type::Link;
        RecordLink.URL1 := 'https://completely-different-url.com';
        RecordLink.Company := 'CRONUS';
        RecordLink.Insert();

        RLMigrationBuffer.Get(1, 'CRONUS');
        Assert.IsFalse(RLMigrationDuplicateFinder.IsDuplicate(RLMigrationBuffer), 'Different URL should not be a duplicate');

        // Clean up
        RecordLink.Delete();
    end;

    // ============================================================
    // Buffer Management Tests
    // ============================================================

    [Test]
    procedure Buffer_DeleteBuffer_RemovesAllRecords()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] DeleteBuffer removes all buffer records
        CleanupTestData();
        ImportTestDataFromCsv();

        Assert.AreNotEqual(0, RLMigrationBuffer.Count(), 'Buffer should have data before delete');

        RLMigrationMgt.DeleteBuffer();

        Assert.AreEqual(0, RLMigrationBuffer.Count(), 'Buffer should be empty after delete');
    end;

    [Test]
    procedure Buffer_ResetProgress_ClearsAllProgress()
    var
        RLMigrationProgress: Record "RL Migration Progress";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] ResetProgress clears all progress records
        CleanupTestData();
        ImportTestDataFromCsv();
        SetupReplicatedCompany('CRONUS');

        RLMigrationMgt.TransferBufferToRecordLinks();
        Assert.AreNotEqual(0, RLMigrationProgress.Count(), 'Should have progress after transfer');

        RLMigrationMgt.ResetProgress();
        Assert.AreEqual(0, RLMigrationProgress.Count(), 'Progress should be cleared');
    end;

    // ============================================================
    // Progress Tracking Tests
    // ============================================================

    [Test]
    procedure Progress_PerCompanyTracking()
    var
        RLMigrationProgress: Record "RL Migration Progress";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] Progress is tracked per company
        CleanupTestData();
        ImportTestDataFromCsv();
        SetupReplicatedCompany('CRONUS');
        SetupReplicatedCompany('FABRIKAM');

        RLMigrationMgt.TransferBufferToRecordLinks();

        // Verify CRONUS progress
        RLMigrationProgress.Get('CRONUS');
        Assert.AreEqual(9, RLMigrationProgress."Total Records", 'CRONUS should have 9 total records');
        Assert.AreEqual(9, RLMigrationProgress."Migrated Records", 'CRONUS should have 9 migrated');
        Assert.AreEqual(RLMigrationProgress.Status::Completed, RLMigrationProgress.Status, 'CRONUS should be completed');

        // Verify FABRIKAM progress
        RLMigrationProgress.Get('FABRIKAM');
        Assert.AreEqual(6, RLMigrationProgress."Total Records", 'FABRIKAM should have 6 total records');
        Assert.AreEqual(6, RLMigrationProgress."Migrated Records", 'FABRIKAM should have 6 migrated');
        Assert.AreEqual(RLMigrationProgress.Status::Completed, RLMigrationProgress.Status, 'FABRIKAM should be completed');

        // Verify blank company progress
        RLMigrationProgress.Get('');
        Assert.AreEqual(5, RLMigrationProgress."Total Records", 'Blank company should have 5 total records');
        Assert.AreEqual(RLMigrationProgress.Status::Completed, RLMigrationProgress.Status, 'Blank company should be completed');
    end;

    // ============================================================
    // Rename Users in Record Link Tests
    // ============================================================

    [Test]
    procedure RenameUsers_InRecordLinks_RenamesMatchingUsers()
    var
        RecordLink: Record "Record Link";
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
        RenamedCount: Integer;
    begin
        // [SCENARIO] RenameUsersInRecordLinks renames User ID and To User ID in system Record Links
        CleanupTestData();

        // Create record links with on-prem usernames
        RecordLink."Link ID" := 88001;
        RecordLink."User ID" := 'JOHN.ONPREM';
        RecordLink."To User ID" := 'JANE.ONPREM';
        RecordLink.Company := 'CRONUS';
        RecordLink.Insert();

        RecordLink."Link ID" := 88002;
        RecordLink."User ID" := 'JANE.ONPREM';
        RecordLink."To User ID" := '';
        RecordLink.Company := 'CRONUS';
        RecordLink.Insert();

        // Set up user mappings
        RLMigrationUserMapping."Source User Name" := 'JOHN.ONPREM';
        RLMigrationUserMapping."Dest User Name" := 'JOHN@CONTOSO.COM';
        RLMigrationUserMapping."Is Mapped" := true;
        RLMigrationUserMapping.Insert();

        RLMigrationUserMapping."Source User Name" := 'JANE.ONPREM';
        RLMigrationUserMapping."Dest User Name" := 'JANE@CONTOSO.COM';
        RLMigrationUserMapping."Is Mapped" := true;
        RLMigrationUserMapping.Insert();

        // Act
        RenamedCount := RLMigrationMgt.RenameUsersInRecordLinks();

        // Verify
        Assert.IsTrue(RenamedCount > 0, 'Should have renamed at least one record');

        RecordLink.Get(88001);
        Assert.AreEqual('JOHN@CONTOSO.COM', RecordLink."User ID", 'User ID should be renamed');
        Assert.AreEqual('JANE@CONTOSO.COM', RecordLink."To User ID", 'To User ID should be renamed');

        RecordLink.Get(88002);
        Assert.AreEqual('JANE@CONTOSO.COM', RecordLink."User ID", 'User ID should be renamed for second record');

        // Clean up
        RecordLink.SetFilter("Link ID", '88001|88002');
        RecordLink.DeleteAll();
    end;

    [Test]
    procedure RenameUsers_UnmappedUsers_NotRenamed()
    var
        RecordLink: Record "Record Link";
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] Users without mapping are not renamed in Record Link table
        CleanupTestData();

        // Create record link with unmapped user
        RecordLink."Link ID" := 88003;
        RecordLink."User ID" := 'ADMIN.ONPREM';
        RecordLink.Company := 'CRONUS';
        RecordLink.Insert();

        // Only map JOHN, not ADMIN
        RLMigrationUserMapping."Source User Name" := 'JOHN.ONPREM';
        RLMigrationUserMapping."Dest User Name" := 'JOHN@CONTOSO.COM';
        RLMigrationUserMapping."Is Mapped" := true;
        RLMigrationUserMapping.Insert();

        RLMigrationUserMapping."Source User Name" := 'ADMIN.ONPREM';
        RLMigrationUserMapping."Dest User Name" := '';
        RLMigrationUserMapping."Is Mapped" := false;
        RLMigrationUserMapping.Insert();

        RLMigrationMgt.RenameUsersInRecordLinks();

        RecordLink.Get(88003);
        Assert.AreEqual('ADMIN.ONPREM', RecordLink."User ID", 'Unmapped user should not be renamed');

        // Clean up
        RecordLink.Delete();
    end;

    // ============================================================
    // Transfer Content Integrity Tests
    // ============================================================

    [Test]
    procedure Transfer_NoteWithBlob_PreservesContent()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationMapping: Record "RL Migration Mapping";
        RecordLink: Record "Record Link";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
        NoteInStream: InStream;
        NoteText: Text;
    begin
        // [SCENARIO] Note BLOB content is preserved during transfer to Record Link
        CleanupTestData();
        ImportTestDataFromCsv();
        SetupReplicatedCompany('CRONUS');

        RLMigrationMgt.TransferBufferToRecordLinks();

        // Find the mapping for buffer row 3 (note)
        RLMigrationMapping.Get(3, 'CRONUS');

        // Read the transferred Record Link's note
        RecordLink.Get(RLMigrationMapping."Target Link ID");
        RecordLink.CalcFields(Note);
        Assert.IsTrue(RecordLink.Note.HasValue(), 'Transferred Note should have content');

        RecordLink.Note.CreateInStream(NoteInStream);
        NoteInStream.ReadText(NoteText);
        Assert.IsTrue(NoteText.StartsWith('Called customer RE: order #1042'), 'Note content should be preserved');
    end;

    [Test]
    procedure Transfer_NotifyAndToUserId_Preserved()
    var
        RLMigrationMapping: Record "RL Migration Mapping";
        RecordLink: Record "Record Link";
        RLMigrationMgt: Codeunit "RL Migration Mgt.";
    begin
        // [SCENARIO] Notify flag and To User ID are preserved during transfer
        CleanupTestData();
        ImportTestDataFromCsv();
        SetupReplicatedCompany('CRONUS');

        RLMigrationMgt.TransferBufferToRecordLinks();

        // Buffer row 2: Notify=true, To User ID=JOHN.ONPREM
        RLMigrationMapping.Get(2, 'CRONUS');
        RecordLink.Get(RLMigrationMapping."Target Link ID");
        Assert.IsTrue(RecordLink.Notify, 'Notify should be true for row 2');
        Assert.AreEqual('JOHN.ONPREM', RecordLink."To User ID", 'To User ID should be preserved for row 2');

        // Buffer row 1: Notify=false, To User ID=''
        RLMigrationMapping.Get(1, 'CRONUS');
        RecordLink.Get(RLMigrationMapping."Target Link ID");
        Assert.IsFalse(RecordLink.Notify, 'Notify should be false for row 1');
        Assert.AreEqual('', RecordLink."To User ID", 'To User ID should be blank for row 1');
    end;

    // ============================================================
    // CSV Import Tests
    // ============================================================

    [Test]
    procedure Import_CsvXmlport_LoadsAllRecords()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
    begin
        // [SCENARIO] XMLport correctly imports all 20 rows from CSV with correct field values
        CleanupTestData();
        ImportTestDataFromCsv();

        Assert.AreEqual(20, RLMigrationBuffer.Count(), 'Expected 20 records imported from CSV');

        // Verify a Link record
        RLMigrationBuffer.Get(1, 'CRONUS');
        Assert.AreEqual(RLMigrationBuffer.Type::Link, RLMigrationBuffer.Type, 'Row 1 should be Type Link');
        Assert.AreEqual('BC Documentation', RLMigrationBuffer.Description, 'Row 1 description mismatch');
        Assert.AreEqual('JOHN.ONPREM', RLMigrationBuffer."User ID", 'Row 1 User ID mismatch');
        Assert.AreEqual('https://docs.microsoft.com/en-us/dynamics365/business-central/', RLMigrationBuffer.URL1, 'Row 1 URL mismatch');

        // Verify a Note record with BLOB
        RLMigrationBuffer.Get(3, 'CRONUS');
        Assert.AreEqual(RLMigrationBuffer.Type::Note, RLMigrationBuffer.Type, 'Row 3 should be Type Note');
        RLMigrationBuffer.CalcFields(Note);
        Assert.IsTrue(RLMigrationBuffer.Note.HasValue(), 'Row 3 should have Note content');

        // Verify Notify and To User ID
        RLMigrationBuffer.Get(2, 'CRONUS');
        Assert.IsTrue(RLMigrationBuffer.Notify, 'Row 2 Notify should be true');
        Assert.AreEqual('JOHN.ONPREM', RLMigrationBuffer."To User ID", 'Row 2 To User ID mismatch');

        // Verify blank company record
        RLMigrationBuffer.Get(7, '');
        Assert.AreEqual('ADMIN.ONPREM', RLMigrationBuffer."User ID", 'Row 7 User ID mismatch');
        Assert.AreEqual('', RLMigrationBuffer.Company, 'Row 7 should have blank company');

        // Verify blank user record
        RLMigrationBuffer.Get(8, '');
        Assert.AreEqual('', RLMigrationBuffer."User ID", 'Row 8 should have blank User ID');

        // Verify FABRIKAM record
        RLMigrationBuffer.Get(4, 'FABRIKAM');
        Assert.AreEqual('ERP Sync Bug #42', RLMigrationBuffer.Description, 'Row 4 description mismatch');
        Assert.AreEqual('ADMIN.ONPREM', RLMigrationBuffer."User ID", 'Row 4 User ID mismatch');
    end;

    // ============================================================
    // Helpers
    // ============================================================

    local procedure SetupReplicatedCompany(CompanyName: Text[50])
    var
        HybridCompany: Record "Hybrid Company";
    begin
        if not HybridCompany.Get(CompanyName) then begin
            HybridCompany.Name := CompanyName;
            HybridCompany.Replicate := true;
            HybridCompany.Insert();
        end else begin
            HybridCompany.Replicate := true;
            HybridCompany.Modify();
        end;
    end;

    local procedure ResetProgressOnly()
    var
        RLMigrationProgress: Record "RL Migration Progress";
    begin
        RLMigrationProgress.DeleteAll();
    end;

    local procedure CleanupTestData()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationMapping: Record "RL Migration Mapping";
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationProgress: Record "RL Migration Progress";
        RecordLink: Record "Record Link";
        HybridCompany: Record "Hybrid Company";
    begin
        RLMigrationBuffer.DeleteAll();
        RLMigrationMapping.DeleteAll();
        RLMigrationUserMapping.DeleteAll();
        RLMigrationProgress.DeleteAll();
        HybridCompany.SetFilter(Name, 'CRONUS|FABRIKAM');
        HybridCompany.DeleteAll();
        // Clean up all Record Links to ensure test isolation
        RecordLink.DeleteAll();
    end;
}
