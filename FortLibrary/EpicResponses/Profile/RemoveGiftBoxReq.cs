﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Profile
{
    public class RemoveGiftBoxReq
    {
        public string giftBoxItemId { get; set; } = string.Empty;

        public string[] giftBoxItemIds { get; set; } = new string[0];
    }
}
