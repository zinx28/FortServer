using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Profile
{
    public class SetItemFavoriteStatusReq
    {
        public string targetItemId { get; set; } = string.Empty;
        public bool bFavorite { get; set; } = false;
    }

    public class SetItemFavoriteStatusBatchReq
    {
        public List<string> itemIds { get; set; } = new();
        public List<bool> itemFavStatus { get; set; } = new();
    }
}
