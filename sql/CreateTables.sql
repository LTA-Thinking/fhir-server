create table dbo.ResourceTypes (
	Id int PRIMARY KEY,
	Name varchar(64)
)

create table dbo.Resources (
	ResourceTypeId INT REFERENCES ResourceTypes(Id),
	ResourceId varchar(256),
	VersionNumber INT,
	Resource varchar(max),
	IsDeleted bit,
	PRIMARY KEY (ResourceTypeId, ResourceId, VersionNumber)
)