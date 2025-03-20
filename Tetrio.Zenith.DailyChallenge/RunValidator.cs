using System.Diagnostics;
using TetraLeague.Overlay.Network.Api.Tetrio.Models;
using Tetrio.Overlay.Database;
using Tetrio.Overlay.Database.Entities;
using Tetrio.Overlay.Database.Enums;

namespace Tetrio.Zenith.DailyChallenge;

public class RunValidator
{
    public IEnumerable<Challenge> ValidateRun(List<Challenge> challenges, Run run, string[] mods)
    {
        Console.WriteLine($"[DAILY VALIDATION] Running validation for {run.TetrioId} submitted by {run.User.Username}");

        foreach (var challenge in challenges)
        {
            if (challenge.Conditions != null)
            {
                Console.WriteLine($"- Validating {((Difficulty)challenge.Points).ToString()} challenge");

                var isChallengeCompleted = true;

                var requiredMods = challenge.Mods.Split(" ");

                if (!mods.All(x => requiredMods.Contains(x)))
                {
                    isChallengeCompleted = false;
                    Console.WriteLine("\t- Not all required mods found.");
                }
                else
                {
                    foreach (var condition in challenge.Conditions)
                    {
                        switch (condition.Type)
                        {
                            case ConditionType.Height:
                                isChallengeCompleted &= run.Altitude >= condition.Value;
                                break;
                            case ConditionType.KOs:
                                isChallengeCompleted &= run.KOs >= condition.Value;
                                break;
                            case ConditionType.AllClears:
                                isChallengeCompleted &= run.AllClears >= condition.Value;
                                break;
                            case ConditionType.Quads:
                                isChallengeCompleted &= run.Quads >= condition.Value;
                                break;
                            case ConditionType.Spins:
                                isChallengeCompleted &= run.Spins >= condition.Value;
                                break;
                            default:
                                continue;
                        }

                        Console.WriteLine($"\t- {condition.Type.ToString()} {(isChallengeCompleted ? "is valid" : $"is not valid. Run Invalid for challenge {(Difficulty)challenge.Points}, aborting...")}");

                        if (!isChallengeCompleted) break;
                    }
                }

                Console.WriteLine($"- Validation for challenge {challenge.Id} ({((Difficulty) challenge.Points).ToString()}) has been completed for {run.TetrioId}.");

                if (isChallengeCompleted)
                {
                    yield return challenge;
                }
            }
        }
    }
}