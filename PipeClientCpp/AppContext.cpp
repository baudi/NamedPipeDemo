#include "stdafx.h"
#include "AppContext.h"

AppContext::AppContext(const std::string &processName, const std::string &processId)
{
	this->processName = processName;
	this->processID = processId;
}

void AppContext::StartChildProcess()
{
	HANDLE hFile;
	BOOL flg;
	DWORD dwWrite;
	char szPipeUpdate[200];

	std::string s("\\\\.\\pipe\\");
	s += this->processID;
	std::string ws;

	ws.assign(s.begin(), s.end());
	LPCSTR pipeName = ws.c_str();


	hFile = CreateFile(pipeName, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
	auto error = GetLastError();
	strcpy(szPipeUpdate, "Data from Named Pipe client for create namedpipe testsetse");
	if (hFile == INVALID_HANDLE_VALUE)
	{
		DWORD dw = GetLastError();
		printf("CreateFile failed for Named Pipe client\n:");
	}
	else
	{
		flg = WriteFile(hFile, szPipeUpdate, strlen(szPipeUpdate), &dwWrite, NULL);
		if (FALSE == flg)
		{
			printf("WriteFile failed for Named Pipe client\n");
		}
		else
		{
			printf("WriteFile succeeded for Named Pipe client\n");
		}
		CloseHandle(hFile);
	}
}

AppContext::~AppContext()
{

}
