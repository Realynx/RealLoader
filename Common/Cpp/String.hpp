#pragma once

//defines a common struct for handling thick and c strings

#include <codecvt>
#include <locale>
#include <string>

#include <Windows.h>

namespace PalMM::Util
{
#if defined(_WIN32)

    //defines a string literal for wide chars
#define STR(s) L ## s

    std::wstring ConvertCStringToThickString(const std::string& str)
    {
        if (str.empty()) return std::wstring();
        int size_needed = MultiByteToWideChar(CP_UTF8, 0, &str[0], (int)str.size(), NULL, 0);
        std::wstring wstrTo(size_needed, 0);
        MultiByteToWideChar(CP_UTF8, 0, &str[0], (int)str.size(), &wstrTo[0], size_needed);
        return wstrTo;
    }

    std::string ConvertThickStringToCString(const std::wstring& wstr)
    {
        if (wstr.empty()) return std::string();
        int size_needed = WideCharToMultiByte(CP_UTF8, 0, &wstr[0], (int)wstr.size(), NULL, 0, NULL, NULL);
        std::string strTo(size_needed, 0);
        WideCharToMultiByte(CP_UTF8, 0, &wstr[0], (int)wstr.size(), &strTo[0], size_needed, NULL, NULL);
        return strTo;
    }

    //Linux
#else

    //defines a string literal for wide chars
#define STR(s) 

    std::string ConvertCStringToThickString(const std::string& str)
    {
        return str;
        //if (str.empty()) return std::wstring();
        //int size_needed = MultiByteToWideChar(CP_UTF8, 0, &str[0], (int)str.size(), NULL, 0);
        //std::wstring wstrTo(size_needed, 0);
        //MultiByteToWideChar(CP_UTF8, 0, &str[0], (int)str.size(), &wstrTo[0], size_needed);
        //return wstrTo;
    }

    std::string ConvertThickStringToCString(const std::string& wstr)
    {
        return wstr;
       //if (wstr.empty()) return std::string();
       //int size_needed = WideCharToMultiByte(CP_UTF8, 0, &wstr[0], (int)wstr.size(), NULL, 0, NULL, NULL);
       //std::string strTo(size_needed, 0);
       //WideCharToMultiByte(CP_UTF8, 0, &wstr[0], (int)wstr.size(), &strTo[0], size_needed, NULL, NULL);
       //return strTo;
    }

#endif

    //compares two char arrays
    inline bool IsSameString(const char* s1, const char* s2) { return strcmp(s1, s2); }

    //compares two thick char arrays
   // inline bool IsSameString(const wchar_t* s1, const wchar_t* s2) {return IsSameString(ConvertThickStringToCString(s1).c_str(), ConvertThickStringToCString(s2).c_str());}

    //defines a string that can be converted between thick and c string on the fly
    struct String
    {
        //the internal data's it switches between
        
#if defined(_WIN32)
        std::wstring thickCharData = L"";
#endif

        std::string charData = "";

#if defined(_WIN32)
        //sets the string data
        inline void SetCharData(const wchar_t* data)
        {
            charData = ConvertThickStringToCString(data);
            thickCharData = std::wstring(data);
        }

        //sets the string data
        inline void SetThickCharData(const char* data)
        {
            thickCharData = ConvertCStringToThickString(data);
            charData = std::string(data);
        }

        //gets the thick char array
        inline const wchar_t* GetWideCharArray() { return thickCharData.c_str(); }

        //gets the char array
        inline const char* GetCharArray() { return charData.c_str(); }

        //Linux defs
#else

        //sets the string data
        inline void SetCharData(const char_t* data)
        {
            charData = std::string(data); // ConvertThickStringToCString(data);
            //thickCharData = std::wstring(data);
        }

        //sets the string data
        inline void SetThickCharData(const char* data)
        {
           // thickCharData = ConvertCStringToThickString(data);
            charData = std::string(data);
        }

        //gets the thick char array
        inline const wchar_t* GetWideCharArray() { return charData.c_str(); } //{ return thickCharData.c_str(); }

        //gets the char array
        inline const char* GetCharArray() { return charData.c_str(); }

#endif
    };
}