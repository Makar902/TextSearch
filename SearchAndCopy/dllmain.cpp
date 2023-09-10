// dllmain.cpp : Defines the entry point for the DLL application.

#include "pch.h"
#include <iostream>
#include <vector>
#include <string>
#include <fstream>
#include <Windows.h>
#include <filesystem>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

struct FileInfo {
    std::string fileName;
    std::string filePath;
    DWORD fileAttributes;
    FILETIME creationTime;
    FILETIME lastAccessTime;
    FILETIME lastWriteTime;
    ULONGLONG fileSize;
};
 //Don`t use
__declspec(dllexport)
std::vector<FileInfo> SearchAndModifyFiles(const std::string& directoryWhere, const std::string& wordsToSearch) {
    WCHAR buffer[MAX_PATH];
    GetModuleFileNameW(NULL, buffer, MAX_PATH);
    std::wstring currentPath = buffer;
    std::wstring outputDirectory = currentPath.substr(0, currentPath.find_last_of(L"\\"));

    WIN32_FIND_DATA findData;
    HANDLE findHandle = FindFirstFile((std::wstring(directoryWhere.begin(), directoryWhere.end()) + L"\\*.*").c_str(), &findData);

    std::vector<FileInfo> resultList;

    if (findHandle != INVALID_HANDLE_VALUE) {
        do {
            if (!(findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
                std::wstring wFileName = findData.cFileName;
                std::string fileName(wFileName.begin(), wFileName.end());

                std::string filePath = directoryWhere + "\\" + fileName;

                std::ifstream file(filePath);
                std::string fileContent;
                std::string line;

                while (std::getline(file, line)) {
                    fileContent += line;
                }
                if (fileContent.find(wordsToSearch) != std::string::npos) {
                    FileInfo fileInfo;
                    fileInfo.fileName = fileName;
                    fileInfo.filePath = filePath;
                    fileInfo.fileAttributes = findData.dwFileAttributes;
                    fileInfo.creationTime = findData.ftCreationTime;
                    fileInfo.lastAccessTime = findData.ftLastAccessTime;
                    fileInfo.lastWriteTime = findData.ftLastWriteTime;
                    fileInfo.fileSize = ((ULONGLONG)findData.nFileSizeHigh << 32) | findData.nFileSizeLow;
                    resultList.push_back(fileInfo);

                    // Copy and modify the file here
                    std::string ResPath = std::string(outputDirectory.begin(), outputDirectory.end()) + "\\" + fileName;
                    ResPath.append("\\");
                    ResPath.append(fileName);

                    if (CopyFileA(filePath.c_str(), ResPath.c_str(), FALSE)) {
                        std::ifstream inFile(ResPath);
                        std::string fileContent;
                        std::string line;

                        while (std::getline(inFile, line)) {
                            fileContent += line + "\n";
                        }

                        size_t pos = 0;
                        while ((pos = fileContent.find(wordsToSearch, pos)) != std::string::npos) {
                            fileContent.replace(pos, wordsToSearch.length(), "*******");
                            pos += 7; // Move past the replacement text
                        }

                        std::ofstream outFile(ResPath);
                        outFile << fileContent;
                    }
                }
            }
        } while (FindNextFile(findHandle, &findData) != 0);

        FindClose(findHandle);
    }

    return resultList;
}
__declspec(dllexport)
std::vector<FileInfo> inline SearchAndModifyFiles(const std::string& directoryWhere, const std::string& wordsToSearch,bool userReport) {
    WCHAR buffer[MAX_PATH];
    GetModuleFileNameW(NULL, buffer, MAX_PATH);
    std::wstring currentPath = buffer;
    std::wstring outputDirectory = currentPath.substr(0, currentPath.find_last_of(L"\\"));

    WIN32_FIND_DATA findData;
    HANDLE findHandle = FindFirstFile((std::wstring(directoryWhere.begin(), directoryWhere.end()) + L"\\*.*").c_str(), &findData);

    std::vector<FileInfo> resultList;

    if (findHandle != INVALID_HANDLE_VALUE) {
        do {
            if (!(findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
                std::wstring wFileName = findData.cFileName;
                std::string fileName(wFileName.begin(), wFileName.end());

                std::string filePath = directoryWhere + "\\" + fileName;

                std::ifstream file(filePath);
                std::string fileContent;
                std::string line;

                while (std::getline(file, line)) {
                    fileContent += line;
                }
                if (fileContent.find(wordsToSearch) != std::string::npos) {
                    FileInfo fileInfo;
                    fileInfo.fileName = fileName;
                    fileInfo.filePath = filePath;
                    fileInfo.fileAttributes = findData.dwFileAttributes;
                    fileInfo.creationTime = findData.ftCreationTime;
                    fileInfo.lastAccessTime = findData.ftLastAccessTime;
                    fileInfo.lastWriteTime = findData.ftLastWriteTime;
                    fileInfo.fileSize = ((ULONGLONG)findData.nFileSizeHigh << 32) | findData.nFileSizeLow;
                    resultList.push_back(fileInfo);                  
                }
            }
        } while (FindNextFile(findHandle, &findData) != 0);

        FindClose(findHandle);
    }

    return resultList;
}
