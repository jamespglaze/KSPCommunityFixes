using System;
using KSP.UI.Screens.Mapview;

namespace KSPCommunityFixes.QoL
{
    class EncounterVelocityAndAngle : BasePatch
    {
        protected override Version VersionMin => new Version(1, 8, 0);

        protected override void ApplyPatches()
        {
            AddPatch(PatchType.Postfix, typeof(PatchRendering), nameof(PatchRendering.mnEnd_OnUpdateCaption));
        }

        // After PatchRendering.mnEnd_OnUpdateCaption finishes updating an instance of MapNode.CaptionData, check if patch currently being rendered
        // ends with an encounter or escape, and if so, update the unused captionLine2 and captionLine3 fields with the velocity and angle off prograde
        // at the moment of patch transition, as measured against the body being encountered or escaped from.
        static void PatchRendering_mnEnd_OnUpdateCaption_Postfix(PatchRendering __instance, ref MapNode n, ref MapNode.CaptionData cData)
        {
            switch (__instance.patch.patchEndTransition)
            {
                case Orbit.PatchTransitionType.ENCOUNTER:
                    double encounterUT = __instance.patch.UTsoi;
                    Orbit nextPatch = __instance.patch.nextPatch;
                    Vector3d encounterVel = nextPatch.getOrbitalVelocityAtUT(encounterUT);
                    cData.captionLine2 = "Rel-V: " + encounterVel.magnitude.ToString("0.##") + "m/s";
                    cData.captionLine3 = "Encounter Angle: " + Vector3d.Angle(nextPatch.referenceBody.orbit.getOrbitalVelocityAtUT(encounterUT), encounterVel).ToString("0.##") + "°";
                    break;
                case Orbit.PatchTransitionType.ESCAPE:
                    double escapeUT = __instance.patch.UTsoi;
                    Vector3d escapeVel = __instance.patch.getOrbitalVelocityAtUT(escapeUT);
                    cData.captionLine2 = "Exit-V: " + escapeVel.magnitude.ToString("0.##") + "m/s";
                    cData.captionLine3 = "Ejection Angle: " + Vector3d.Angle(__instance.patch.referenceBody.orbit.getOrbitalVelocityAtUT(escapeUT), escapeVel).ToString("0.##") + "°";
                    break;
                default:
                    break;
            }
        }
    }
}