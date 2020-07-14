@echo off

msbuild /t:Clean /t:Build ^
	/p:Configuration=Release ^
	/p:AllowedReferenceRelatedFileExtensions=none

mkdir bin

copy Packer\bin\Release\Packer.exe     bin\Packer.exe
copy Unpacker\bin\Release\Unpacker.exe bin\Unpacker.exe
copy Patcher\bin\Release\Patcher.exe   bin\Patcher.exe