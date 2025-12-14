const Battlepass = require("./Ch3Battlepass.json");
const Prices = require("./AuraPrices.json")
let ConvertedBP = [];
Battlepass.forEach(element => {
    if (element.Name.includes("AthenaSeasonItemEntryReward") && element.Properties != null) {
        console.log(element.Properties.bIsFreePassReward)


        if (element.Properties.BattlePassOffer != null) {

            var RewardItem = element.Properties.BattlePassOffer.RewardItem
            var templatePush = "";
            let Variants = [];
            if (RewardItem.ItemDefinition.AssetPathName
                && RewardItem.ItemDefinition.AssetPathName.includes(".")) {
                var test = RewardItem.ItemDefinition.AssetPathName.split(".")[1].toLowerCase();
                console.log(RewardItem.ItemDefinition.AssetPathName);
                //console.log(test);
                //if(test.includes(""))
                if (
                    test.includes("eid") ||
                    test.includes("emoji") ||
                    test.includes("spid") ||
                    test.includes("toy")
                ) {
                    templatePush = `AthenaDance:${test}`;
                } else if (test.includes("vtid")) {
                    templatePush = `CosmeticVariantToken:${test}`;
                    //
                    //
                    if (element.Properties.BattlePassPreviewInfoList != null) {
                        element.Properties.BattlePassPreviewInfoList.forEach(element2 => {
                            if (element2.PreviewParams.VariantsToApply) {
                                element2.PreviewParams.VariantsToApply.forEach(e => {
                                    var SoMuchAura = e.VariantChannelTag.TagName.split(".");
                                    var ChannelType = "";
                                    if (SoMuchAura.length > 2)
                                        ChannelType = SoMuchAura[3];

                                    var ItemDef = "";
                                    var LazyRipped = e.ItemVariantIsUsedFor.ObjectName;
                                    var FirstPartii = LazyRipped.split("ItemDef")[0];
                                    if (FirstPartii)
                                        ItemDef = FirstPartii + ":" + LazyRipped.split("'")[1];

                                    Variants.push({
                                        connectedItem: ItemDef,
                                        channel: ChannelType,
                                        added: [
                                            e.ActiveVariantTag.TagName.split(".")[3]
                                        ],
                                    })
                                })
                            }
                        });
                    }

                    //Variants
                    //
                } else if (test.includes("athenabattlestar")) {
                    templatePush = `FortPersistentResourceItem:AthenaBattleStar`;
                } else if (test.includes("mtxgiveaway")) {
                    templatePush = `Currency:${test}`;
                } else if (test.includes("athenaseasonalxp")) {
                    templatePush = `AccountResource:${test}`;
                } else if (test.includes("glider")) {
                    templatePush = `AthenaGlider:${test}`;
                } else if (test.includes("cid")) {
                    templatePush = `AthenaCharacter:${test}`;
                } else if (
                    test.includes("athenaseason") ||
                    test.includes("athenanextseason")
                ) {
                    // idk
                    templatePush = `Token:${test}`;
                } else if (test.includes("wrap")) {
                    templatePush = `AthenaItemWrap:${test}`;
                } else if (test.includes("pickaxe")) {
                    templatePush = `AthenaPickaxe:${test}`;
                } else if (test.includes("lsid")) {
                    templatePush = `AthenaLoadingScreen:${test}`;
                } else if (test.includes("trails")) {
                    templatePush = `AthenaSkyDiveContrail:${test}`;
                } else if (test.includes("musicpack")) {
                    templatePush = `AthenaMusicPack:${test}`;
                } else if (test.includes("bid") || test.includes("petcarrier")) {
                    templatePush = `AthenaBackpack:${test}`;
                }
                else if (RewardItem.ItemDefinition.AssetPathName.includes("BannerIcons")) {
                    templatePush = `HomebaseBannerIcon:${test}`;
                }
                //QuestSchedules
                //HomebaseBannerIcon
                //else if(test.includes("")){
                //  templatePush = `HomebaseBannerIcon:${test}`
                //}

                console.log(templatePush);
            }

            var StuffToPush = {
                bRequireBp: element.Properties.bIsFreePassReward ? false : true,
                OfferId: element.Properties.BattlePassOffer.OfferId,
                templateId: templatePush,
                Price: Prices[element.Properties.BattlePassOffer.OfferPriceRowHandle.RowName].Cost,
                Quantity: RewardItem.Quantity != null ? RewardItem.Quantity : 1
            }

            if(Variants.length > 0)
                StuffToPush.new_variants = Variants

            ConvertedBP.push(StuffToPush)
        }
    }
});

console.log(JSON.stringify(ConvertedBP));