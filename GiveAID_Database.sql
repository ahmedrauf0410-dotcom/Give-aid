-- ===================================================================================
-- Give-AID — NGO Website Database Creation and Seed Script
-- Microsoft SQL Server Management Studio (SSMS) Compatible
-- Target Database: [GiveAID]
-- ===================================================================================

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'GiveAID')
BEGIN
    CREATE DATABASE [GiveAID]
    COLLATE SQL_Latin1_General_CP1_CI_AS;
    PRINT 'Database [GiveAID] created successfully.';
END
ELSE
BEGIN
    PRINT 'Database [GiveAID] already exists.';
END
GO

USE [GiveAID];
GO

-- Table: SiteSettings
IF OBJECT_ID(N'[dbo].[SiteSettings]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SiteSettings] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [NgoName] NVARCHAR(100) NOT NULL CONSTRAINT [DF_SiteSettings_NgoName] DEFAULT ('Give-AID'),
        [Tagline] NVARCHAR(200) NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [Phone] NVARCHAR(50) NOT NULL,
        [Address] NVARCHAR(250) NOT NULL,
        [CityStateZip] NVARCHAR(100) NOT NULL,
        [FacebookUrl] NVARCHAR(250) NULL,
        [TwitterUrl] NVARCHAR(250) NULL,
        [InstagramUrl] NVARCHAR(250) NULL,
        [YouTubeUrl] NVARCHAR(250) NULL,
        [FooterText] NVARCHAR(500) NOT NULL,
        [LogoUrl] NVARCHAR(300) NULL,
        [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_SiteSettings_UpdatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_SiteSettings] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [SiteSettings] created.';
END
GO

-- Table: Admins
IF OBJECT_ID(N'[dbo].[Admins]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Admins] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Admins_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Admins_IsActive] DEFAULT ((1)),
        CONSTRAINT [PK_Admins] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Admins_Username] UNIQUE NONCLUSTERED ([Username] ASC),
        CONSTRAINT [UQ_Admins_Email] UNIQUE NONCLUSTERED ([Email] ASC)
    );
    PRINT 'Table [Admins] created.';
END
GO

-- Table: Users
IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [Phone] NVARCHAR(20) NULL,
        [Address] NVARCHAR(250) NULL,
        [City] NVARCHAR(100) NULL,
        [Profession] NVARCHAR(100) NULL,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Users_IsActive] DEFAULT ((1)),
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
    );
    PRINT 'Table [Users] created.';
END
GO

-- Table: DonationCauses
IF OBJECT_ID(N'[dbo].[DonationCauses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[DonationCauses] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(150) NOT NULL,
        [Category] NVARCHAR(100) NOT NULL,
        [ShortDescription] NVARCHAR(300) NOT NULL,
        [FullDescription] NVARCHAR(MAX) NOT NULL,
        [TargetAmount] DECIMAL(18,2) NOT NULL,
        [RaisedAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_DonationCauses_RaisedAmount] DEFAULT ((0.00)),
        [ImageUrl] NVARCHAR(300) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_DonationCauses_IsActive] DEFAULT ((1)),
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_DonationCauses_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_DonationCauses] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [DonationCauses] created.';
END
GO

