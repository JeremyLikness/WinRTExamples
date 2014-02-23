// PrimeChecker.cpp
#include "pch.h"
#include "PrimeChecker.h"

using namespace WinRtExampleMath;
using namespace Platform;

PrimeChecker::PrimeChecker()
{

}

bool PrimeChecker::IsPrime(int candidate)
{
	if (candidate == 2)
	{
		return true;
	}

	for (count = 2; count < candidate; count++)
	{
		if (candidate % count == 0)
		{
			return false;
		}
	}

	return true;
}
