USE [LMS];
GO

-- 1. AI Documents Metadata
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AI_Documents')
BEGIN
    CREATE TABLE [dbo].[AI_Documents] (
        [DocumentId]        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CourseId]          INT NOT NULL,
        [ModuleId]          INT NULL,
        [LessonId]          INT NULL,
        [UploadedBy]        NVARCHAR(128) NOT NULL,
        [FileName]          NVARCHAR(255) NOT NULL,
        [FilePath]          NVARCHAR(500) NOT NULL,
        [FileType]          NVARCHAR(20) NOT NULL,
        [FileSize]          BIGINT NOT NULL,
        [DocumentStatus]    NVARCHAR(50) NOT NULL DEFAULT 'Uploaded',
        [ProcessingError]   NVARCHAR(MAX) NULL,
        [DocumentVersion]   INT NOT NULL DEFAULT 1,
        [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [IndexedAt]         DATETIME2 NULL,
        CONSTRAINT [FK_AI_Documents_Courses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses]([CourseId]) ON DELETE CASCADE,
        CONSTRAINT [FK_AI_Documents_Modules] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Modules]([ModuleId]),
        CONSTRAINT [FK_AI_Documents_Lessons] FOREIGN KEY ([LessonId]) REFERENCES [dbo].[Lessons]([LessonId]),
        CONSTRAINT [FK_AI_Documents_Users] FOREIGN KEY ([UploadedBy]) REFERENCES [dbo].[AspNetUsers]([Id])
    );
END
GO

-- 2. AI Document Chunks
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AI_DocumentChunks')
BEGIN
    CREATE TABLE [dbo].[AI_DocumentChunks] (
        [ChunkId]           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [DocumentId]        INT NOT NULL,
        [ChunkIndex]        INT NOT NULL,
        [ExtractedText]     NVARCHAR(MAX) NOT NULL,
        [PageNumber]        INT NULL,
        [SectionTitle]      NVARCHAR(255) NULL,
        [VectorId]          NVARCHAR(128) NOT NULL,
        [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [FK_AI_DocumentChunks_Documents] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[AI_Documents]([DocumentId]) ON DELETE CASCADE
    );
END
GO

-- 3. AI Chat Sessions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AI_ChatSessions')
BEGIN
    CREATE TABLE [dbo].[AI_ChatSessions] (
        [SessionId]         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId]            NVARCHAR(128) NOT NULL,
        [CourseId]          INT NULL,
        [SessionTitle]      NVARCHAR(255) NOT NULL,
        [Mode]              NVARCHAR(50) NOT NULL DEFAULT 'CourseAssistant',
        [IsArchived]        BIT NOT NULL DEFAULT 0,
        [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [FK_AI_ChatSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]),
        CONSTRAINT [FK_AI_ChatSessions_Courses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses]([CourseId])
    );
END
GO

-- 4. AI Chat Messages
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AI_ChatMessages')
BEGIN
    CREATE TABLE [dbo].[AI_ChatMessages] (
        [MessageId]         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [SessionId]         INT NOT NULL,
        [Sender]            NVARCHAR(20) NOT NULL,
        [MessageText]       NVARCHAR(MAX) NOT NULL,
        [TokenCount]        INT NOT NULL DEFAULT 0,
        [ResponseDuration]  INT NOT NULL DEFAULT 0,
        [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [FK_AI_ChatMessages_Sessions] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[AI_ChatSessions]([SessionId]) ON DELETE CASCADE
    );
END
GO

-- 5. AI Chat Sources (Grounding Citations)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AI_ChatSources')
BEGIN
    CREATE TABLE [dbo].[AI_ChatSources] (
        [SourceId]          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MessageId]         INT NOT NULL,
        [DocumentId]        INT NULL,
        [ChunkId]           INT NULL,
        [SourceTitle]       NVARCHAR(255) NOT NULL,
        [PageNumber]        INT NULL,
        [RelevanceScore]    FLOAT NOT NULL,
        [Excerpt]           NVARCHAR(1000) NULL,
        CONSTRAINT [FK_AI_ChatSources_Messages] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[AI_ChatMessages]([MessageId]) ON DELETE CASCADE,
        CONSTRAINT [FK_AI_ChatSources_Documents] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[AI_Documents]([DocumentId]),
        CONSTRAINT [FK_AI_ChatSources_Chunks] FOREIGN KEY ([ChunkId]) REFERENCES [dbo].[AI_DocumentChunks]([ChunkId])
    );
END
GO

-- 6. AI Usage Logs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AI_UsageLogs')
BEGIN
    CREATE TABLE [dbo].[AI_UsageLogs] (
        [UsageId]           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId]            NVARCHAR(128) NOT NULL,
        [SessionId]         INT NULL,
        [ModelName]         NVARCHAR(100) NOT NULL,
        [PromptTokens]      INT NOT NULL,
        [CompletionTokens]  INT NOT NULL,
        [TotalTokens]       INT NOT NULL,
        [EstimatedCost]     DECIMAL(18, 6) NOT NULL DEFAULT 0.0,
        [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [FK_AI_UsageLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id])
    );
END
GO

-- 7. AI Feedback
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AI_Feedback')
BEGIN
    CREATE TABLE [dbo].[AI_Feedback] (
        [FeedbackId]        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MessageId]         INT NOT NULL,
        [UserId]            NVARCHAR(128) NOT NULL,
        [Rating]            INT NOT NULL,
        [FeedbackText]      NVARCHAR(1000) NULL,
        [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [FK_AI_Feedback_Messages] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[AI_ChatMessages]([MessageId]) ON DELETE CASCADE,
        CONSTRAINT [FK_AI_Feedback_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id])
    );
END
GO

-- 8. AI Indexing Background Jobs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AI_IndexingJobs')
BEGIN
    CREATE TABLE [dbo].[AI_IndexingJobs] (
        [JobId]             INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [DocumentId]        INT NOT NULL,
        [JobStatus]         NVARCHAR(50) NOT NULL DEFAULT 'Queued',
        [StartedAt]         DATETIME2 NULL,
        [CompletedAt]       DATETIME2 NULL,
        [ErrorMessage]      NVARCHAR(MAX) NULL,
        [RetryCount]        INT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_AI_IndexingJobs_Documents] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[AI_Documents]([DocumentId]) ON DELETE CASCADE
    );
END
GO

-- Performance Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AI_Documents_Course_Status')
    CREATE NONCLUSTERED INDEX [IX_AI_Documents_Course_Status] ON [dbo].[AI_Documents]([CourseId], [DocumentStatus]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AI_ChatSessions_User')
    CREATE NONCLUSTERED INDEX [IX_AI_ChatSessions_User] ON [dbo].[AI_ChatSessions]([UserId], [IsArchived]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AI_ChatMessages_Session')
    CREATE NONCLUSTERED INDEX [IX_AI_ChatMessages_Session] ON [dbo].[AI_ChatMessages]([SessionId], [CreatedAt]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AI_ChatSources_Message')
    CREATE NONCLUSTERED INDEX [IX_AI_ChatSources_Message] ON [dbo].[AI_ChatSources]([MessageId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AI_UsageLogs_User_Created')
    CREATE NONCLUSTERED INDEX [IX_AI_UsageLogs_User_Created] ON [dbo].[AI_UsageLogs]([UserId], [CreatedAt]);
GO
