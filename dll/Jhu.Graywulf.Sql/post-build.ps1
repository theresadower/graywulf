﻿param(
	[string]$SolutionDir,
	[string]$SolutionName,
	[string]$ProjectDir,
	[string]$ProjectName,
	[string]$OutDir,
	[string]$TargetName
)

cp $ProjectDir$OutDir$TargetName.dll $SolutionDir$OutDir
cp $ProjectDir$OutDir$TargetName.pdb $SolutionDir$OutDir
cp ${ProjectDir}${OutDir}MySql.Data.dll $SolutionDir$OutDir
cp ${ProjectDir}${OutDir}Npgsql.dll $SolutionDir$OutDir