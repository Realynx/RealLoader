#pragma once

//defines a common struct for handling thick and c strings

#include <nethost.h> //include so we get char_t

#include <codecvt>
#include <locale>
#include <string>
#include <cstring>

#if defined(_WIN32)
#include <Windows.h>
#endif

namespace RealLoader::Util{

#if defined(_WIN32)

	//defines a string literal for wide chars
#define STR(s) L ## s

	//defines a directory seperator
#define DIR STR("\\")

	static inline std::wstring ConvertCStringToThickString(const std::string& str){

		if (str.empty()) return std::wstring();
		int size_needed = MultiByteToWideChar(CP_UTF8, 0, &str[0], (int)str.size(), NULL, 0);
		std::wstring wstrTo(size_needed, 0);
		MultiByteToWideChar(CP_UTF8, 0, &str[0], (int)str.size(), &wstrTo[0], size_needed);
		return wstrTo;
	}

	static inline std::string ConvertThickStringToCString(const std::wstring& wstr){

		if (wstr.empty()) return std::string();
		int size_needed = WideCharToMultiByte(CP_UTF8, 0, &wstr[0], (int)wstr.size(), NULL, 0, NULL, NULL);
		std::string strTo(size_needed, 0);
		WideCharToMultiByte(CP_UTF8, 0, &wstr[0], (int)wstr.size(), &strTo[0], size_needed, NULL, NULL);
		return strTo;
	}

	//Linux
#else

	//defines a string literal for wide chars
#define STR(s) s

#define DIR "/"

	std::string ConvertCStringToThickString(const std::string& str) { return str; }

	std::string ConvertThickStringToCString(const std::string& wstr) { return wstr; }

#endif

	//compares two char arrays
	static inline bool IsSameString(const char_t* s1, const char_t* s2)
	{
#if defined(_WIN32)
		return (!lstrcmpW(s1, s2) ? true : false);
#else
		return (!strcmp(s1, s2) ? true : false);
#endif
	}

	//defines a common string class
	struct RealString
	{
#if defined(_WIN32)
		std::wstring data = '\0';
#else
		std::string data = '\0';
#endif

		//default constructor
		RealString() {}

		RealString(const char_t* _data)
		{
			data = _data;
		}

		RealString(const std::string& _data)
		{
#if defined(_WIN32)
			data = ConvertCStringToThickString(_data);
#else
			dara = _data
#endif
		}

#if defined(_WIN32)
		RealString(const std::wstring& _data)
		{
			data = _data;
		}
#endif

		//operators

		RealString operator+(const char_t* other)
		{
			return data + other;
		}

		RealString operator+(const RealString& other)
		{
			return data + other.data;
		}

		RealString operator+=(const RealString& other)
		{
			return data += other.data;
		}
	};
}