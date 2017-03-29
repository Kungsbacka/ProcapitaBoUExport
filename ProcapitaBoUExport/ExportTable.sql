CREATE TABLE [dbo].[ProcapitaBoUExport](
	[personnr] [nvarchar](12) NULL,
	[fornamn] [nvarchar](100) NULL,
	[efternamn] [nvarchar](100) NULL,
	[fullnamn] [nvarchar](100) NULL,
	[roll] [nvarchar](100) NULL,
	[rektorsomrade] [nvarchar](100) NULL,
	[rektorsomradeid] [nvarchar](100) NULL,
	[enhet] [nvarchar](100) NULL,
	[enhetid] [nvarchar](100) NULL,
	[skolform] [nvarchar](100) NULL,
	[skolformid] [nvarchar](100) NULL,
	[skolformkort] [nvarchar](100) NULL,
	[avdelning] [nvarchar](100) NULL,
	[avdelningid] [nvarchar](100) NULL,
	[klass] [nvarchar](100) NULL,
	[klassid] [nvarchar](100) NULL,
	[klassperiod] [nvarchar](100) NULL,
	[grupp] [nvarchar](100) NULL,
	[gruppid] [nvarchar](100) NULL,
	[grupperiod] [nvarchar](100) NULL,
	[amne] [nvarchar](100) NULL,
	[amneid] [nvarchar](100) NULL,
	[arskurs] [nvarchar](100) NULL,
	[gatuadress] [nvarchar](100) NULL,
	[postnummer] [nvarchar](100) NULL,
	[postort] [nvarchar](100) NULL,
	[vh1] [nvarchar](12) NULL,
	[vh2] [nvarchar](12) NULL,
	[uttagsdatum] [date] NOT NULL
);