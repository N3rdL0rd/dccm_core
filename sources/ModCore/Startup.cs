﻿using Hashlink;
using ModCore.Storage;
using ModCore.Track;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModCore
{
    internal unsafe static class Startup
    {
        [WillCallHL]
        private static int AfterCoreLoaded()
        {
            var logger = Log.Logger.ForContext(typeof(Startup));

            //Load hlboot.dat
            var hlbootPath = Environment.GetEnvironmentVariable("DCCM_HLBOOT_PATH");
            if (string.IsNullOrEmpty(hlbootPath))
            {
                hlbootPath = FolderInfo.GameRoot.GetFilePath("hlboot.dat");
            }
            
            byte* hlboot = null;
            int hlbootSize = 0;
            logger.Information("Finding hlboot.dat");
            if (File.Exists(hlbootPath))
            {
                var mmf = MemoryMappedFile.CreateFromFile(hlbootPath);
                var view = mmf.CreateViewAccessor();
                view.SafeMemoryMappedViewHandle.AcquirePointer(ref hlboot);
                hlbootSize = (int)view.SafeMemoryMappedViewHandle.ByteLength;
            }
            else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    logger.Information("Loading hlboot.dat from deadcells_gl.exe");
                    hlboot = (byte*)Native.hlu_get_hl_bytecode_from_exe(FolderInfo.GameRoot.GetFilePath("deadcells_gl.exe"), &hlbootSize);
                }
                else
                {
                    throw new FileNotFoundException(null, "hlboot.dat");
                }
            }
            byte* err = null;
            logger.Information("Reading hl bytecode");

            hl_global_init();


            var code = hl_code_read(hlboot, hlbootSize, &err);
            if(err != null)
            {
                logger.Error("An error occurred while loading bytecode: {err}", Marshal.PtrToStringAnsi((nint)err));
                return -1;
            }
            logger.Information("Starting game");
            MixTrace.MarkEnteringHL();
            return Native.hlu_start_game(code);
        }
        
        public static int StartGame()
        {
            Console.Title = "Dead Cells with Core Modding";
            Core.Initialize();

            return AfterCoreLoaded();
        }
    }
}
