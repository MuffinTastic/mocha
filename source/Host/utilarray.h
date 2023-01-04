#pragma once
#include <vector>

struct UtilArray
{
	// How many items are in this array?
	int count;

	// How big is this array (in bytes)
	int size;

	// A pointer to the data that this array contains
	void* data;

	// Convert this array to a vector of T
	template <typename T>
	std::vector<T> GetData()
	{
		std::vector<T> vec;
		T* convertedData = ( T* )data;

		vec.insert( vec.begin(), convertedData, convertedData + count );

		return vec;
	}

	// Convert from a vector of T to a UtilArray
	template <typename T>
	static UtilArray FromVector( std::vector<T> vec )
	{
		UtilArray array;
		array.count = vec.size();
		array.size = vec.size() * sizeof( T );
		array.data = vec.data();

		return array;
	}
};