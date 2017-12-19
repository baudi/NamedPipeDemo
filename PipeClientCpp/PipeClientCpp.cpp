// PipeClientCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "PipeClientCpp.h"
#include "AppContext.h"
#include <string>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// The one and only application object

CWinApp theApp;

using namespace std;

int main(int argc, const char * argv[])
{
    int nRetCode = 0;

    HMODULE hModule = ::GetModuleHandle(nullptr);

    if (hModule != nullptr)
    {
        // initialize MFC and print and error on failure
        if (!AfxWinInit(hModule, nullptr, ::GetCommandLine(), 0))
        {
            // TODO: change error code to suit your needs
            wprintf(L"Fatal Error: MFC initialization failed\n");
            nRetCode = 1;
        }
        else
        {
			bool parentComplete = false;
			//System::Windows::Forms::MessageBox::Show("debug");
			string p1 = argv[0];
			string p2 = argv[1];
			AppContext process(p1, p2);
			process.StartChildProcess();

			return 0;
        }
    }
    else
    {
        // TODO: change error code to suit your needs
        wprintf(L"Fatal Error: GetModuleHandle failed\n");
        nRetCode = 1;
    }

    return nRetCode;
}