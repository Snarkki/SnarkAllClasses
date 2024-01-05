using HarmonyLib;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Progression.Prerequisites;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Components;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Progression.Paths;

namespace SnarkAllClasses
{
    internal class ClassPatcher
    {
        static public void ChangeBlueprints()
        {
            // base classes
            var AdeptPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("1529e5a0e7844bf3bb8d0cc0501264d4");
            var FighterCareerPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("974496d72fbe4329b438ee15cf004bd2");
            var LeaderCareerPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("33725d84e95e4323ac46d8fbf899b250");
            var SoldierCareerPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("06f4f78a9c1a472b85cd79a9a142153d");
            // archetypes
            var VeteranPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("651684417def4c258c72ba91f481b817");
            var TacticianPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("604fa184d7d944c8ae5965f9700782b5");
            var AssassinCareerPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("7b90955673a54136be9c11743943fdfe");
            var Hunter_CareerPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("6f276e8a8e2c4a548504ae39d2a7f22a");
            var StrategistCareerPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("a31b390cabe7464fbfd0e1ba53c4112f");
            var Vanguard_CareerPath = ResourcesLibrary.TryGetBlueprint<BlueprintCareerPath>("fec9cd09f11b4615b7a17f441350d2d4");

            //Create a new PrerequisiteFact
            var newAdeptPrequisite = new PrerequisiteFact
            {
                Not = false,          // Set Not as false
                m_Fact = AdeptPath.ToReference<BlueprintUnitFactReference>(),   // Set m_Fact as the AdeptPath
                MinRank = 15          // Set MinRank as 15
            };

            var newFighterPreq = new PrerequisiteFact
            {
                Not = false,          // Set Not as false
                m_Fact = FighterCareerPath.ToReference<BlueprintUnitFactReference>(),   // Set m_Fact as the AdeptPath
                MinRank = 15          // Set MinRank as 15
            };

            var newLeaderPreq = new PrerequisiteFact
            {
                Not = false,          // Set Not as false
                m_Fact = LeaderCareerPath.ToReference<BlueprintUnitFactReference>(),   // Set m_Fact as the AdeptPath
                MinRank = 15          // Set MinRank as 15
            };

            var newSoldierPreq = new PrerequisiteFact
            {
                Not = false,          // Set Not as false
                m_Fact = SoldierCareerPath.ToReference<BlueprintUnitFactReference>(),   // Set m_Fact as the AdeptPath
                MinRank = 15          // Set MinRank as 15
            };

            // FighterWarrior addons
            Hunter_CareerPath.Prerequisites.List = Hunter_CareerPath.Prerequisites.List.AddToArray(newFighterPreq);
            TacticianPath.Prerequisites.List = TacticianPath.Prerequisites.List.AddToArray(newFighterPreq);
            StrategistCareerPath.Prerequisites.List = StrategistCareerPath.Prerequisites.List.AddToArray(newFighterPreq);

            // Officer Addons
            AssassinCareerPath.Prerequisites.List = AssassinCareerPath.Prerequisites.List.AddToArray(newLeaderPreq);
            Hunter_CareerPath.Prerequisites.List = Hunter_CareerPath.Prerequisites.List.AddToArray(newLeaderPreq);
            VeteranPath.Prerequisites.List = VeteranPath.Prerequisites.List.AddToArray(newLeaderPreq);

            // Operative Addons
            Vanguard_CareerPath.Prerequisites.List = Vanguard_CareerPath.Prerequisites.List.AddToArray(newAdeptPrequisite);
            VeteranPath.Prerequisites.List = VeteranPath.Prerequisites.List.AddToArray(newAdeptPrequisite);
            TacticianPath.Prerequisites.List = TacticianPath.Prerequisites.List.AddToArray(newAdeptPrequisite);


            // Soldiers Addons
            AssassinCareerPath.Prerequisites.List = AssassinCareerPath.Prerequisites.List.AddToArray(newSoldierPreq);
            Vanguard_CareerPath.Prerequisites.List = Vanguard_CareerPath.Prerequisites.List.AddToArray(newSoldierPreq);
            StrategistCareerPath.Prerequisites.List = StrategistCareerPath.Prerequisites.List.AddToArray(newSoldierPreq);



        }
    }
}
