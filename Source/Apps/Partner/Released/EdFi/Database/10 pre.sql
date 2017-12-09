SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Must be executed inside the target database
DECLARE @stmt AS VARCHAR(500), @p1 AS VARCHAR(100), @p2 AS VARCHAR(100);
DECLARE @cr CURSOR;

-- Must be executed inside the target database
-- drop views
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_TYPE='VIEW' AND
                    TABLE_SCHEMA='pbist_edfi' AND
                    TABLE_NAME IN ('vw_AcademicSubjectDescriptor', 'vw_AcademicSubjectType', 'vw_AddressType', 'vw_AttendanceEventCategoryDescriptor', 'vw_AttendanceEventCategoryType', 
                                   'vw_BehaviorDescriptor', 'vw_CalendarDateCalendarEvent', 'vw_CalendarEventDescriptor', 'vw_CalendarEventType', 'vw_Course', 'vw_CourseOffering', 'vw_DisciplineIncident',
                                   'vw_DisciplineIncidentBehavior', 'vw_EducationOrganization', 'vw_ElectronicMailType', 'vw_Grade', 'vw_GradeLevelDescriptor', 'vw_GradeLevelType', 'vw_GradeType',
                                   'vw_GradingPeriod', 'vw_LocalEducationAgency', 'vw_Parent', 'vw_ParentAddress', 'vw_ParentElectronicMail', 'vw_ParentTelephone', 'vw_RelationType', 'vw_School',
                                   'vw_Section', 'vw_Staff', 'vw_StaffEducationOrganizationAssignmentAssociation', 'vw_StaffElectronicMail', 'vw_StaffSchoolAssociation', 'vw_StaffSectionAssociation',
                                   'vw_StateAbbreviationType', 'vw_Student', 'vw_StudentDisciplineIncidentAssociation', 'vw_StudentDisciplineIncidentAssociationBehavior', 'vw_', 'vw_StudentParentAssociation',
                                   'vw_StudentSchoolAssociation', 'vw_StudentSchoolAttendanceEvent', 'vw_StudentSectionAssociation', 'vw_StudentSectionAttendanceEvent', 'vw_TelephoneNumberType',
                                   'vw_TermDescriptor', 'vw_TermType');
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP VIEW pbist_edfi.' + QuoteName(@p1);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1;
END;
CLOSE @cr;
DEALLOCATE @cr;
go

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='pbist_edfi')
BEGIN
    EXEC ('CREATE SCHEMA pbist_edfi'); -- Avoid batch error
END;
