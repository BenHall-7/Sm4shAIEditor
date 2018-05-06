﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sm4shAIEditor.Static
{
    static class script_data
    {
        public class CmdInfo
        {
            public byte ID;
            public string Name;
            public string Description;
            //when I get a chance overloads and parameters will be included

            public CmdInfo(byte id, string name, string description)
            {
                ID = id;
                Name = name;
                Description = description;
            }
        }
        public static List<CmdInfo> CmdData = new List<CmdInfo>()
        {
            new CmdInfo(0x00, "End", ""),
            new CmdInfo(0x01, "SetVar", "Stores the value retrieved by arg2 to the variable ID specified by arg1"),
            new CmdInfo(0x02, "SetVec2D", ""),
            new CmdInfo(0x03, "Label", ""),
            new CmdInfo(0x04, "Return", ""),
            new CmdInfo(0x05, "SearchLabel", ""),
            new CmdInfo(0x06, "If", ""),
            new CmdInfo(0x07, "IfNot", ""),
            new CmdInfo(0x08, "Else", ""),
            new CmdInfo(0x09, "EndIf", ""),
            new CmdInfo(0x0a, "SetStickRel", ""),
            new CmdInfo(0x0b, "SetButton", ""),
            new CmdInfo(0x0c, "VarAdd", ""),
            new CmdInfo(0x0d, "VarSub", ""),
            new CmdInfo(0x0e, "VarMul", ""),
            new CmdInfo(0x0f, "VarDiv", ""),
            new CmdInfo(0x10, "VecAdd", ""),
            new CmdInfo(0x11, "VecSub", ""),
            new CmdInfo(0x12, "VecMul", ""),
            new CmdInfo(0x13, "VecDiv", ""),
            new CmdInfo(0x14, "GoToCurrentLabel", ""),
            new CmdInfo(0x15, "SetVarRandf", ""),
            new CmdInfo(0x16, "Or", ""),
            new CmdInfo(0x17, "OrNot", ""),
            new CmdInfo(0x18, "And", ""),
            new CmdInfo(0x19, "AndNot", ""),
            new CmdInfo(0x1a, "SetFrame", ""),
            new CmdInfo(0x1b, "SetAct", ""),
            new CmdInfo(0x1c, "GoToLabel", ""),
            new CmdInfo(0x1d, "GetNearestCliffRel", ""),
            new CmdInfo(0x1e, "VarAbs", ""),
            new CmdInfo(0x1f, "SetStickAbs", ""),
            new CmdInfo(0x20, "Unk_20", ""),
            new CmdInfo(0x21, "Unk_21", ""),
            new CmdInfo(0x22, "SetWait", ""),
            new CmdInfo(0x23, "CliffCheck", ""),
            new CmdInfo(0x24, "CalcArriveFrameX", ""),
            new CmdInfo(0x25, "CalcArriveFrameY", ""),
            new CmdInfo(0x26, "GetShieldHP", ""),
            new CmdInfo(0x27, "StagePtRand", ""),
            new CmdInfo(0x28, "CalcArrivePosX", ""),
            new CmdInfo(0x29, "CalcArrivePosY", ""),
            new CmdInfo(0x2a, "AtkdDiceRoll", ""),
            new CmdInfo(0x2b, "Unk_2b", ""),
            new CmdInfo(0x2c, "Norm", ""),
            new CmdInfo(0x2d, "Dot", ""),
            new CmdInfo(0x2e, "CalcArrivePos_Sec", ""),
            new CmdInfo(0x2f, "Unk_2f", ""),
            new CmdInfo(0x30, "Unk_30", ""),
            new CmdInfo(0x31, "GetNearestCliffAbs", ""),
            new CmdInfo(0x32, "ClearStick", ""),
            new CmdInfo(0x33, "Unk_33", ""),//new to Smash 4
            new CmdInfo(0x34, "Unk_34", ""),
            new CmdInfo(0x35, "Unk_35", ""),
            new CmdInfo(0x36, "Unk_36", "Unused"),
            new CmdInfo(0x37, "Unk_37", ""),
            new CmdInfo(0x38, "Unk_38", ""),
            new CmdInfo(0x39, "Unk_39", ""),
        };

        public static Dictionary<UInt32, string> script_value_special = new Dictionary<UInt32, string>()
        {
            //0x1000 = par_work_update value. Range of some sort?
            //0x1001 = next par_work_update value
            {0x1002, "ai_lr" },
            {0x1003, "get_lr_tgt" },
            {0x1004, "ai_pos_xy" },//vector
            {0x1005, "tgt_pos_xy" },//vector
            //0x1006 = distance from ledge in direction of opponent?
            {0x1007, "timer" },
            {0x1008, "ai_spd_xy" },//vector
            {0x1009, "zero" },
            {0x100a, "one" },
            {0x100b, "ai_pos_y" },
            {0x100c, "tgt_pos_y" },
            {0x100d, "ai_spd_y" },
            {0x100e, "randf"},
            //0x100f = {Ground = 2, Air = 1}
            //0x1010 = distance from front ledge?
        };//maximum value = 0x103E

        public static List<string> buttons = new List<string>
        {
            "attack", "special", "shield", "jump"
        };

        public static Dictionary<UInt32, string> if_chks = new Dictionary<UInt32, string>()
        {
            {0x1002, "timer_passed" },
            {0x1005, "ai_aerial" },
            {0x1007, "greater" },
            {0x1008, "less" },
            {0x1009, "geq" },
            {0x100a, "leq" },
            {0x100d, "ai_off_stage" },
            {0x100f, "ai_action" },
            {0x101b, "tgt_aerial" },
            {0x101e, "ai_char" },
            {0x101f, "tgt_char" },
            {0x1024, "ai_subaction" }
        };

        public static List<string> fighters = new List<string>()
        {
            "miifighter",
            "miiswordsman",
            "miigunner",
            "mario",
            "donkey",
            "link",
            "samus",
            "yoshi",
            "kirby",
            "fox",
            "pikachu",
            "luigi",
            "captain",
            "ness",
            "peach",
            "koopa",
            "zelda",
            "sheik",
            "marth",
            "gamewatch",
            "ganon",
            "falco",
            "wario",
            "metaknight",
            "pit",
            "szerosuit",
            "pikmin",
            "diddy",
            "dedede",
            "ike",
            "lucario",
            "robot",
            "toonlink",
            "lizardon",
            "sonic",
            "purin",
            "mariod",
            "lucina",
            "pitb",
            "rosetta",
            "wiifit",
            "littlemac",
            "murabito",
            "palutena",
            "reflet",
            "duckhunt",
            "koopajr",
            "shulk",
            "gekkouga",
            "pacman",
            "rockman",
            "mewtwo",
            "ryu",
            "lucas",
            "roy",
            "cloud",
            "bayonetta",
            "kamui",
            "koopag",
            "warioman",
            "littlemacg",
            "lucariom",
            "miienemyf",
            "miienemys",
            "miienemyg"
        };
    }
}