-- Table: Donations
IF OBJECT_ID(N'[dbo].[Donations]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Donations] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NULL,
        [DonationCauseId] INT NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [DonorName] NVARCHAR(100) NOT NULL,
        [DonorEmail] NVARCHAR(150) NOT NULL,
        [DonorPhone] NVARCHAR(20) NULL,
        [CardHolderName] NVARCHAR(100) NOT NULL,
        [MaskedCardNumber] NVARCHAR(24) NOT NULL,
        [ExpiryMonth] NVARCHAR(2) NOT NULL,
        [ExpiryYear] NVARCHAR(4) NOT NULL,
        [TransactionReference] NVARCHAR(50) NOT NULL,
        [PaymentStatus] NVARCHAR(30) NOT NULL CONSTRAINT [DF_Donations_PaymentStatus] DEFAULT ('Successful'),
        [DonationDate] DATETIME2 NOT NULL CONSTRAINT [DF_Donations_DonationDate] DEFAULT (SYSUTCDATETIME()),
        [Notes] NVARCHAR(500) NULL,
        CONSTRAINT [PK_Donations] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Donations_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Donations_DonationCauses_DonationCauseId] FOREIGN KEY ([DonationCauseId]) REFERENCES [dbo].[DonationCauses] ([Id])
    );
    CREATE NONCLUSTERED INDEX [IX_Donations_DonationCauseId] ON [dbo].[Donations] ([DonationCauseId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Donations_UserId] ON [dbo].[Donations] ([UserId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Donations_TransactionReference] ON [dbo].[Donations] ([TransactionReference] ASC);
    PRINT 'Table [Donations] created.';
END
GO

-- Table: Programmes
IF OBJECT_ID(N'[dbo].[Programmes]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Programmes] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Title] NVARCHAR(150) NOT NULL,
        [Category] NVARCHAR(100) NOT NULL,
        [ShortDescription] NVARCHAR(300) NOT NULL,
        [FullDescription] NVARCHAR(MAX) NOT NULL,
        [EventDate] DATETIME2 NOT NULL,
        [Location] NVARCHAR(200) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Programmes_Status] DEFAULT ('Upcoming'),
        [ImageUrl] NVARCHAR(300) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Programmes_IsActive] DEFAULT ((1)),
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Programmes_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Programmes] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [Programmes] created.';
END
GO

-- Table: ProgrammeInterests
IF OBJECT_ID(N'[dbo].[ProgrammeInterests]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProgrammeInterests] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ProgrammeId] INT NOT NULL,
        [UserId] INT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [Phone] NVARCHAR(20) NULL,
        [Message] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_ProgrammeInterests_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_ProgrammeInterests] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ProgrammeInterests_Programmes_ProgrammeId] FOREIGN KEY ([ProgrammeId]) REFERENCES [dbo].[Programmes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProgrammeInterests_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE SET NULL
    );
    CREATE NONCLUSTERED INDEX [IX_ProgrammeInterests_ProgrammeId] ON [dbo].[ProgrammeInterests] ([ProgrammeId] ASC);
    CREATE NONCLUSTERED INDEX [IX_ProgrammeInterests_UserId] ON [dbo].[ProgrammeInterests] ([UserId] ASC);
    PRINT 'Table [ProgrammeInterests] created.';
END
GO

-- Table: Partners
IF OBJECT_ID(N'[dbo].[Partners]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Partners] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(150) NOT NULL,
        [Category] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NOT NULL,
        [WebsiteUrl] NVARCHAR(250) NULL,
        [LogoUrl] NVARCHAR(300) NULL,
        [DisplayOrder] INT NOT NULL CONSTRAINT [DF_Partners_DisplayOrder] DEFAULT ((0)),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Partners_IsActive] DEFAULT ((1)),
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Partners_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Partners] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [Partners] created.';
END
GO

-- Table: GalleryItems
IF OBJECT_ID(N'[dbo].[GalleryItems]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[GalleryItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Title] NVARCHAR(150) NOT NULL,
        [Category] NVARCHAR(100) NOT NULL CONSTRAINT [DF_GalleryItems_Category] DEFAULT ('General'),
        [Description] NVARCHAR(500) NULL,
        [ImageUrl] NVARCHAR(300) NOT NULL,
        [DisplayOrder] INT NOT NULL CONSTRAINT [DF_GalleryItems_DisplayOrder] DEFAULT ((0)),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_GalleryItems_IsActive] DEFAULT ((1)),
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_GalleryItems_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_GalleryItems] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [GalleryItems] created.';
END
GO

-- Table: AboutSections (CRITICAL)
IF OBJECT_ID(N'[dbo].[AboutSections]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AboutSections] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [SectionKey] NVARCHAR(50) NOT NULL,
        [Title] NVARCHAR(150) NOT NULL,
        [Subtitle] NVARCHAR(200) NULL,
        [Content] NVARCHAR(MAX) NOT NULL,
        [ImageUrl] NVARCHAR(300) NULL,
        [DisplayOrder] INT NOT NULL CONSTRAINT [DF_AboutSections_DisplayOrder] DEFAULT ((0)),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_AboutSections_IsActive] DEFAULT ((1)),
        [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_AboutSections_UpdatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_AboutSections] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_AboutSections_SectionKey] UNIQUE NONCLUSTERED ([SectionKey] ASC)
    );
    PRINT 'Table [AboutSections] created.';
