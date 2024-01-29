#pragma once

//defines a common struct for handling thick and c strings

#include "atlbase.h"
#include "atlstr.h"
#include "comutil.h"
#include <string>

namespace PalMM::Util
{
    //defines a string literal for wide chars
#define STR(s) L ## s

    //converts a char array to a thick char array
    inline static std::wstring ConvertCStringToThickString(const char* data) { return std::wstring(CA2W(data).m_psz); }

    //converts a thick char array to a char array
    inline static std::string ConvertThickStringToCString(const wchar_t* data) { return std::string(CStringA(data).GetString()); }

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