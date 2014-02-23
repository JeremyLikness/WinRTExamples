#pragma once

namespace WinRtExampleMath
{
	public ref class PrimeChecker sealed
	{		
		int count;
	public:
		PrimeChecker();
		bool IsPrime(int candidate);
	};
}