END
GO

-- Table: HelpItems
IF OBJECT_ID(N'[dbo].[HelpItems]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[HelpItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Question] NVARCHAR(250) NOT NULL,
        [Answer] NVARCHAR(MAX) NOT NULL,
        [Category] NVARCHAR(100) NOT NULL CONSTRAINT [DF_HelpItems_Category] DEFAULT ('General'),
        [DisplayOrder] INT NOT NULL CONSTRAINT [DF_HelpItems_DisplayOrder] DEFAULT ((0)),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_HelpItems_IsActive] DEFAULT ((1)),
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_HelpItems_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_HelpItems] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [HelpItems] created.';
END
GO

-- Table: UserQueries
IF OBJECT_ID(N'[dbo].[UserQueries]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserQueries] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NULL,
        [SenderName] NVARCHAR(100) NOT NULL,
        [SenderEmail] NVARCHAR(150) NOT NULL,
        [Subject] NVARCHAR(200) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [AdminResponse] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(30) NOT NULL CONSTRAINT [DF_UserQueries_Status] DEFAULT ('Pending'),
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_UserQueries_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [RespondedAt] DATETIME2 NULL,
        CONSTRAINT [PK_UserQueries] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_UserQueries_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE SET NULL
    );
    CREATE NONCLUSTERED INDEX [IX_UserQueries_UserId] ON [dbo].[UserQueries] ([UserId] ASC);
    PRINT 'Table [UserQueries] created.';
END
GO

-- Table: ContactMessages
IF OBJECT_ID(N'[dbo].[ContactMessages]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ContactMessages] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [Phone] NVARCHAR(20) NULL,
        [Subject] NVARCHAR(200) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [IsRead] BIT NOT NULL CONSTRAINT [DF_ContactMessages_IsRead] DEFAULT ((0)),
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_ContactMessages_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_ContactMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table [ContactMessages] created.';
END
GO

-- Table: Invitations
IF OBJECT_ID(N'[dbo].[Invitations]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Invitations] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [SenderUserId] INT NULL,
        [SenderName] NVARCHAR(100) NOT NULL,
        [FriendName] NVARCHAR(100) NOT NULL,
        [FriendEmail] NVARCHAR(150) NOT NULL,
        [PersonalMessage] NVARCHAR(500) NULL,
        [SentAt] DATETIME2 NOT NULL CONSTRAINT [DF_Invitations_SentAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Invitations] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Invitations_Users_SenderUserId] FOREIGN KEY ([SenderUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE SET NULL
    );
    CREATE NONCLUSTERED INDEX [IX_Invitations_SenderUserId] ON [dbo].[Invitations] ([SenderUserId] ASC);
    PRINT 'Table [Invitations] created.';
END
GO

-- Seed: SiteSettings
IF NOT EXISTS (SELECT 1 FROM [dbo].[SiteSettings])
BEGIN
    INSERT INTO [dbo].[SiteSettings] ([NgoName], [Tagline], [Email], [Phone], [Address], [CityStateZip], [FacebookUrl], [TwitterUrl], [InstagramUrl], [YouTubeUrl], [FooterText], [LogoUrl])
    VALUES (
        N'Give-AID',
        N'Empowering Communities, Transforming Lives',
        N'info@giveaid.org',
        N'+1 (555) 019-2834',
        N'742 Evergreen Terrace, Suite 100',
        N'Springfield, IL 62704',
        N'https://facebook.com',
        N'https://twitter.com',
        N'https://instagram.com',
        N'https://youtube.com',
        N'Give-AID is a certified non-profit humanitarian organization striving to alleviate poverty, empower marginalized groups, provide healthcare and education, and foster sustainable community development worldwide.',
        N'/images/logo.png'
    );
    PRINT 'SiteSettings seeded.';
END
GO

-- Seed: Admin Account (admin / Admin@123)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Admins] WHERE [Username] = N'admin')
BEGIN
    INSERT INTO [dbo].[Admins] ([Username], [Email], [FullName], [PasswordHash], [IsActive])
    VALUES (
        N'admin',
        N'admin@giveaid.org',
        N'Chief Administrator',
        N'SEED_ADMIN_HASH',
        1
    );
    PRINT 'Admin account [admin] seeded.';
