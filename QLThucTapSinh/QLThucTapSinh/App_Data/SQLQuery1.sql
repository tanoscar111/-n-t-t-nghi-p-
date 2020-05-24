CREATE TABLE Role(
	RoleID INT IDENTITY (1,1) NOT NULL,
	NameRole NVARCHAR(250) NULL,
	PRIMARY KEY (RoleID)
);

CREATE TABLE Menu(
	Id INT IDENTITY (1,1) NOT NULL,
	TextMenu NVARCHAR(250) NULL,
	Link VARCHAR(250) NULL,
	RoleID INT NULL,
	PRIMARY KEY(Id),
	CONSTRAINT FK_Menu_Role FOREIGN KEY (RoleID) REFERENCES dbo.Role(RoleID)
);

--Thêm Ngày gia hạn, hạn sử dụng, status--
CREATE TABLE Organization(
	ID VARCHAR(8) NOT NULL,
	Name NVARCHAR(500) NULL,
	Address NVARCHAR(500) NULL,
	Phone VARCHAR(11) NULL,
	Fax VARCHAR(11) NULL,
	Image VARCHAR(250) NULL,
	Logo VARCHAR(250) NULL,
	Note NVARCHAR(500) NULL,
	Email VARCHAR(250) NULL,
	StartDay DATETIME NULL,
	ExpiryDate INT NULL,
	Status BIT DEFAULT (0) NULL,
	SendEmail BIT NULL,
	PRIMARY KEY(ID)
);

CREATE TABLE Person(
	PersonID VARCHAR(8)  NOT NULL,
	LastName NVARCHAR(50) NULL,
	FirstName NVARCHAR(50) NULL,
	Birthday DATE NULL,
	Gender BIT NULL,
	Address NVARCHAR(500) NULL,
	Phone VARCHAR(10) NULL,
	Email VARCHAR(250) NULL,
	Image VARCHAR(250) NULL,
	RoleID INT NULL,
	CompanyID VARCHAR(8) NULL,
	SchoolID VARCHAR(8) NULL,
	PRIMARY KEY(PersonID),
	CONSTRAINT FK_Person_Role FOREIGN KEY (RoleID) REFERENCES dbo.Role(RoleID),
	CONSTRAINT FK_Person_Company FOREIGN KEY (CompanyID) REFERENCES dbo.Organization(ID),
	CONSTRAINT FK_Person_School FOREIGN KEY (SchoolID) REFERENCES dbo.Organization(ID)
);

CREATE TABLE Users(
	UserName Varchar(50) NOT NULL,
	PersonID VARCHAR(8) UNIQUE FOREIGN KEY REFERENCES dbo.Person(PersonID),
	PassWord VARCHAR(50) NULL,
	Status BIT DEFAULT (0) NULL,
	PRIMARY KEY(UserName),
	
);

CREATE TABLE InternShip(
	InternshipID INT NOT NULL,
	CourseName NVARCHAR(250) NULL,
	Note NVARCHAR(500) NULL,
	PersonID VARCHAR(8) NULL,
	CompanyID VARCHAR(8) NULL,
	StartDay DATETIME NULL,
	ExpiryDate INT NULL,
	Status BIT DEFAULT (0) NULL,
	PRIMARY KEY (InternshipID),
	CONSTRAINT FK_InternShip_Person FOREIGN KEY (PersonID) REFERENCES dbo.Person(PersonID),
	CONSTRAINT FK_InternShip_Company FOREIGN KEY (CompanyID) REFERENCES dbo.Organization(ID)

);

CREATE TABLE Task(
	TaskID INT NOT NULL,
	TaskName NVARCHAR(250) NULL,
	Note NVARCHAR(500) NULL,
	Video VARCHAR(250) NULL,
	PersonID VARCHAR(8) NULL,
	PRIMARY KEY (TaskID),
	CONSTRAINT FK_Task_Person FOREIGN KEY (PersonID) REFERENCES dbo.Person(PersonID));

CREATE TABLE IntershipWithTask(
	ID INT NOT NULL,
	InternshipID INT NULL,
	TaskID INT NULL,
	SORT INT NULL,
	PRIMARY KEY (ID),
	CONSTRAINT FK_IntershipWithTask_Task FOREIGN KEY (TaskID) REFERENCES dbo.Task(TaskID),
	CONSTRAINT FK_IntershipWithTask_Internship FOREIGN KEY (InternshipID) REFERENCES dbo.InternShip(InternshipID)
);

CREATE TABLE Question(
	QuestionID INT NOT NULL,
	TaskID INT NULL,
	Content NVARCHAR(500) NULL,
	Answer VARCHAR(10) NULL,
	A NVARCHAR(300) NULL,
	B NVARCHAR(300) NULL,
	C NVARCHAR(300) NULL,
	D NVARCHAR(300) NULL,
	PRIMARY KEY(QuestionID),
	CONSTRAINT FK_Question_Task FOREIGN KEY (TaskID) REFERENCES dbo.Task(TaskID)
);

--Thêm mã sinh viên--
CREATE TABLE Intern(
	PersonID VARCHAR(8) UNIQUE FOREIGN KEY (PersonID) REFERENCES dbo.Person(PersonID),
	StudentCode VARCHAR(15) NULL,
	InternshipID INT NULL,
	Result INT NULL,
	PRIMARY KEY (PersonID),
	CONSTRAINT FK_Intern_InternShip FOREIGN KEY (InternshipID) REFERENCES dbo.InternShip(InternshipID),
);

