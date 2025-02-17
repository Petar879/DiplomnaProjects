﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Interfaces
{
    public interface IHandheldNfcService
    {
        /// <summary>
        /// Configuration for NFC service 
        /// </summary>
        public void ConfigureNfcAdapter()
        {

        }

        /// <summary>
        /// Enable NFC search
        /// </summary>
        public void EnableForegroundDispatch()
        {

        }

        /// <summary>
        /// Disable NFC search
        /// </summary>
        public void DisableForegroundDispatch()
        {

        }

        /// <summary>
        /// Unconfigure NFC services
        /// </summary>
        public void UnconfigureNfcAdapter()
        {

        }

        /// <summary>
        /// Send data over NFC 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public Task SendAsync(byte[] bytes);

        /// <summary>
        /// Open NFC settings, This is only for Android.
        /// </summary>
        /// <returns></returns>
        public Task<bool> OpenNFCSettingsAsync() => Task.FromResult(true);
    }
}
