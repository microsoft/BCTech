table 50101 "Dataverse cdm_worker"
{
    ExternalName = 'cdm_worker';
    TableType = CDS;
    Description = 'A person who is associated to a job within a company.';

    fields
    {
        field(1; cdm_workerId; GUID)
        {
            ExternalName = 'cdm_workerid';
            ExternalType = 'Uniqueidentifier';
            ExternalAccess = Insert;
            Description = 'Unique identifier for entity instances';
            Caption = 'Worker';
        }
        field(2; CreatedOn; Datetime)
        {
            ExternalName = 'createdon';
            ExternalType = 'DateTime';
            ExternalAccess = Read;
            Description = 'Date and time when the record was created.';
            Caption = 'Created On';
        }
        field(3; CreatedBy; GUID)
        {
            ExternalName = 'createdby';
            ExternalType = 'Lookup';
            ExternalAccess = Read;
            Description = 'Unique identifier of the user who created the record.';
            Caption = 'Created By';
            TableRelation = "CRM Systemuser".SystemUserId;
        }
        field(4; ModifiedOn; Datetime)
        {
            ExternalName = 'modifiedon';
            ExternalType = 'DateTime';
            ExternalAccess = Read;
            Description = 'Date and time when the record was modified.';
            Caption = 'Modified On';
        }
        field(5; ModifiedBy; GUID)
        {
            ExternalName = 'modifiedby';
            ExternalType = 'Lookup';
            ExternalAccess = Read;
            Description = 'Unique identifier of the user who modified the record.';
            Caption = 'Modified By';
            TableRelation = "CRM Systemuser".SystemUserId;
        }
        field(6; CreatedOnBehalfBy; GUID)
        {
            ExternalName = 'createdonbehalfby';
            ExternalType = 'Lookup';
            ExternalAccess = Read;
            Description = 'Unique identifier of the delegate user who created the record.';
            Caption = 'Created By (Delegate)';
            TableRelation = "CRM Systemuser".SystemUserId;
        }
        field(7; ModifiedOnBehalfBy; GUID)
        {
            ExternalName = 'modifiedonbehalfby';
            ExternalType = 'Lookup';
            ExternalAccess = Read;
            Description = 'Unique identifier of the delegate user who modified the record.';
            Caption = 'Modified By (Delegate)';
            TableRelation = "CRM Systemuser".SystemUserId;
        }
        field(16; OwnerId; GUID)
        {
            ExternalName = 'ownerid';
            ExternalType = 'Owner';
            Description = 'Owner Id';
            Caption = 'Owner';
        }
        field(21; OwningBusinessUnit; GUID)
        {
            ExternalName = 'owningbusinessunit';
            ExternalType = 'Lookup';
            ExternalAccess = Read;
            Description = 'Unique identifier for the business unit that owns the record';
            Caption = 'Owning Business Unit';
            TableRelation = "CRM Businessunit".BusinessUnitId;
        }
        field(22; OwningUser; GUID)
        {
            ExternalName = 'owninguser';
            ExternalType = 'Lookup';
            ExternalAccess = Read;
            Description = 'Unique identifier for the user that owns the record.';
            Caption = 'Owning User';
            TableRelation = "CRM Systemuser".SystemUserId;
        }
        field(23; OwningTeam; GUID)
        {
            ExternalName = 'owningteam';
            ExternalType = 'Lookup';
            ExternalAccess = Read;
            Description = 'Unique identifier for the team that owns the record.';
            Caption = 'Owning Team';
            TableRelation = "CRM Team".TeamId;
        }
        field(24; statecode; Option)
        {
            ExternalName = 'statecode';
            ExternalType = 'State';
            ExternalAccess = Modify;
            Description = 'Status of the Worker';
            Caption = 'Status';
            InitValue = " ";
            OptionMembers = " ",Active,Inactive;
            OptionOrdinalValues = -1, 0, 1;
        }
        field(26; statuscode; Option)
        {
            ExternalName = 'statuscode';
            ExternalType = 'Status';
            Description = 'Reason for the status of the Worker';
            Caption = 'Status Reason';
            InitValue = " ";
            OptionMembers = " ",Active,Inactive;
            OptionOrdinalValues = -1, 1, 2;
        }
        field(28; VersionNumber; BigInteger)
        {
            ExternalName = 'versionnumber';
            ExternalType = 'BigInt';
            ExternalAccess = Read;
            Description = 'Version Number';
            Caption = 'Version Number';
        }
        field(29; ImportSequenceNumber; Integer)
        {
            ExternalName = 'importsequencenumber';
            ExternalType = 'Integer';
            ExternalAccess = Insert;
            Description = 'Sequence number of the import that created this record.';
            Caption = 'Import Sequence Number';
        }
        field(30; OverriddenCreatedOn; Datetime)
        {
            ExternalName = 'overriddencreatedon';
            ExternalType = 'DateTime';
            ExternalAccess = Insert;
            Description = 'Date and time that the record was migrated.';
            Caption = 'Record Created On';
        }
        field(31; TimeZoneRuleVersionNumber; Integer)
        {
            ExternalName = 'timezoneruleversionnumber';
            ExternalType = 'Integer';
            Description = 'For internal use only.';
            Caption = 'Time Zone Rule Version Number';
        }
        field(32; UTCConversionTimeZoneCode; Integer)
        {
            ExternalName = 'utcconversiontimezonecode';
            ExternalType = 'Integer';
            Description = 'Time zone code that was in use when the record was created.';
            Caption = 'UTC Conversion Time Zone Code';
        }
        field(33; cdm_WorkerNumber; Text[128])
        {
            ExternalName = 'cdm_workernumber';
            ExternalType = 'String';
            Description = 'The number of the worker';
            Caption = 'Worker Number';
        }
        field(34; cdm_Birthdate; Date)
        {
            ExternalName = 'cdm_birthdate';
            ExternalType = 'DateTime';
            Description = 'The birthdate of the worker';
            Caption = 'Birthdate';
        }
        field(35; cdm_Description; Text[2000])
        {
            ExternalName = 'cdm_description';
            ExternalType = 'String';
            Description = 'The description of the worker';
            Caption = 'Description';
        }
        field(36; cdm_EmailAddress1; Text[255])
        {
            ExternalName = 'cdm_emailaddress1';
            ExternalType = 'String';
            Description = 'The first alternate email address of the worker';
            Caption = 'Email Address 1';
        }
        field(37; cdm_EmailAddress2; Text[80])
        {
            ExternalName = 'cdm_emailaddress2';
            ExternalType = 'String';
            Description = 'The second alternate email address of the worker';
            Caption = 'Email Address 2';
        }
        field(38; cdm_FacebookIdentity; Text[255])
        {
            ExternalName = 'cdm_facebookidentity';
            ExternalType = 'String';
            Description = 'The Facebook identity of the worker';
            Caption = 'Facebook Identity';
        }
        field(39; cdm_FirstName; Text[100])
        {
            ExternalName = 'cdm_firstname';
            ExternalType = 'String';
            Description = 'The given name of the worker';
            Caption = 'First Name';
        }
        field(40; cdm_FullName; Text[128])
        {
            ExternalName = 'cdm_fullname';
            ExternalType = 'String';
            Description = 'The combination of the first, middle, and last name of the worker';
            Caption = 'Full Name';
        }
        field(41; cdm_Gender; Option)
        {
            ExternalName = 'cdm_gender';
            ExternalType = 'Picklist';
            Description = 'The gender of the worker';
            Caption = 'Gender';
            InitValue = NotSpecified;
            OptionMembers = Male,Female,NotSpecified,"Non-specific";
            OptionOrdinalValues = 754400000, 754400001, 754400002, 754400003;
        }
        field(43; cdm_Generation; Text[128])
        {
            ExternalName = 'cdm_generation';
            ExternalType = 'String';
            Description = 'The generation of the worker';
            Caption = 'Generation';
        }
        field(44; cdm_IsEmailContactAllowed; Boolean)
        {
            ExternalName = 'cdm_isemailcontactallowed';
            ExternalType = 'Boolean';
            Description = 'Determines whether contact of this worker via email is permitted';
            Caption = 'Is Email Contact Allowed';
        }
        field(46; cdm_IsPhoneContactAllowed; Boolean)
        {
            ExternalName = 'cdm_isphonecontactallowed';
            ExternalType = 'Boolean';
            Description = 'Determines whether contact of the worker via phone is permitted';
            Caption = 'Is Phone Contact Allowed';
        }
        field(48; cdm_LastName; Text[100])
        {
            ExternalName = 'cdm_lastname';
            ExternalType = 'String';
            Description = 'The last name of the worker.';
            Caption = 'Last Name';
        }
        field(49; cdm_LinkedInIdentity; Text[255])
        {
            ExternalName = 'cdm_linkedinidentity';
            ExternalType = 'String';
            Description = 'The LinkedIn identity of the worker';
            Caption = 'LinkedIn Identity';
        }
        field(50; cdm_ManagerWorkerId; GUID)
        {
            ExternalName = 'cdm_managerworkerid';
            ExternalType = 'Lookup';
            Description = 'The manager of the worker';
            Caption = 'Manager';
            TableRelation = "Dataverse cdm_worker".cdm_workerId;
        }
        field(51; cdm_MiddleName; Text[100])
        {
            ExternalName = 'cdm_middlename';
            ExternalType = 'String';
            Description = 'The middle name of the worker';
            Caption = 'Middle Name';
        }
        field(52; cdm_MobilePhone; Text[50])
        {
            ExternalName = 'cdm_mobilephone';
            ExternalType = 'String';
            Description = 'The mobile phone number of the worker';
            Caption = 'Mobile Phone';
        }
        field(53; cdm_OfficeGraphIdentifier; Text[200])
        {
            ExternalName = 'cdm_officegraphidentifier';
            ExternalType = 'String';
            Description = 'The Office Graph identifier of the worker';
            Caption = 'Office Graph Identifier';
        }
        field(54; cdm_PrimaryEmailAddress; Text[80])
        {
            ExternalName = 'cdm_primaryemailaddress';
            ExternalType = 'String';
            Description = 'The primary email address of the worker';
            Caption = 'Primary Email Address';
        }
        field(55; cdm_PrimaryTelephone; Text[255])
        {
            ExternalName = 'cdm_primarytelephone';
            ExternalType = 'String';
            Description = 'The primary telephone number of the worker.';
            Caption = 'Primary Telephone';
        }
        field(56; cdm_Profession; Text[128])
        {
            ExternalName = 'cdm_profession';
            ExternalType = 'String';
            Description = 'The profession of the worker';
            Caption = 'Profession';
        }
        field(57; cdm_SocialNetwork01; Option)
        {
            ExternalName = 'cdm_socialnetwork01';
            ExternalType = 'Picklist';
            Description = 'The first alternate social network';
            Caption = 'Social Network 1';
            InitValue = " ";
            OptionMembers = " ",Facebook,Twitter,LinkedIn,XING,Konnects,Myspace;
            OptionOrdinalValues = -1, 754400000, 754400001, 754400002, 754400003, 754400004, 754400005;
        }
        field(59; cdm_SocialNetwork02; Option)
        {
            ExternalName = 'cdm_socialnetwork02';
            ExternalType = 'Picklist';
            Description = 'The second alterative social network';
            Caption = 'Social Network 2';
            InitValue = " ";
            OptionMembers = " ",Facebook,Twitter,LinkedIn,XING,Konnects,Myspace;
            OptionOrdinalValues = -1, 754400000, 754400001, 754400002, 754400003, 754400004, 754400005;
        }
        field(61; cdm_SocialNetworkIdentity01; Text[128])
        {
            ExternalName = 'cdm_socialnetworkidentity01';
            ExternalType = 'String';
            Description = 'The identity related to social network 1';
            Caption = 'Social Network Identity 1';
        }
        field(62; cdm_SocialNetworkIdentity02; Text[128])
        {
            ExternalName = 'cdm_socialnetworkidentity02';
            ExternalType = 'String';
            Description = 'The identity related to social network 2';
            Caption = 'Social Network Identity 2';
        }
        field(63; cdm_Source; Option)
        {
            ExternalName = 'cdm_source';
            ExternalType = 'Picklist';
            Description = 'The source of the worker';
            Caption = 'Source';
            InitValue = Default;
            OptionMembers = Default,Operations,CustomerEngagement,Finance,Attract,Onboarding,Gauge,LinkedIn,Greenhouse,ICIMS,Talent;
            OptionOrdinalValues = 754400000, 754400001, 754400002, 754400003, 754400004, 754400005, 754400006, 754400007, 754400008, 754400009, 754400010;
        }
        field(65; cdm_Status; Option)
        {
            ExternalName = 'cdm_status';
            ExternalType = 'Picklist';
            Description = 'The status of the worker  Example: Active, Inactive';
            Caption = 'Status';
            InitValue = Active;
            OptionMembers = Active,Inactive;
            OptionOrdinalValues = 754400000, 754400001;
        }
        field(67; cdm_Telephone1; Text[50])
        {
            ExternalName = 'cdm_telephone1';
            ExternalType = 'String';
            Description = 'The first alternate telephone number of the worker.';
            Caption = 'Telephone 1';
        }
        field(68; cdm_Telephone2; Text[50])
        {
            ExternalName = 'cdm_telephone2';
            ExternalType = 'String';
            Description = 'The second alternate telephone number of the worker.';
            Caption = 'Telephone 2';
        }
        field(69; cdm_Telephone3; Text[50])
        {
            ExternalName = 'cdm_telephone3';
            ExternalType = 'String';
            Description = 'The third alternate phone number of the worker.';
            Caption = 'Telephone 3';
        }
        field(70; cdm_TwitterIdentity; Text[255])
        {
            ExternalName = 'cdm_twitteridentity';
            ExternalType = 'String';
            Description = 'The Twitter identity of the worker.';
            Caption = 'Twitter Identity';
        }
        field(71; cdm_Type; Option)
        {
            ExternalName = 'cdm_type';
            ExternalType = 'Picklist';
            Description = 'The type of the worker.';
            Caption = 'Worker Type';
            InitValue = Employee;
            OptionMembers = Employee,Contractor,Volunteer,Unspecified;
            OptionOrdinalValues = 754400000, 754400001, 754400002, 754400003;
        }
        field(73; cdm_WebsiteURL; Text[300])
        {
            ExternalName = 'cdm_websiteurl';
            ExternalType = 'String';
            Description = 'The URL of the worker''s personal website.';
            Caption = 'Website URL';
        }
        field(74; cdm_YomiFirstName; Text[150])
        {
            ExternalName = 'cdm_yomifirstname';
            ExternalType = 'String';
            Description = 'The phonetic annunciation of the first name of the worker';
            Caption = 'Yomi First Name';
        }
        field(75; cdm_YomiFullName; Text[150])
        {
            ExternalName = 'cdm_yomifullname';
            ExternalType = 'String';
            Description = 'The phonetic annunciation of the full name of the worker';
            Caption = 'Yomi Full Name';
        }
        field(76; cdm_YomiLastName; Text[150])
        {
            ExternalName = 'cdm_yomilastname';
            ExternalType = 'String';
            Description = 'The phonetic annunciation of the last name of the worker';
            Caption = 'Yomi Last Name';
        }
        field(77; cdm_YomiMiddleName; Text[150])
        {
            ExternalName = 'cdm_yomimiddlename';
            ExternalType = 'String';
            Description = 'The phonetic annunciation of the middle name of the worker';
            Caption = 'Yomi Middle Name';
        }
        field(78; cdm_ManagerWorkerIdName; Text[128])
        {
            FieldClass = FlowField;
            CalcFormula = lookup("Dataverse cdm_worker".cdm_WorkerNumber where(cdm_workerId = field(cdm_ManagerWorkerId)));
            ExternalName = 'cdm_managerworkeridname';
            ExternalType = 'String';
            ExternalAccess = Read;
        }
        field(79; msdyn_LinkedInmemberreference; Text[128])
        {
            ExternalName = 'msdyn_linkedinmemberreference';
            ExternalType = 'String';
            Description = 'Reference to LinkedIn member identifier';
            Caption = 'LinkedIn member reference';
        }
        field(80; msdyn_UserId; GUID)
        {
            ExternalName = 'msdyn_userid';
            ExternalType = 'Lookup';
            Description = 'Unique identifier for User associated with Worker.';
            Caption = 'User';
            TableRelation = "CRM Systemuser".SystemUserId;
        }
    }
    keys
    {
        key(PK; cdm_workerId)
        {
            Clustered = true;
        }
        key(Name; cdm_WorkerNumber)
        {
        }
    }
    fieldgroups
    {
        fieldgroup(Dropdown; cdm_WorkerNumber)
        {
        }
    }
}