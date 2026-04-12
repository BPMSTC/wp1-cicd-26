-- Purpose: Create the dbo.Missions table used by the application Mission model.
-- Execution order: Run this script first (01) before any seed/data scripts.

IF OBJECT_ID(N'dbo.Missions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Missions
    (
        MissionId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Title NVARCHAR(100) NOT NULL,
        Description NVARCHAR(1000) NULL,
        AssignedSector NVARCHAR(80) NOT NULL,
        Commander NVARCHAR(80) NULL,
        Priority NVARCHAR(20) NOT NULL,
        Status NVARCHAR(30) NOT NULL,
        ThreatLevel NVARCHAR(20) NOT NULL,
        LaunchDate DATE NOT NULL
    );
END;