END
GO

-- Seed: Demo Community User (john.doe@example.com / User@123)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = N'john.doe@example.com')
BEGIN
    INSERT INTO [dbo].[Users] ([FullName], [Email], [Phone], [Address], [City], [Profession], [PasswordHash], [IsActive])
    VALUES (
        N'Johnathan Doe',
        N'john.doe@example.com',
        N'+1 (555) 839-1029',
        N'120 Elmwood Avenue',
        N'Springfield',
        N'Software Engineer & Volunteer',
        N'SEED_USER_HASH',
        1
    );
    PRINT 'Demo User [john.doe@example.com] seeded.';
END
GO

-- Seed: All 7 About Us Subsections
IF NOT EXISTS (SELECT 1 FROM [dbo].[AboutSections])
BEGIN
    INSERT INTO [dbo].[AboutSections] ([SectionKey], [Title], [Subtitle], [Content], [ImageUrl], [DisplayOrder], [IsActive])
    VALUES
    (
        N'WhatWeDo',
        N'What We Do',
        N'Comprehensive community development and crisis intervention',
        N'At Give-AID, we design and deliver evidence-based interventions spanning child protection, accessible healthcare, vocational education, emergency disaster relief, and women empowerment. We partner directly with grass-roots community leaders to build self-sustaining ecosystems that break cyclical poverty and restore dignity.',
        N'/images/about-whatwedo.jpg',
        1,
        1
    ),
    (
        N'OurMission',
        N'Our Mission & Vision',
        N'Building an equitable world with opportunities for everyone',
        N'Our mission is to empower marginalized families and vulnerable communities by securing essential human rights: clean water, nutritious sustenance, high-quality education, healthcare access, and vocational self-sufficiency. We envision a society where every child dreams freely, every elder is cared for, and every family has the tools to thrive.',
        N'/images/about-mission.jpg',
        2,
        1
    ),
    (
        N'OurTeam',
        N'Our Leadership & Volunteers',
        N'Passionate changemakers and dedicated humanitarian professionals',
        N'Our interdisciplinary team consists of licensed social workers, pediatric healthcare practitioners, experienced disaster response coordinators, and thousands of dedicated volunteers. Together with our governing board, our staff adheres to the highest standards of financial transparency and ethical stewardship.',
        N'/images/about-team.jpg',
        3,
        1
    ),
    (
        N'CareerWithUs',
        N'Career With Us',
        N'Join our mission and make a lifelong positive impact',
        N'Are you passionate about social justice, international development, and humanitarian service? Give-AID offers dynamic full-time, fellowship, and volunteer opportunities across field operations, public health outreach, digital advocacy, and donor relations. We value empathy, innovative problem-solving, and collaborative spirit.',
        N'/images/about-career.jpg',
        4,
        1
    ),
    (
        N'OurAchievements',
        N'Our Key Milestones & Impact',
        N'Measurable outcomes achieved through donor generosity',
        N'Over the past decade, Give-AID has provided over 250,000 hot nutritious meals, built 38 solar-powered clean water filtration units in underserved villages, sponsored continuous schooling for 4,200 children, conducted 110 free mobile health clinics, and provided seed-funding to 1,500 women-led cottage businesses.',
        N'/images/about-achievements.jpg',
        5,
        1
    ),
    (
        N'OurSupporters',
        N'Our Supporters & Philanthropists',
        N'The generous hearts that fuel every single campaign',
        N'We are profoundly grateful to our individual donors, family trusts, international NGOs, and socially responsible corporations. Your generous contributions provide the lifeblood of our initiatives. Every dollar received is diligently audited and deployed directly to programmatic welfare.',
        N'/images/about-supporters.jpg',
        6,
        1
    ),
    (
        N'ReadAboutUs',
        N'Read About Us & Our Governance',
        N'Operating with uncompromising transparency and accountability',
        N'Give-AID is an independently audited 501(c)(3) equivalent NGO. Over 88% of all programmatic funds directly reach our beneficiaries in the field, with minimal overhead. We publish comprehensive annual impact reports, quarterly financial filings, and independent monitoring assessments for public scrutiny.',
        N'/images/about-read.jpg',
        7,
        1
    );
    PRINT 'All 7 AboutSections seeded.';
