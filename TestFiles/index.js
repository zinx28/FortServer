/*const datafile = require("./Data.json")
var tesa =0;
for(var index in datafile) {
  tesa += 1;
  var dataHa = datafile[index];
 // console.log({
   // "Level": dataHa.Level,
  //  "XpToNextLevel": dataHa.XpToNextLevel,
 //   "XpTotal": dataHa.XpTotal
 // })
  
  var temp = []
  if(dataHa.ChaseRewardTemplateId != "") {
    temp.push({
      "TemplateId": dataHa.ChaseRewardTemplateId,
      "Level": tesa,
      "Quantity": 1
    })
  }

  console.log({ "Rewards": temp })
}*/

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
  for(var RewardsIndex in dataHa.Properties.Rewards) {
    var Rewards = dataHa.Properties.Rewards[RewardsIndex];
      if(!Rewards.ItemDefinition.includes("AthenaSeasonalXP")) {
        throw "ERROR REWARDS"
      }

      SeasonXP = Rewards.Quantity
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
        "Objectives": temp2
      }
    })
  //}

  console.log(JSON.stringify(temp))
}