create database ChitChatChew;
use ChitChatChew;
create table Users
(
	id int primary key identity(1,1),
	username nvarchar(MAX) not null,
	pw varchar(MAX) not null,
	firstName nvarchar(MAX),
	lastName nvarchar(max),
	email nvarchar(max),
	phone nvarchar(max),
	profilePicture nvarchar(max),
	verificationCode varchar(6),
	createdAt datetime default GETDATE(),
	isActive bit default (0),
	isBusy bit default (0),
	statusText varchar(max),
	isAdmin bit default (0)
)

create table UserFriends(
	id int foreign key references Users(id),
	friendId int foreign key references Users(id),
	primary key(id, friendId)
)

create table UserBlocks(
	id int foreign key references Users(id),
	blockedId int foreign key references Users(id)
	primary key(id, blockedId)
)

create table Groups(
	id int primary key identity(1,1),
	groupName nvarchar(20) not null,
	createdBy int foreign key references Users(id),
	isPublic bit default (0)
)


create table GroupUsers(
	groupId int foreign key references Groups(id),
	userId int foreign key references Users(id),
	primary key(groupId, userId)
)

create table Images(
	id int primary key identity(1,1),
	imageUrl nvarchar(max) not null,
	createdAt datetime default GETDATE(),
	createdBy int foreign key references Users(id)
)

create table Files(
	id int primary key identity(1,1),
	fileUrl nvarchar(max) not null,
	createdAt datetime default GETDATE(),
	createdBy int foreign key references Users(id)
)

create table Gifs(
	id int primary key identity(1,1),
	gifName nvarchar(max) not null,
	gifUrl nvarchar(max) not null
)

create table Stickers(
	id int primary key identity(1,1),
	stickerName nvarchar(max) not null,
	stickerUrl nvarchar(max) not null
)

create table Messages(
	id int primary key identity(1,1),
	senderId int foreign key references Users(id) not null,
	messageText nvarchar(max),
	messageImg int foreign key references Images(id),
	messageGif int foreign key references Gifs(id),
	messageFile int foreign key references Files(id),
	messageSticker int foreign key references Stickers(id),
	createdAt datetime 
)

create table GroupMessages(
	groupId int foreign key references Groups(id),
	messageId int foreign key references Messages(id),
	primary key(groupId, messageId)
)

create table ContactTickets(
	id int primary key identity(1,1),
	senderName nvarchar(max) not null,
	senderEmail varchar(max) not null,
	senderPhone varchar(max) not null,
	ticketText nvarchar(max) not null
)





