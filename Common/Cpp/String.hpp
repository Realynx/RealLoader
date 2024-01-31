#pragma once

//defines a common struct for handling thick and c strings

#include <codecvt>
#include <locale>
#include <string>

namespace PalMM::Util
{
    //defines a string literal for wide chars
#define STR(s) L ## s

    std::wstring ConvertCStringToThickString(const std::string& str)
    {
        std::wstring_convert<std::codecvt_utf8<wchar_t>, wchar_t> converterX;
        return converterX.from_bytes(str);
    }

    std::string ConvertThickStringToCString(const std::wstring& wstr)
    {
        std::wstring_convert<std::codecvt_utf8<wchar_t>, wchar_t> converterX;
        return converterX.to_bytes(wstr);
    }

    //compares two char arrays
    inline bool IsSameString(const char* s1, const char* s2) { return strcmp(s1, s2); }

    //compares two thick char arrays
    inline bool IsSameString(const wchar_t* s1, const wchar_t* s2) {return IsSameString(ConvertThickStringToCString(s1).c_str(), ConvertThickStringToCString(s2).c_str());}

    //defines a string that can be converted between thick and c string on the fly
    struct String
    {
        //the internal data's it switches between
        std::wstring thickCharData = L"";
        std::string charData = "";

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
    };
}