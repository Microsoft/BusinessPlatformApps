SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE VIEW pbist_edfi.vw_AcademicSubjectDescriptor AS
    SELECT AcademicSubjectDescriptorId, AcademicSubjectTypeId
    FROM edfi.AcademicSubjectDescriptor;
go

CREATE VIEW pbist_edfi.vw_AcademicSubjectType AS
    SELECT AcademicSubjectTypeId, CodeValue
    FROM edfi.AcademicSubjectType;
go

CREATE VIEW pbist_edfi.vw_AddressType AS
    SELECT AddressTypeId, CodeValue
    FROM edfi.AddressType;
go

CREATE VIEW pbist_edfi.vw_AttendanceEventCategoryDescriptor AS
    SELECT AttendanceEventCategoryDescriptorId, AttendanceEventCategoryTypeId
    FROM edfi.AttendanceEventCategoryDescriptor;
go

CREATE VIEW pbist_edfi.vw_AttendanceEventCategoryType AS
    SELECT AttendanceEventCategoryTypeId, CodeValue
    FROM edfi.AttendanceEventCategoryType;
go

CREATE VIEW pbist_edfi.vw_BehaviorDescriptor AS
    SELECT BehaviorDescriptorId, BehaviorTypeId
    FROM edfi.BehaviorDescriptor;
go

CREATE VIEW pbist_edfi.vw_CalendarDateCalendarEvent AS
    SELECT SchoolId, [Date], CalendarEventDescriptorId, EventDuration, CreateDate
    FROM edfi.CalendarDateCalendarEvent;
go

CREATE VIEW pbist_edfi.vw_CalendarEventDescriptor AS
    SELECT CalendarEventDescriptorId, CalendarEventTypeId
    FROM edfi.CalendarEventDescriptor;
go

CREATE VIEW pbist_edfi.vw_CalendarEventType AS
    SELECT CalendarEventTypeId,	CodeValue
    FROM edfi.CalendarEventType;
go

CREATE VIEW pbist_edfi.vw_Course AS
    SELECT EducationOrganizationId,	CourseCode,	CourseTitle, AcademicSubjectDescriptorId
    FROM edfi.Course;
go

CREATE VIEW pbist_edfi.vw_CourseOffering AS
    SELECT LocalCourseCode,	SchoolId, SchoolYear, TermDescriptorId, LocalCourseTitle, InstructionalTimePlanned, CourseCode, EducationOrganizationId
    FROM edfi.CourseOffering;
go

CREATE VIEW pbist_edfi.vw_DisciplineIncident AS
    SELECT IncidentIdentifier, SchoolId, IncidentDate, ReporterName
    FROM edfi.DisciplineIncident;
go

CREATE VIEW pbist_edfi.vw_DisciplineIncidentBehavior AS
    SELECT SchoolId, IncidentIdentifier, BehaviorDescriptorId
    FROM edfi.DisciplineIncidentBehavior;
go

CREATE VIEW pbist_edfi.vw_EducationOrganization AS
    SELECT EducationOrganizationId, StateOrganizationId, NameOfInstitution
    FROM edfi.EducationOrganization;
go

CREATE VIEW pbist_edfi.vw_ElectronicMailType AS
    SELECT ElectronicMailTypeId, CodeValue, [Description], ShortDescription
    FROM edfi.ElectronicMailType;
go

CREATE VIEW pbist_edfi.vw_Grade AS
    SELECT GradingPeriodDescriptorId, GradeTypeId, StudentUSI, SchoolId, LocalCourseCode, UniqueSectionCode, SchoolYear, TermDescriptorId, BeginDate, LetterGradeEarned, NumericGradeEarned, GradingPeriodBeginDate, ClassPeriodName, ClassroomIdentificationCode
    FROM edfi.Grade;
go

CREATE VIEW pbist_edfi.vw_GradeLevelDescriptor AS
    SELECT GradeLevelDescriptorId, GradeLevelTypeId
    FROM edfi.GradeLevelDescriptor;
go

CREATE VIEW pbist_edfi.vw_GradeLevelType AS
    SELECT GradeLevelTypeId, CodeValue
    FROM edfi.GradeLevelType;
go

CREATE VIEW pbist_edfi.vw_GradeType AS
    SELECT GradeTypeId, CodeValue
    FROM edfi.GradeType;
go

CREATE VIEW pbist_edfi.vw_GradingPeriod AS
    SELECT GradingPeriodDescriptorId, SchoolId, BeginDate, TotalInstructionalDays, EndDate, PeriodSequence
    FROM edfi.GradingPeriod;
go

CREATE VIEW pbist_edfi.vw_LocalEducationAgency AS
    SELECT LocalEducationAgencyId, ParentLocalEducationAgencyId, LocalEducationAgencyCategoryTypeId, CharterStatusTypeId, EducationServiceCenterId, StateEducationAgencyId
    FROM edfi.LocalEducationAgency;
go

CREATE VIEW pbist_edfi.vw_Parent AS
    SELECT ParentUSI, ParentUniqueId, PersonalTitlePrefix, FirstName, MiddleName, LastSurname
    FROM edfi.Parent;
go

CREATE VIEW pbist_edfi.vw_ParentAddress AS
    SELECT ParentUSI, AddressTypeId, StreetNumberName, ApartmentRoomSuiteNumber, BuildingSiteNumber, City, StateAbbreviationTypeId, PostalCode
    FROM edfi.ParentAddress;
go

CREATE VIEW pbist_edfi.vw_ParentElectronicMail AS
    SELECT ParentUSI, ElectronicMailTypeId, ElectronicMailAddress, PrimaryEmailAddressIndicator
    FROM edfi.ParentElectronicMail;
