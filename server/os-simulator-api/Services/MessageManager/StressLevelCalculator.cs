using System;
using Microsoft.Extensions.Options;
using Serilog;
using SomeSimulator.Data.Models.Configurations;

namespace SoMeSimulator.Services.MessageManager
{
    public class StressLevelCalculator: IStressLevelCalculator
    {
        
        private readonly IOptions<StressLevel> _stressLevel;

        public StressLevelCalculator(IOptions<StressLevel> stressLevel)
        {
            _stressLevel = stressLevel;
        }
        public bool ShouldPost(uint stressLevel)
        {
            return this.Chance(stressLevel);
        }

        public bool ShouldComment(uint stressLevel)
        {
            return this.Chance(stressLevel, _stressLevel.Value.Modifier);
        }
        /// <summary>
        ///         
        /// </summary>
        /// <param name="stressLevel"></param>
        /// <param name="modifier">MaxLevel is decreased by modifier.</param>
        /// <returns></returns>
        private bool Chance(uint stressLevel, double modifier = 0) {
            try
            {
                var random0To1000 = new Random().Next(0, 1000);

                var stressLevelModifier = Math.Ceiling(_stressLevel.Value.Modifier * (stressLevel / 100.0D));

                var maxLevel = _stressLevel.Value.MaxLevel - modifier;

                Log.Debug($"Randomized message r{random0To1000} l{maxLevel - stressLevelModifier}");

                return random0To1000 >= maxLevel - stressLevelModifier;
            }
            catch (Exception e)
            {
                Log.Debug($"Error when determing if a post should be sent. {e.Message}");
                return false;
            }
        }

    }
}