END
GO

-- Seed: Donation Causes
IF NOT EXISTS (SELECT 1 FROM [dbo].[DonationCauses])
BEGIN
    INSERT INTO [dbo].[DonationCauses] ([Name], [Category], [ShortDescription], [FullDescription], [TargetAmount], [RaisedAmount], [ImageUrl], [IsActive])
    VALUES
    (
        N'Children Welfare & Nutrition',
        N'Children',
        N'Providing essential daily nutrition, protective clothing, and nurturing shelter for vulnerable and orphaned children.',
        N'Malnutrition impairs cognitive growth and physical vitality. Through this campaign, Give-AID operates daily nutrition centres, emergency infant milk distribution, and seasonal clothing packages for children living in high-risk environments.',
        50000.00,
        32850.00,
        N'/images/cause-children.jpg',
        1
    ),
    (
        N'Education for Every Child',
        N'Education',
        N'Supplying school tuition fees, textbooks, digital tablets, and school uniforms to low-income students.',
        N'Education is the single most effective ladder out of intergenerational poverty. Your donations fund complete school sponsorship packages, qualified remedial teachers, classroom supplies, and computer labs for rural schools.',
        40000.00,
        26400.00,
        N'/images/cause-education.jpg',
        1
    ),
    (
        N'Disabled Persons Accessibility & Care',
        N'Disability',
        N'Distributing wheelchairs, prosthetic aids, hearing devices, and accessible skill training for disabled individuals.',
        N'People with physical and sensory disabilities face systemic barriers to employment and mobility. We provide custom assistive mobility devices, physical rehabilitation therapy, and specialized digital vocational skills.',
        35000.00,
        19200.00,
        N'/images/cause-disabled.jpg',
        1
    ),
    (
        N'Women Welfare & Artisan Micro-Grants',
        N'Women',
        N'Empowering single mothers and female artisans through vocational training, micro-grants, and legal support.',
        N'When women thrive, entire communities prosper. Give-AID provides vocational tailoring, handicraft training, business literacy classes, and zero-interest micro-grants to empower women into self-sufficient entrepreneurs.',
        45000.00,
        37100.00,
        N'/images/cause-women.jpg',
        1
    ),
    (
        N'Youth Development & Vocational Trades',
        N'Youth',
        N'Offering coding bootcamps, electrical/plumbing trade apprenticeships, and youth sports mentorship.',
        N'Unemployment among youths breeds despair. Our youth innovation hubs teach high-demand digital skills, certified trade mechanics, leadership workshops, and organized community sports leagues.',
        30000.00,
        14800.00,
        N'/images/cause-youth.jpg',
        1
    ),
    (
        N'Elderly Healthcare & Social Protection',
        N'Elderly',
        N'Ensuring routine chronic prescription medicines, nutritional support, and companionship visits for seniors.',
        N'Many elderly citizens lack family support or pension security. We deliver weekly door-to-door medical checkups, essential chronic medications (hypertension, diabetes), warm blankets, and community dining clubs.',
        25000.00,
        18950.00,
        N'/images/cause-elderly.jpg',
        1
    ),
    (
        N'Emergency Medical Relief & Clean Water',
        N'Healthcare',
        N'Installing deep community water boreholes and dispatching mobile medical clinics to crisis zones.',
        N'Waterborne diseases remain a leading cause of preventable illness. This fund installs deep-well solar boreholes providing safe drinking water to thousands, alongside mobile doctor clinics dispensing life-saving medicine.',
        60000.00,
        48500.00,
        N'/images/cause-water.jpg',
        1
    );
    PRINT 'DonationCauses seeded.';
END
GO