go

CREATE VIEW pbist_edfi.vw_ParentTelephone AS
    SELECT ParentUSI, TelephoneNumberTypeId, TelephoneNumber
    FROM edfi.ParentTelephone;
go

CREATE VIEW pbist_edfi.vw_RelationType AS
    SELECT RelationTypeId, CodeValue
    FROM edfi.RelationType;
go

CREATE VIEW pbist_edfi.vw_School AS
    SELECT SchoolId, LocalEducationAgencyId, SchoolTypeId
    FROM edfi.School;
go

CREATE VIEW pbist_edfi.vw_Section AS
    SELECT SchoolId, ClassPeriodName, ClassroomIdentificationCode, LocalCourseCode, TermDescriptorId, SchoolYear, UniqueSectionCode
    FROM edfi.Section;
go

CREATE VIEW pbist_edfi.vw_Staff AS
    SELECT StaffUSI, FirstName, MiddleName, LastSurname, MaidenName, StaffUniqueId
    FROM edfi.Staff;
go

CREATE VIEW pbist_edfi.vw_StaffEducationOrganizationAssignmentAssociation AS
    SELECT StaffUSI, EducationOrganizationId, StaffClassificationDescriptorId, PositionTitle
    FROM edfi.StaffEducationOrganizationAssignmentAssociation;
go

CREATE VIEW pbist_edfi.vw_StaffElectronicMail AS
    SELECT StaffUSI, ElectronicMailTypeId, ElectronicMailAddress, PrimaryEmailAddressIndicator
    FROM edfi.StaffElectronicMail;
go

CREATE VIEW pbist_edfi.vw_StaffSchoolAssociation AS
    SELECT StaffUSI, ProgramAssignmentDescriptorId, SchoolId, SchoolYear
    FROM edfi.StaffSchoolAssociation;
go

CREATE VIEW pbist_edfi.vw_StaffSectionAssociation AS
    SELECT StaffUSI, SchoolId, LocalCourseCode, SchoolYear, TermDescriptorId, UniqueSectionCode
    FROM edfi.StaffSectionAssociation;
go

CREATE VIEW pbist_edfi.vw_StateAbbreviationType AS
    SELECT StateAbbreviationTypeId, CodeValue
    FROM edfi.StateAbbreviationType;
go

CREATE VIEW pbist_edfi.vw_Student AS
    SELECT StudentUSI, FirstName, MiddleName, LastSurname, SexTypeId, BirthDate, HispanicLatinoEthnicity, OldEthnicityTypeId, EconomicDisadvantaged, StudentUniqueId, CitizenshipStatusTypeId
    FROM edfi.Student;
go

CREATE VIEW pbist_edfi.vw_StudentDisciplineIncidentAssociation AS
    SELECT StudentUSI, SchoolId, IncidentIdentifier
    FROM edfi.StudentDisciplineIncidentAssociation;
go

CREATE VIEW pbist_edfi.vw_StudentDisciplineIncidentAssociationBehavior AS
    SELECT StudentUSI, SchoolId, BehaviorDescriptorId, IncidentIdentifier
    FROM edfi.StudentDisciplineIncidentAssociationBehavior;
go

CREATE VIEW pbist_edfi.vw_ AS
    SELECT StudentUSI, ParentUSI, RelationTypeId, PrimaryContactStatus, LivesWith, ContactPriority, ContactRestrictions
    FROM edfi.StudentParentAssociation;
go

CREATE VIEW pbist_edfi.vw_StudentParentAssociation AS
    SELECT StudentUSI, ParentUSI, RelationTypeId, PrimaryContactStatus, LivesWith, ContactPriority, ContactRestrictions
    FROM edfi.StudentParentAssociation;
go

CREATE VIEW pbist_edfi.vw_StudentSchoolAssociation AS
    SELECT StudentUSI, SchoolId, SchoolYear, EntryDate, ExitWithdrawDate
    FROM edfi.StudentSchoolAssociation;
go

CREATE VIEW pbist_edfi.vw_StudentSchoolAttendanceEvent AS
    SELECT StudentUSI, SchoolId, SchoolYear, EventDate, AttendanceEventCategoryDescriptorId, TermDescriptorId, AttendanceEventReason, EducationalEnvironmentTypeId
    FROM edfi.StudentSchoolAttendanceEvent;
go

CREATE VIEW pbist_edfi.vw_StudentSectionAssociation AS
    SELECT StudentUSI, SchoolId, ClassPeriodName, ClassroomIdentificationCode, LocalCourseCode, UniqueSectionCode, SequenceOfCourse, SchoolYear, TermDescriptorId, BeginDate, EndDate, HomeroomIndicator
    FROM edfi.StudentSectionAssociation;
go

CREATE VIEW pbist_edfi.vw_StudentSectionAttendanceEvent AS
    SELECT StudentUSI, ClassroomIdentificationCode, SchoolId, ClassPeriodName, LocalCourseCode, SchoolYear, TermDescriptorId, UniqueSectionCode, SequenceOfCourse, EventDate, AttendanceEventCategoryDescriptorId
    FROM edfi.StudentSectionAttendanceEvent;
go

CREATE VIEW pbist_edfi.vw_TelephoneNumberType AS
    SELECT TelephoneNumberTypeId, CodeValue
    FROM edfi.TelephoneNumberType;
go

CREATE VIEW pbist_edfi.vw_TermDescriptor AS
    SELECT TermDescriptorId, TermTypeId
    FROM edfi.TermDescriptor;
go

CREATE VIEW pbist_edfi.vw_TermType AS
    SELECT CodeValue, [Description], ShortDescription, Id
    FROM edfi.TermType;
go
