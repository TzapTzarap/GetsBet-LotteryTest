USE [master]
GO
/****** Object:  Database [LotteryDb]    Script Date: 8/21/2022 10:32:20 PM ******/
CREATE DATABASE [LotteryDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'LotteryDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\LotteryDb.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'LotteryDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\LotteryDb_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [LotteryDb] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [LotteryDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [LotteryDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [LotteryDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [LotteryDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [LotteryDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [LotteryDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [LotteryDb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [LotteryDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [LotteryDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [LotteryDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [LotteryDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [LotteryDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [LotteryDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [LotteryDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [LotteryDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [LotteryDb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [LotteryDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [LotteryDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [LotteryDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [LotteryDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [LotteryDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [LotteryDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [LotteryDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [LotteryDb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [LotteryDb] SET  MULTI_USER 
GO
ALTER DATABASE [LotteryDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [LotteryDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [LotteryDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [LotteryDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [LotteryDb] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [LotteryDb] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [LotteryDb] SET QUERY_STORE = OFF
GO
USE [LotteryDb]
GO
/****** Object:  Table [dbo].[LotteryDraws]    Script Date: 8/21/2022 10:32:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LotteryDraws](
	[UnixDrawTime] [bigint] NOT NULL,
	[WinningNumbers] [varchar](150) NOT NULL,
	[LocalDrawTime] [datetime] NOT NULL,
 CONSTRAINT [PK_LotteryDraws] PRIMARY KEY CLUSTERED 
(
	[UnixDrawTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spAddDrawResult]    Script Date: 8/21/2022 10:32:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spAddDrawResult] (@DrawTime BIGINT,
								@WinningNumbers VARCHAR(150),
								@LocalDrawTime DATETIME)


AS 
SET NOCOUNT ON

BEGIN

 INSERT INTO [dbo].LotteryDraws (unixDrawTime, 
								 WinningNumbers, 
								 LocalDrawTime)
 VALUES (@DrawTime,
		@WinningNumbers, 
		@LocalDrawTime)


END
GO
/****** Object:  StoredProcedure [dbo].[spCheckIfDrawExists]    Script Date: 8/21/2022 10:32:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[spCheckIfDrawExists] (@DrawTime BIGINT)
								 


AS 

DECLARE @DrawExists bit
SET @DrawExists = 0


IF EXISTS (SELECT UnixDrawTime FROM LotteryDraws WHERE UnixDrawTime = @DrawTime)
BEGIN 
SET @DrawExists	= 1
END



Select @DrawExists
GO
/****** Object:  StoredProcedure [dbo].[spGetDrawsResults]    Script Date: 8/21/2022 10:32:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spGetDrawsResults] (@DrawDate DATETIME)
								 
AS


BEGIN

SELECT UnixDrawTime,WinningNumbers FROM LotteryDraws WHERE CAST(LocalDrawTime as date) =  @DrawDate

END

GO
/****** Object:  StoredProcedure [dbo].[spGetFrequentNumbers]    Script Date: 8/21/2022 10:32:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spGetFrequentNumbers] (@DrawDate DATETIME)
								 
AS

BEGIN



IF OBJECT_ID('tempdb..#Draws') IS NOT NULL DROP TABLE #Draws
 
 
CREATE TABLE #Draws
(
    [Id] [INT] IDENTITY(1,1) NOT NULL,
    [WiningNumbers] Varchar(150) 
)



INSERT INTO #Draws  (WiningNumbers)
SELECT WinningNumbers FROM [dbo].LotteryDraws WHERE CAST(LocalDrawTime as date) =  @DrawDate


IF OBJECT_ID('tempdb..#AllNumbers') IS NOT NULL DROP TABLE #AllNumbers
 
CREATE TABLE #AllNumbers
(
    [Id] [INT] IDENTITY(1,1) NOT NULL,
    [Number] INT 
)


DECLARE @total_draws INT

SET @total_draws = (SELECT max(Id) FROM #Draws)

DECLARE @draws_counter INT
SET @draws_counter = 1



WHILE(@total_draws >= @draws_counter)
	BEGIN

		INSERT INTO #AllNumbers (Number)
				SELECT VALUE FROM string_split((SELECT WiningNumbers FROM #Draws WHERE Id = @draws_counter), ',')
		
		SET @draws_counter  = @draws_counter + 1

	END


-- most frequent
SELECT TOP 3 Number as least_frequent , count(Number) AS Frequency FROM #AllNumbers GROUP BY Number ORDER BY Frequency 

-- least frequent
SELECT TOP 3 Number as most_frequent, count(Number) AS Frequency FROM #AllNumbers GROUP BY Number ORDER BY Frequency DESC



END 
GO
USE [master]
GO
ALTER DATABASE [LotteryDb] SET  READ_WRITE 
GO