-- Seed: Programmes
IF NOT EXISTS (SELECT 1 FROM [dbo].[Programmes])
BEGIN
    INSERT INTO [dbo].[Programmes] ([Title], [Category], [ShortDescription], [FullDescription], [EventDate], [Location], [Status], [ImageUrl], [IsActive])
    VALUES
    (
        N'Annual Winter Warmth Drive 2026',
        N'Relief',
        N'Community distribution of 5,000 thermal blankets, winter jackets, and heaters to unhoused families.',
        N'As temperatures plummet, vulnerable homeless citizens and under-insulated households face severe hypothermia risks. Give-AID volunteers distribute heavy-duty thermal fleece blankets, thermal socks, winter coats, and hot soup across key urban and peri-urban hubs.',
        DATEADD(DAY, 25, SYSUTCDATETIME()),
        N'Metro Community Center, North Wing, Springfield',
        N'Upcoming',
        N'/images/prog-winter.jpg',
        1
    ),
    (
        N'Mobile Health & Free Eye Care Clinic',
        N'Healthcare',
        N'Free medical consultations, prescription optical glasses, and diabetes screenings for 1,200 residents.',
        N'A team of 15 volunteer physicians, optometrists, and registered nurses will conduct comprehensive triage, pediatric examinations, cataract evaluations, and distribute free corrective eyeglasses and antibiotics.',
        DATEADD(DAY, 40, SYSUTCDATETIME()),
        N'Riverside High School Gymnasium, Springfield',
        N'Upcoming',
        N'/images/prog-health.jpg',
        1
    ),
    (
        N'Clean Water for Rural Schools Initiative',
        N'Environment',
        N'Commissioning of 8 new solar-powered ultrafiltration clean drinking water stations in public schools.',
        N'Access to clean water drastically cuts student absenteeism. We are hosting the ribbon-cutting and hygiene training workshops for faculty, parents, and over 3,000 schoolchildren.',
        DATEADD(DAY, 55, SYSUTCDATETIME()),
        N'Green Valley Educational District, Sector 4',
        N'Upcoming',
        N'/images/prog-water.jpg',
        1
    ),
    (
        N'Youth Digital Literacy & Coding Bootcamp',
        N'Education',
        N'A free 6-week weekend training in web programming and computer basics for disadvantaged youth.',
        N'Equipping young adults aged 16-24 with practical skills in HTML, CSS, JavaScript, and computer diagnostics to jumpstart their career prospects in the modern economy.',
        DATEADD(DAY, 70, SYSUTCDATETIME()),
        N'Give-AID Tech Hub, 3rd Floor Auditorium',
        N'Upcoming',
        N'/images/prog-coding.jpg',
        1
    ),
    (
        N'Women Artisan Skill & Micro-Business Fair',
        N'Women Welfare',
        N'Exhibition showcasing handmade crafts, textiles, and organic foods produced by our women trainees.',
        N'Celebrate and purchase exquisite hand-loomed goods, embroidery, pottery, and culinary treats directly from women entrepreneurs enrolled in our livelihood empowerment programs.',
        DATEADD(DAY, 85, SYSUTCDATETIME()),
        N'Centennial City Plaza, Pavilion B',
        N'Upcoming',
        N'/images/prog-women.jpg',
        1
    ),
    (
        N'Weekend Food Relief & Community Kitchen',
        N'Relief',
        N'Serving 800 freshly cooked warm meals and distributing grocery survival packs to elderly pensioners.',
        N'Join our chef teams and distribution volunteers as we prepare, package, and deliver nutrient-dense hot meals and monthly pantry dry goods to senior housing complexes.',
        DATEADD(DAY, 12, SYSUTCDATETIME()),
        N'Hope Baptist Hall, East Springfield',
        N'Upcoming',
        N'/images/prog-food.jpg',
        1
    );
    PRINT 'Programmes seeded.';
END
GO

