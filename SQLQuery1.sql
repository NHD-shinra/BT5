﻿CREATE DATABASE QuanLySinhVien

USE QuanLySinhVien

CREATE TABLE Faculty (
    FacultyID int NOT NULL PRIMARY KEY, 
    FacultyName nvarchar(200) NOT NULL
);
GO

CREATE TABLE Student (
    StudentID nvarchar(20) NOT NULL PRIMARY KEY, 
    FullName nvarchar(200) NOT NULL,
    AverageScore float NOT NULL,
    FacultyID int NOT NULL,
    -- Tạo mối quan hệ (Khóa ngoại)
    FOREIGN KEY (FacultyID) REFERENCES Faculty(FacultyID)
);
GO

INSERT INTO Faculty (FacultyID, FacultyName) VALUES (1, N'Công Nghệ Thông Tin');
INSERT INTO Faculty (FacultyID, FacultyName) VALUES (2, N'Ngôn Ngữ Anh');
INSERT INTO Faculty (FacultyID, FacultyName) VALUES (3, N'Quản trị kinh doanh');

INSERT INTO Student (StudentID, FullName, AverageScore, FacultyID) 
VALUES (N'1611061916', N'Nguyễn Trần Hoàng Lan', 4.5, 1);
INSERT INTO Student (StudentID, FullName, AverageScore, FacultyID) 
VALUES (N'1711060596', N'Đàm Minh Đức', 2.5, 1);
INSERT INTO Student (StudentID, FullName, AverageScore, FacultyID) 
VALUES (N'1711061004', N'Nguyễn Quốc An', 10, 2);

select * from Faculty