CREATE TABLE TestResults(
	ID INT NOT NULL,
	PersonID VARCHAR(8) NULL,
	TaskID INT NULL,
	Answer INT NULL,
	PRIMARY KEY(ID),
	CONSTRAINT FK_TestResults_Task FOREIGN KEY (TaskID) REFERENCES dbo.Task(TaskID),
	CONSTRAINT FK_TestResults_Intern FOREIGN KEY (PersonID) REFERENCES dbo.Intern(PersonID)
	
);

INSERT INTO dbo.Role(NameRole) VALUES  ('Admin' ), ('Manager' ), ('Representative'), ('Ledder'),('Interns');

INSERT INTO dbo.Menu(TextMenu,Link,RoleID)VALUES (N'Quản lý Công ty','/QLCompany/Index', 1),
	(N'Quản lý Nhà trường','/QLSchool/Index', 1),
	(N'Thông tin Cá nhân','/QLPerson/TTcanhan1', 1),
	(N'Báo cáo & Thống kê', NULL, 1),
	(N'Thông tin Cá nhân', '/QLPerson/TTcanhan1', 2),
	(N'Quản lý Thực tập sinh', '/QLIntern/Index', 2),
	(N'Quản lý Ledder', NULL, 2),
	(N'Quản lý Khóa học','/QLInternship/Index', 2),
	(N'Quản lý Bài học', '/QLTask/Index', 2),
	(N'Quản lý Câu hỏi', '/QLQuestion/Index', 2),
	(N'Thông tin Công ty', NULL, 2),
	(N'Thông tin Cá nhân', '/QLPerson/TTcanhan1', 3),
	(N'Quản lý Thực tập sinh', '/QLIntern/Index', 3),	
	(N'Thông tin Khoa - Trường', NULL, 3),
	(N'Thông tin Cá nhân', '/QLPerson/TTcanhan1', 4),
	(N'Quản lý Thực tập sinh', '/QLIntern/Index', 4),
	(N'Quản lý Khóa học','/QLInternship/Index', 4),
	(N'Quản lý Bài học', '/QLTask/Index', 4),
	(N'Quản lý Câu hỏi', '/QLQuestion/Index', 4),
	(N'Thông tin Cá nhân', '/QLPerson/TTcanhan1', 5),
	(N'Khóa học', '/QLTest/Index', 5),
	(N'Kết quả Kiểm tra', NULL, 5);

INSERT INTO dbo.Person(PersonID,RoleID) VALUES ('ZXCVBNML',1);	

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('Admin','21232f297a57a5a743894a0e4a801fc3','ZXCVBNML',1);

--Thêm Company --
INSERT INTO dbo.Organization (ID,Name,Address,StartDay)VALUES(   'QWERTFGH', N'Công ty AXon', N'Đường 2 tháng 9',GETDATE());

--Thêm tài khoản Manager--
INSERT INTO dbo.Person(PersonID,RoleID,CompanyID) VALUES ('ZXCVBHML',2,'QWERTFGH');	

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('Axondn','692b1ef6188549a424778fd18296189d','ZXCVBHML',1);

--Thêm tài khoản Ledder --
INSERT INTO dbo.Person(PersonID,RoleID,CompanyID) VALUES ('ZXCKBHML',4,'QWERTFGH');	

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('tanledder','1cc94f1121d696407d7ea2fee5b5814b','ZXCKBHML',1);

--Thêm Company --
INSERT INTO dbo.Organization (ID,Name,Address,StartDay)VALUES(   'QWETTFGH', N'Công ty FPT', N'Đường 30 tháng 4',GETDATE());

--Thêm tài khoản Manager--
INSERT INTO dbo.Person(PersonID,RoleID,CompanyID) VALUES ('ZXDVBHML',2,'QWETTFGH');	

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('fptdanang','d96ec39d987dbbea01c152b0a91167c2','ZXDVBHML',1);

-- Thêm School --
INSERT INTO dbo.Organization (ID,Name,Address,Phone,Fax,StartDay,ExpiryDate)VALUES (   'ASDFHHJR', N'Đại Học Kiến trúc', N'Nguyễn Văn Linh','0905342438', '02363865976',GETDATE(),1);

--Thêm tài khoản School--
INSERT INTO dbo.Person(PersonID,RoleID,SchoolID) VALUES ('ZXCVBUNL',3,'ASDFHHJR');	

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('Kientrucdn','6ddeae397bea1b2b71dfc7ed0e75575d','ZXCVBUNL',1);

-- Thêm School --
INSERT INTO dbo.Organization (ID,Name,Address,Phone,Fax,StartDay,ExpiryDate)VALUES (   'ASDFGHJR', N'Duy Tân', N'Nguyễn Văn Linh','0905342438', '02363865976',GETDATE(),1);

--Thêm tài khoản School--
INSERT INTO dbo.Person(PersonID,RoleID,SchoolID) VALUES ('ZXCVBUML',3,'ASDFGHJR');	

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('Duytandn','f05fb3a7afdc99a87fcf1c655c5b3d41','ZXCVBUML',1);

---Thêm Thực tập sinh--
INSERT INTO dbo.Person(PersonID,RoleID,SchoolID,CompanyID) VALUES ('ZXCDBUNQ',5,'ASDFGHJR','QWERTFGH');

INSERT INTO dbo.Intern(PersonID,StudentCode, Result)VALUES('ZXCDBUNQ','2221128202',0);

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('Lanhhuynh','9457997cd3a7d08f6482de2511fe9754','ZXCDBUNQ',1);



-- Thêm câu hỏi --
INSERT INTO dbo.InternShip(InternshipID,CourseName,PersonID,StartDay, ExpiryDate, Status)VALUES(1,N'C# Căn bản và nâng cao','ZXCKBHML',GETDATE(),1,1);

