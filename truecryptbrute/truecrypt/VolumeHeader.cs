﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace truecryptbrute.TrueCrypt
{

    public class VolumeHeader
    {
        public const int TC_VOLUMEHEADER_EFFECTIVE_SIZE = 512;

        private TCHeaderData headerData = new TCHeaderData();
        private IntPtr headerStructPtr;
        
        #region Constructor

        public VolumeHeader(byte[] buffer) {
            if(buffer.Length == TC_VOLUMEHEADER_EFFECTIVE_SIZE) {
                headerData.Data = new byte[TC_VOLUMEHEADER_EFFECTIVE_SIZE];
                buffer.CopyTo(headerData.Data, 0);

                headerStructPtr = Marshal.AllocHGlobal(TC_VOLUMEHEADER_EFFECTIVE_SIZE);
                Marshal.StructureToPtr(headerData, headerStructPtr, true);          
            } else {
                throw new FieldAccessException("Volumeheader buffer has invalid length!");
            }
        }

        #endregion

        /// <summary>
        /// Decrypts this header with the given Key Pool
        /// </summary>
        /// <param name="passwordStructure">With Keyfiles applied to it!</param>
        /// <returns></returns>
        public bool DecryptVolumeHeader(byte[] passwordStructure){
            TCErrorCode Status;

            var ret = TrueCryptInterOP.CheckVolumeHeaderPassword(false, headerStructPtr, passwordStructure);
            
            if(Enum.IsDefined(typeof(TCErrorCode), ret)){
                Status = (TCErrorCode)ret;
            }else{
                throw new NotSupportedException();
            }

            switch(Status) {

                case TCErrorCode.ERR_SUCCESS:
                    return true;

                case TCErrorCode.ERR_PASSWORD_WRONG:
                    return false;

                default:
                    throw new TCException(Status);
            }

        }

    }
}