-- Seed: Partners
IF NOT EXISTS (SELECT 1 FROM [dbo].[Partners])
BEGIN
    INSERT INTO [dbo].[Partners] ([Name], [Category], [Description], [WebsiteUrl], [LogoUrl], [DisplayOrder], [IsActive])
    VALUES
    (
        N'Global Health Outreach Alliance',
        N'NGO / Healthcare',
        N'International medical consortium collaborating on pharmaceutical supplies, triage equipment, and volunteer doctor delegations.',
        N'https://example.org/global-health',
        N'/images/partner-gho.png',
        1,
        1
    ),
    (
        N'Beacon Education Foundation',
        N'Academic Foundation',
        N'Philanthropic endowment supplying curriculum licenses, classroom books, and digital educational hardware.',
        N'https://example.org/beacon-education',
        N'/images/partner-beacon.png',
        2,
        1
    ),
    (
        N'Apex Community Bank CSR',
        N'Corporate Partner',
        N'Providing dollar-for-dollar employee matching grants, financial literacy workshops, and micro-business credit lines.',
        N'https://example.org/apex-bank',
        N'/images/partner-apex.png',
        3,
        1
    ),
    (
        N'CleanWorld Water Initiative',
        N'Environmental NGO',
        N'Specializing in geological groundwater survey, solar pump installations, and village sanitation training.',
        N'https://example.org/clean-world',
        N'/images/partner-cleanworld.png',
        4,
        1
    ),
    (
        N'Horizon Tech Innovations',
        N'Corporate Tech Sponsor',
        N'Donating refurbished laptops, cloud services, and software mentorship for our youth coding bootcamps.',
        N'https://example.org/horizon-tech',
        N'/images/partner-horizon.png',
        5,
        1
    ),
    (
        N'United Humanity Network',
        N'International Aid Federation',
        N'Rapid disaster response partner facilitating logistics and international air freight during regional crises.',
        N'https://example.org/united-humanity',
        N'/images/partner-humanity.png',
        6,
        1
    );
    PRINT 'Partners seeded.';
END
GO

-- Seed: HelpItems FAQs
IF NOT EXISTS (SELECT 1 FROM [dbo].[HelpItems])
BEGIN
    INSERT INTO [dbo].[HelpItems] ([Question], [Answer], [Category], [DisplayOrder], [IsActive])
    VALUES
    (
        N'How does Give-AID ensure my donation reaches those in need?',
        N'Give-AID practices strict fiduciary transparency. Over 88% of all funds are directed straight into active field programmes. Our financial statements are audited by independent certified public accountants and filed publicly annually.',
        N'Donations',
        1,
        1
    ),
    (
        N'Is my online donation secure?',
        N'Yes! We utilize 256-bit SSL encryption. In this demonstration environment, card processing is fully simulated—no actual card numbers or CVVs are permanently stored on our servers, ensuring absolute privacy.',
        N'Donations',
        2,
        1
    ),
    (
        N'How do I volunteer or participate in upcoming programmes?',
        N'Navigate to our ''Programmes'' page, select any upcoming initiative that resonates with you, and click ''Participate / I am Interested''. Fill in your details and our volunteer coordinator will contact you with orientation schedules.',
        N'Volunteering',
        3,
        1
    ),
    (
        N'Can I receive a tax deduction certificate for my contributions?',
        N'Yes! All registered donors receive an automated digital transaction receipt immediately upon successful payment. At year-end, registered members can view and download their consolidated annual giving statement in their profile.',
        N'Donations',
        4,
        1
    ),
    (
        N'How do I register an account as a community member?',
        N'Click ''Register'' in the top navigation bar, complete the brief form with your contact details, and submit. You will immediately be able to log in, track your donations, raise support queries, and RSVP to events.',
        N'Account',
        5,
        1
    ),
    (
        N'How can our company become an official Give-AID Partner?',
        N'We welcome CSR collaborations, grant partnerships, and employee giving drives! Please contact our partnerships director via the Contact Us form or email us directly at partners@giveaid.org.',
        N'Partnership',
        6,
        1
    ),
    (
        N'How can I raise a query or request assistance?',
        N'Registered members can log into their account and click ''Queries'' in the navbar to open a formal inquiry. You can track the status in real time as our administration reviews and responds to your query.',
        N'Support',
        7,
        1
    ),
    (
        N'Can I invite my friends and colleagues to join Give-AID?',
        N'Yes! Use our ''Invite Friends'' tool accessible from the main navigation. Simply provide your friend''s name and email along with a personalized note to introduce them to our humanitarian community.',
        N'Community',
        8,
        1
    );
    PRINT 'HelpItems FAQs seeded.';
END
GO

