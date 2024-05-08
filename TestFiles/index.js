const datafile = require("./Data.json")
var tesa = 0;
var Data = [];
for(var index in datafile) {
  tesa += 1;
  var dataHa = datafile[index];
 // console.log({
   // "Level": dataHa.Level,
  //  "XpToNextLevel": dataHa.XpToNextLevel,
 //   "XpTotal": dataHa.XpTotal
 // })
  
  //var temp = []
  //if(dataHa.ChaseRewardTemplateId != "") {
    //temp.push({
    //  "TemplateId": dataHa.ChaseRewardTemplateId,
    ///  "Level": tesa,
    //  "Quantity": 1
    //})
  //}

  Data.push({
     "Level": dataHa.Level,
     "XpToNextLevel": dataHa.XpToNextLevel,
     "XpTotal": dataHa.XpTotal
   })
  //console.log({ "Rewards": temp })
}
console.log(JSON.stringify(Data));

/*
const datafile = require("./BattlePass.json");
var Data = [];
var Level = 0;

for(var index in datafile){
  var dataha = datafile[index];

  //var DataToPush = {
     // Rewards: dataha.Rewards,
     // Level: Level
  //}
  var rewards = []
  dataha.Rewards.forEach(e => { 
     var templatePush = "";
     if(e.TemplateId != "") {
        templatePush = e.TemplateId
     }else {
        if(e.ItemDefinition.AssetPathName) {
            var test = e.ItemDefinition.AssetPathName.split(".")[1].toLowerCase();
            console.log(e.ItemDefinition.AssetPathName);
            //console.log(test);
            //if(test.includes(""))
            if(e.ItemDefinition.AssetPathName.includes("ChallengeBundleSchedules")){
              templatePush = `ChallengeBundleSchedule:${test}`
            }else
            if(test.includes("eid") || test.includes("emoji") || test.includes("spid") || test.includes("toy")){
               templatePush = `AthenaDance:${test}`
            }else if(test.includes("vtid")){
              templatePush = `CosmeticVariantToken:${test}`
            }
            else if(test.includes("mtxgiveaway")) {
              templatePush = `Currency:${test}` 
            }else if(test.includes("athenaseasonalxp")) {
              templatePush = `AccountResource:${test}`
            }else if(test.includes("glider")) {
              templatePush = `AthenaGlider:${test}`
            }else if(test.includes("cid")) {
              templatePush = `AthenaCharacter:${test}`
            }else if(test.includes("athenaseason") || test.includes("athenanextseason")) { // idk
              templatePush = `Token:${test}`
            }else if(test.includes("wrap")){
              templatePush = `AthenaItemWrap:${test}`
            }else if(test.includes("pickaxe")) {
              templatePush = `AthenaPickaxe:${test}`
            }else if(test.includes("lsid")) {
              templatePush = `AthenaLoadingScreen:${test}`
            }else if(test.includes("trails")) {
              templatePush = `AthenaSkyDiveContrail:${test}`
            }else if(test.includes("musicpack")){
              templatePush = `AthenaMusicPack:${test}`
            }else if(test.includes("bid") || test.includes("petcarrier")){
              templatePush = `AthenaBackpack:${test}`
            }

            console.log(templatePush)
        }
     }
     
     rewards.push({
        templateId: templatePush,
        quantity: e.Quantity
     })
  })

  var DataToPush = {
      Rewards: rewards,
      Level: Level
  }

  Data.push(DataToPush);

  Level += 1;
}*/

console.log(JSON.stringify(Data));

/*
const datafile = require("./SeasonStars.json");
var Data = [];
var Level = 0;
for(var index in datafile){
  var dataha = datafile[index];
  var BattleStars = 0;
  for(var RewardsIndex in dataha.Rewards) 
  {
      var te = dataha.Rewards[RewardsIndex];
      BattleStars = te.Quantity;
  }
  var DataToPush = {
      Level: Level,
      BattleStars
  };
  

  Data.push(DataToPush);

  Level += 1;
}

console.log(JSON.stringify(Data));*/

/*
const datafile = require("./Quest.json")
var tesa =0;
for(var index in datafile) {
  tesa += 1;
  var dataHa = datafile[index];
 // console.log({
   // "Level": dataHa.Level,
  //  "XpToNextLevel": dataHa.XpToNextLevel,
 //   "XpTotal": dataHa.XpTotal
 // })
  var SeasonXP = 0;
  var SeasonBattleStars = 0;
  for(var RewardsIndex in dataHa.Properties.Rewards) {
    var Rewards = dataHa.Properties.Rewards[RewardsIndex];
      console.log(Rewards);

      // V1 Quests (season 1 -> 1.9.1) i think (i dont have other versions)
      //if(!Rewards.ItemDefinition.includes("AthenaSeasonalXP")) {
        //throw "ERROR REWARDS"
      //}

      //SeasonXP = Rewards.Quantity

      // V2 / V3 (idk why it's v3 after a few versions)
      if(Rewards.ItemDefinition.AssetPathName.includes("AthenaSeasonalXP")) {
          SeasonXP = Rewards.Quantity;
      }

      if(Rewards.ItemDefinition.AssetPathName.includes("AthenaSeasonalBP")) {
         SeasonBattleStars = Rewards.Quantity
      }

    
   
  }
  
  var temp = []
  var temp2 = []
 // if(dataHa.ChaseRewardTemplateId != "") {
  var OBjectives = dataHa.Properties.Objectives
  OBjectives.forEach(element => {
    temp2.push({
      "BackendName": element.BackendName,
      "ObjectiveState": element.ObjectiveStatHandle.RowName,
      "ItemEvent": element.ItemEvent,
      "ItemReference": element.ItemReference,
      "ItemTemplateIdOverride": element.ItemTemplateIdOverride,
      "Description": element.Description.SourceString,
      "HudShortDescription": element.HudShortDescription.SourceString,
      "Count": element.Count,
      "Stage": element.Stage,
      "bHidden": element.bHidden
    })
  });
    temp.push({
      "Type": dataHa.Type,
      "Name": dataHa.Name,
      "Class": dataHa.Class,
      "Properties": {
        "DisplayName": dataHa.Properties.DisplayName.LocalizedString,
        "Description": dataHa.Properties.Description.LocalizedString,
        "SeasonXP": SeasonXP,
        "SeasonBattleStars": SeasonBattleStars,
        "Objectives": temp2
      }
    })
  //}

  console.log(JSON.stringify(temp))
}*/