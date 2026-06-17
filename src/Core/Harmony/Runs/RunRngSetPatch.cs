namespace RebalancedSpire.Core.Harmony.Runs;

using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Configs;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Rngs;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch]
// ReSharper disable InconsistentNaming
public static class RunRngSetPatch
{
    private static readonly bool Disabled = !RebalancedSpireSettingsStore.Settings.BetterChildSeedGeneration;

    [HarmonyPatch(typeof(RunRngSet), "CreateRng")]
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool PreFix_CreateRng(RunRngSet __instance, RunRngType rngType, ref Rng __result)
    {
        if (Disabled)
        {
            return true;
        }

        using IncrementalHash hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var name = StringHelper.SnakeCase(rngType.ToString());
        var seedBytes = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(seedBytes, __instance.Seed);
        hasher.AppendData(seedBytes);
        hasher.AppendData(Encoding.UTF8.GetBytes(name));
        __result = new Rng(BinaryPrimitives.ReadUInt32LittleEndian(hasher.GetHashAndReset()));
        return false;
    }
}