-- Seed: GalleryItems
IF NOT EXISTS (SELECT 1 FROM [dbo].[GalleryItems])
BEGIN
    INSERT INTO [dbo].[GalleryItems] ([Title], [Category], [Description], [ImageUrl], [DisplayOrder], [IsActive])
    VALUES
    (
        N'Child Nutrition Center Daily Meal Distribution',
        N'Children',
        N'Volunteers serving hot fortified meals to 300+ primary school students.',
        N'/images/gallery-1.jpg',
        1,
        1
    ),
    (
        N'Solar Clean Water Filtration Commissioning',
        N'Water',
        N'Villagers celebrating the turning on of their first clean tap water borehole.',
        N'/images/gallery-2.jpg',
        2,
        1
    ),
    (
        N'Youth Robotics & Computer Skills Workshop',
        N'Education',
        N'High school students collaborating on beginner robotics kits.',
        N'/images/gallery-3.jpg',
        3,
        1
    ),
    (
        N'Mobile Eyecare & Reading Glasses Camp',
        N'Healthcare',
        N'Optometrist fitting customized reading glasses for an elderly grandmother.',
        N'/images/gallery-4.jpg',
        4,
        1
    ),
    (
        N'Women Weaving & Textile Cooperative',
        N'Women',
        N'Artisans creating traditional fabrics funded through our micro-grant program.',
        N'/images/gallery-5.jpg',
        5,
        1
    ),
    (
        N'Winter Blanket & Thermal Kit Packaging',
        N'Relief',
        N'Over 1,000 thermal supply bundles packaged by enthusiastic volunteers.',
        N'/images/gallery-6.jpg',
        6,
        1
    );
    PRINT 'GalleryItems seeded.';
END
GO

-- Seed: Sample Donations
IF NOT EXISTS (SELECT 1 FROM [dbo].[Donations])
BEGIN
    DECLARE @CauseId INT = (SELECT TOP 1 [Id] FROM [dbo].[DonationCauses] WHERE [Category] = 'Children');
    DECLARE @UserId INT = (SELECT TOP 1 [Id] FROM [dbo].[Users] WHERE [Email] = 'john.doe@example.com');

    IF @CauseId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Donations] 
        ([UserId], [DonationCauseId], [Amount], [DonorName], [DonorEmail], [DonorPhone], [CardHolderName], [MaskedCardNumber], [ExpiryMonth], [ExpiryYear], [TransactionReference], [PaymentStatus], [DonationDate], [Notes])
        VALUES
        (
            @UserId,
            @CauseId,
            250.00,
            N'Johnathan Doe',
            N'john.doe@example.com',
            N'+1 (555) 839-1029',
            N'JOHNATHAN DOE',
            N'************4242',
            N'12',
            N'2027',
            N'GVAID-2026-100234',
            N'Successful',
            DATEADD(DAY, -5, SYSUTCDATETIME()),
            N'In honor of all dedicated teachers and children.'
        ),
        (
            NULL,
            @CauseId,
            100.00,
            N'Sarah Jenkins',
            N'sarah.j@example.org',
            N'+1 (555) 321-9988',
            N'SARAH JENKINS',
            N'************8812',
            N'08',
            N'2028',
            N'GVAID-2026-100235',
            N'Successful',
            DATEADD(DAY, -3, SYSUTCDATETIME()),
            N'Keep up the wonderful educational and nutritional work!'
        );
        PRINT 'Sample Donations seeded.';
    END
END
GO

-- Seed: Sample User Query
IF NOT EXISTS (SELECT 1 FROM [dbo].[UserQueries])
BEGIN
    DECLARE @UId INT = (SELECT TOP 1 [Id] FROM [dbo].[Users] WHERE [Email] = 'john.doe@example.com');
    INSERT INTO [dbo].[UserQueries] 
    ([UserId], [SenderName], [SenderEmail], [Subject], [Message], [AdminResponse], [Status], [CreatedAt], [RespondedAt])
    VALUES
    (
        @UId,
        N'Johnathan Doe',
        N'john.doe@example.com',
        N'Corporate Matching for Donations',
        N'Hello Give-AID team, my employer matches charitable donations 1:1. Where can I email our corporate match form so the donation to the Children Welfare campaign is doubled?',
        N'Hello Johnathan! Thank you so much for your generosity. You can email the matching form directly to finance@giveaid.org with your transaction reference GVAID-2026-100234, and our treasury team will sign and return it within 24 hours.',
        N'Resolved',
        DATEADD(DAY, -4, SYSUTCDATETIME()),
        DATEADD(DAY, -3, SYSUTCDATETIME())
    );
    PRINT 'Sample UserQuery seeded.';
END
GO
