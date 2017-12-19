#pragma once
#include <string>
#include <stdexcept>

class AppContext
{
private:
	std::string processName;
	std::string processID;
public:
	AppContext(const std::string& processName, const std::string& processId);
	void StartChildProcess();
	virtual ~AppContext();
};

