# Record Link Migration — User Guide

This tool migrates your record links and notes from on-premises Business Central to Business Central Online as part of cloud migration.

## What It Does

When you move to the cloud, your record links (URLs attached to records) and notes are not migrated automatically. This tool handles that migration by:

- Copying record links and notes from your on-premises database into a staging area
- Letting you map on-premises usernames to their cloud equivalents
- Detecting duplicates to avoid creating the same link twice
- Transferring the data into Business Central Online

## Step-by-Step

### 1. Set up the migration

In the **Cloud Migration Setup** wizard, select **Record Link Migration** as the Custom Migration Provider. Run replication as normal — your record links will be copied into a staging buffer.

### 2. Map your users

Open the **Record Link Migration** page and click **Define User Mappings**.

- Click **Populate Users** — this auto-detects all usernames from the staged data and tries to match them to cloud users by email
- Review the list and manually assign any unmatched users
- Click **Apply Mapping** — this updates the staged data with the correct cloud usernames

### 3. Check for issues (optional)

- **Identify Duplicates** — finds record links that already exist in the cloud. You can choose to skip or overwrite each one.
- **Verify Records Exist** — checks whether the records that links point to still exist. Warns you about any that don't.

### 4. Transfer

Click **Transfer to Record Link**. The tool creates the record links in Business Central Online. Progress is shown per company.

If the transfer is interrupted, just run it again — it picks up where it left off.

### 5. Clean up (optional)

The staged data remains after transfer so you can re-run or check for duplicates later. When you're satisfied, click **Delete Buffer** to remove it. You'll see a warning that future duplicate detection won't be possible without it.

## Important Notes

- **User mapping should be done before transfer** — once applied, usernames are updated in the staging area and carried into the final transfer
- **Only replicated companies are included** — record links for companies not selected for replication are skipped
- **Links without a company** (system-wide links) are always transferred
- **Each transfer assigns new IDs** — your existing cloud record links are never affected
