﻿using System.Reflection;
using System.Runtime.InteropServices;

namespace WC3MapDeprotector
{
    public class StormLibrary
    {
        private const string STORMLIB = "StormLib.dll";

        static StormLibrary()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            File.Copy(Path.Combine(directoryName, Environment.Is64BitProcess ? "StormLib_x64.dll" : "StormLib_x86.dll"), Path.Combine(directoryName, STORMLIB), true);
        }

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileOpenArchive([MarshalAs(UnmanagedType.LPTStr)] string szMpqName, uint dwPriority, SFileOpenArchiveFlags dwFlags, out IntPtr phMpq);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileHasFile(IntPtr hMpq, [MarshalAs(UnmanagedType.LPStr)] string szFileName);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileGetFileInfo(IntPtr hMpqOrFile, SFileInfoClass InfoClass, IntPtr pvFileInfo, uint cbFileInfoSize, out uint pcbLengthNeeded);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileExtractFile(IntPtr hMpq, [MarshalAs(UnmanagedType.LPStr)] string szToExtract, [MarshalAs(UnmanagedType.LPTStr)] string szExtracted, uint dwSearchScope);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileCloseArchive(IntPtr hMpq);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern IntPtr SFileFindFirstFile(IntPtr hMpq, [MarshalAs(UnmanagedType.LPStr)] string szMask, out _SFILE_FIND_DATA lpFindFileData, [MarshalAs(UnmanagedType.LPStr)] string szListFile);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileFindNextFile(IntPtr hFind, out _SFILE_FIND_DATA lpFindFileData);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileFindClose(IntPtr hFind);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileOpenFileEx(IntPtr hMpq, [MarshalAs(UnmanagedType.LPStr)] string szFileName, uint dwSearchScope, out IntPtr phFile);

        [DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        public static extern bool SFileCloseFile(IntPtr hFile);

        //[DllImport(STORMLIB, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        //public static extern bool SFileGetFileName(IntPtr hFile, [MarshalAs(UnmanagedType.LPStr)] out string szFileName);

        public unsafe struct _TFileEntry
        {
            public ulong FileNameHash;
            public ulong ByteOffset;
            public ulong FileTime;
            public uint dwHashIndex;
            public uint dwFileSize;
            public uint dwCmpSize;
            public uint dwFlags;
            public ushort lcLocale;
            public ushort wPlatform;
            public uint dwCrc32;
            public fixed byte md5[16];
            public IntPtr szFileName;
        }

        public unsafe struct _TMPQHash
        {
            public ulong dwName1;
            public ulong dwName2;
            public uint lcLocale;
            public uint wPlatform;
            public ulong dwBlockIndex;
        };

        public unsafe struct _SFILE_FIND_DATA
        {
            public fixed byte cFileName[1024];                  // Full name of the found file
            public IntPtr szPlainName;                         // Plain name of the found file
            public uint dwHashIndex;                          // Hash table index for the file
            public uint dwBlockIndex;                          // Block table index for the file
            public uint dwFileSize;                            // File size in bytes
            public uint dwFileFlags;                           // MPQ file flags
            public uint dwCompSize;                            // Compressed file size
            public uint dwFileTimeLo;                          // Low 32-bits of the file time (0 if not present)
            public uint dwFileTimeHi;                          // High 32-bits of the file time (0 if not present)
            public uint lcLocale;                              // Locale version
        }

        [Flags]
        public enum SFileOpenArchiveFlags : uint
        {
            None = 0,
            TypeIsFile = 0,
            TypeIsMemoryMapped = 1,
            TypeIsHttp = 2,
            AccessReadOnly = 256, // 0x00000100
            AccessReadWriteShare = 512, // 0x00000200
            AccessUseBitmap = 1024, // 0x00000400
            DontOpenListfile = 65536, // 0x00010000
            DontOpenAttributes = 131072, // 0x00020000
            DontSearchHeader = 262144, // 0x00040000
            ForceVersion1 = 524288, // 0x00080000
            CheckSectorCRC = 1048576, // 0x00100000
        }

        public enum SFileInfoClass
        {
            SFileMpqFileName,
            SFileMpqStreamBitmap,
            SFileMpqUserDataOffset,
            SFileMpqUserDataHeader,
            SFileMpqUserData,
            SFileMpqHeaderOffset,
            SFileMpqHeaderSize,
            SFileMpqHeader,
            SFileMpqHetTableOffset,
            SFileMpqHetTableSize,
            SFileMpqHetHeader,
            SFileMpqHetTable,
            SFileMpqBetTableOffset,
            SFileMpqBetTableSize,
            SFileMpqBetHeader,
            SFileMpqBetTable,
            SFileMpqHashTableOffset,
            SFileMpqHashTableSize64,
            SFileMpqHashTableSize,
            SFileMpqHashTable,
            SFileMpqBlockTableOffset,
            SFileMpqBlockTableSize64,
            SFileMpqBlockTableSize,
            SFileMpqBlockTable,
            SFileMpqHiBlockTableOffset,
            SFileMpqHiBlockTableSize64,
            SFileMpqHiBlockTable,
            SFileMpqSignatures,
            SFileMpqStrongSignatureOffset,
            SFileMpqStrongSignatureSize,
            SFileMpqStrongSignature,
            SFileMpqArchiveSize64,
            SFileMpqArchiveSize,
            SFileMpqMaxFileCount,
            SFileMpqFileTableSize,
            SFileMpqSectorSize,
            SFileMpqNumberOfFiles,
            SFileMpqRawChunkSize,
            SFileMpqStreamFlags,
            SFileMpqIsReadOnly,
            SFileInfoPatchChain,
            SFileInfoFileEntry,
            SFileInfoHashEntry,
            SFileInfoHashIndex,
            SFileInfoNameHash1,
            SFileInfoNameHash2,
            SFileInfoNameHash3,
            SFileInfoLocale,
            SFileInfoFileIndex,
            SFileInfoByteOffset,
            SFileInfoFileTime,
            SFileInfoFileSize,
            SFileInfoCompressedSize,
            SFileInfoFlags,
            SFileInfoEncryptionKey,
            SFileInfoEncryptionKeyRaw,
        }
    }
}
