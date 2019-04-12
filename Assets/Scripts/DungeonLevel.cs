using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Daryl Keogh
/// Description: This class will handle our statics for items such as tier loot drop chances
/// The level of the dungeon and the health and damage modifiers of enemies
/// </summary>
public class DungeonLevel : MonoBehaviour {
    
    public static int level = 1; //The level of the dungeon that the player is on
    public static float HPModifier = 1; //The multiplier for enemies helath
    public static float DMGModifier = 1; //The multiplier for enemies damage

    //The below chances will increase for mid and higher tier the more the player
    //progresses through the game. The below values are 3 - their drop chance.
    //The lower the drop chance, the higher chance it will drop (it makes sense, trust me)
    //These are used in our HitLootResolver!
    public static float lowTierChance =.7f; //The chance for low tier loot to drop (2.3)
    public static float midTierChance = 2.5f; //The chance for mid tier loot to drop (.5)
    public static float highTierChance = 2.8f; //The chance for high tier loot to drop (.2)

    //Private floats to show the current chance for items
    static float lowRate = 2.3f;
    static float midRate = .5f;
    static float highRate = .2f;

    //Levels up the dungeon by increasing the modifiers and dungeon level
    public static void LevelUpDungeon()
    {
        level++; //Increase level by 1
        HPModifier += .075f; //Increase all enemies health by 7.5 percent
        DMGModifier += .025f; //Increase all enemies damage by 2.5 percent

        //Every 2nd level, we will up the chances for mid tier
        //and high tier loot to drop and lower the chances for low tier loot
        //up to a max value. We always want bad loot to drop more often than good loo
        //to keep the player playing and attempting to max out their stats
        if(level % 2 == 0)
        {
            lowRate = Mathf.Clamp(lowRate - .18f, 1.22f, Mathf.Infinity); //Reduce low tier gear drop rate significantly
            midRate = Mathf.Clamp(lowRate + .10f, 0, 1.1f); //Increase mid tier gear drop rate
            highRate = Mathf.Clamp(lowRate + .08f, 0, .68f); //Increase high tier gear drop rate

            //Set our drop chances
            lowTierChance = 3.0f - lowRate;
            midTierChance = 3.0f - midRate;
            highTierChance = 3.0f - highRate;
        }
    }
}
