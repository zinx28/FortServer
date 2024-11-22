﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLauncher.Services.Globals
{
    public class LauncherConfig
    {
        public static string DiscordURI = "PUT YOUR DISCORD URI HERE"; // i'll change this in the far future

        public static string LoginOauthApi = "http://127.0.0.1:1111/launcher/api/v1/login"; // Post and Get is different

        public static string CurlDll = "FortCurl.dll"; // You will need to find a redirect as i won't provide
    }
}
