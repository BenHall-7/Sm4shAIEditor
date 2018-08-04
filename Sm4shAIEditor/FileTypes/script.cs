﻿using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;

//Some assistance with the file format is credit to Sammi Husky

namespace Sm4shAIEditor
{
    class script
    {
        public UInt32 actCount { get; set; }
        public Dictionary<Act, UInt32> acts { get; set; }

        public script(string binDirectory)
        {
            acts = new Dictionary<Act, uint>();

            BinaryReader binReader = new BinaryReader(File.OpenRead(binDirectory));
            binReader.BaseStream.Seek(0x4, SeekOrigin.Begin);
            actCount = util.ReadReverseUInt32(binReader);
            for (int i = 0; i < actCount; i++)
            {
                binReader.BaseStream.Seek(i * 4 + 0x10, SeekOrigin.Begin);
                UInt32 actOffset = util.ReadReverseUInt32(binReader);
                Act act = new Act(binReader, actOffset);

                acts.Add(act, actOffset);
            }

            binReader.Close();
        }
        public script(Dictionary<uint, string> decompiledActs)
        {
            acts = new Dictionary<Act, uint>();

            List<script.Act> actData = new List<script.Act>();
            foreach (var act in decompiledActs)
            {
                actData.Add(new Act(act.Key, act.Value));
            }
            actCount = (uint)actData.Count;
            uint offset = util.Align0x10(0x10 + (4 * actCount));
            foreach (script.Act act in actData)
            {
                acts.Add(act, offset);
                offset += act.GetSize();
            }
        }

        public class Act
        {
            public UInt32 ID { get; set; }
            public UInt32 ScriptOffset { get; set; }
            public UInt32 ScriptFloatOffset { get; set; }
            public UInt16 VarCount { get; set; }
            public Dictionary<UInt32, float> ScriptFloats { get; set; }

            public List<Cmd> CmdList { get; set; }

            public Act(BinaryReader binReader, UInt32 actPosition)
            {
                binReader.BaseStream.Seek(actPosition, SeekOrigin.Begin);
                ID = util.ReadReverseUInt32(binReader);
                ScriptOffset = util.ReadReverseUInt32(binReader);
                ScriptFloatOffset = util.ReadReverseUInt32(binReader);
                VarCount = util.ReadReverseUInt16(binReader);
                binReader.BaseStream.Seek(ScriptOffset + actPosition, SeekOrigin.Begin);

                ScriptFloats = new Dictionary<uint, float>();

                //Commands
                CmdList = new List<Cmd>();
                UInt32 relOffset = ScriptOffset;
                while (relOffset < ScriptFloatOffset)
                {
                    Cmd cmd = new Cmd(binReader);
                    CmdList.Add(cmd);

                    //add values to the script float list
                    foreach (UInt32 cmdParam in cmd.ParamList)
                    {
                        if (cmdParam >= 0x2000 &&
                            cmdParam < 0x2100 &&
                            !ScriptFloats.ContainsKey(cmdParam) &&
                            cmd.ID != 0x1b)
                        {
                            ScriptFloats.Add(cmdParam, GetScriptFloat(binReader, cmdParam, actPosition, ScriptFloatOffset));
                        }
                    }

                    relOffset += cmd.Size;
                }
            }
            //for compiling
            public Act(UInt32 ID, string text)
            {
                this.ID = ID;
                ScriptOffset = 0x10;//since every act and header is 0x10 aligned this is guaranteed to be 0x10
                ScriptFloatOffset = ScriptOffset;//temporarily set to this; every command increases the offset
                CmdList = new List<Cmd>();
                ScriptFloats = new Dictionary<uint, float>();

                CustomStringReader sReader = new CustomStringReader(text);
                bool isIfArg = false;
                while (!sReader.EndString)
                {
                    Cmd cmd = new Cmd(sReader, ref isIfArg, this);
                    cmd.ParamCount = (byte)cmd.ParamList.Count;
                    cmd.Size = (UInt16)(4 + (4 * cmd.ParamCount));
                    CmdList.Add(cmd);
                    sReader.SkipWhiteSpace();
                    ScriptFloatOffset += cmd.Size;
                }
            }

            public class Cmd
            {
                private Act parent;

                public byte ID { get; set; }
                public byte ParamCount { get; set; }
                public UInt16 Size { get; set; }

                public List<UInt32> ParamList { get; set; }

                public Cmd(BinaryReader binReader)
                {
                    ID = binReader.ReadByte();
                    ParamCount = binReader.ReadByte();
                    Size = util.ReadReverseUInt16(binReader);
                    ParamList = new List<UInt32>(ParamCount);
                    int readParams = 0;
                    while (readParams < ParamCount)
                    {
                        ParamList.Add(util.ReadReverseUInt32(binReader));
                        readParams++;
                    }
                }
                public Cmd(byte id, List<UInt32> paramList)
                {
                    ID = id;
                    ParamList = paramList;
                    ParamCount = (byte)paramList.Count;
                    Size = (UInt16)(ParamCount * 4 + 4);
                }
                public Cmd(CustomStringReader sReader, ref bool isIfArg, Act parent)
                {
                    this.parent = parent;
                    ParamList = new List<uint>();
                    string word;
                    bool canHaveArgs = true;
                    if (isIfArg)//parsing the if statements for their args
                    {
                        word = sReader.ReadIfSymbols();
                        if (word == "||")
                            ID = 0x16;
                        else if (word == "&&")
                            ID = 0x18;
                        else if (word == null)
                        {
                            string nextChar = sReader.ReadChar();
                            if (nextChar == ")")
                                isIfArg = false;
                            else
                                throw new Exception();//syntax error: expected '&&', '||', or ')'
                        }

                        if (isIfArg)
                        {
                            sReader.SkipWhiteSpace();
                            if (sReader.ReadChar() == "!")
                                ID++;
                            else
                                sReader.Position--;

                            ConvertIfParams(sReader);
                        }
                        else
                            sReader.ReadUntilAnyOfChars("{", true);
                    }
                    if (!isIfArg)
                    {
                        if (parent.CmdList.Count > 0 && parent.CmdList[parent.CmdList.Count - 1].ID == 8)//if last command was Else
                        {
                            sReader.SkipWhiteSpace();
                            if (sReader.ReadChar() != "{")//basically we just want to skip to past the { sign so we can read more commands
                                sReader.Position--;//if Else is followed by a real command, don't skip the first letter
                        }
                        word = sReader.ReadWord();
                        if (word == null)
                        {
                            string nextChar = sReader.ReadChar();
                            if (nextChar == "}")
                            {
                                sReader.SkipWhiteSpace();
                                int tempPosition = sReader.Position;
                                word = sReader.ReadWord();
                                if (word != script_data.cmds[8])//if next command isn't Else
                                {
                                    ID = 9;//then the } represents an Endif command
                                    sReader.Position = tempPosition;//move the position back so the next loop reads the next command
                                }
                                else
                                    ID = 8;//if next command is Else, then the command ID skips straight to Else
                                canHaveArgs = false;
                            }
                        }
                        else if (script_data.cmds.Contains(word))
                        {
                            ID = (byte)script_data.cmds.IndexOf(word);
                            if (ID == 6)//if statement
                            {
                                sReader.ReadUntilAnyOfChars("(", true);
                                if (sReader.ReadChar() == "!")
                                    ID++;//IfNot
                                else
                                    sReader.Position--;

                                ConvertIfParams(sReader);
                                sReader.SkipWhiteSpace();
                                string nextChar = sReader.ReadChar();
                                if (nextChar != ")")
                                {
                                    sReader.Position--;
                                    isIfArg = true;
                                }
                                else
                                {
                                    isIfArg = false;
                                    canHaveArgs = false;
                                    sReader.ReadUntilAnyOfChars("{", true);
                                }
                            }
                            else if (ID == 8 || ID == 9)
                                canHaveArgs = false;
                        }
                        else if (word.StartsWith("var") || word.StartsWith("vec"))
                        {
                            ParamList.Add(UInt32.Parse(word.Substring(3)));//the numeric ID is the first arg
                            string op = sReader.ReadEqnSymbols();//operation
                            bool isVec;
                            if (word.Substring(0, 3) == "vec")
                                isVec = true;
                            else
                                isVec = false;

                            switch (op)//set command ID
                            {
                                case "=":
                                    ID = 1;
                                    break;
                                case "+=":
                                    ID = 0xc;
                                    break;
                                case "-=":
                                    ID = 0xd;
                                    break;
                                case "*=":
                                    ID = 0xe;
                                    break;
                                case "/=":
                                    ID = 0xf;
                                    break;
                                default:
                                    throw new Exception(string.Format("unrecognized variable assignment operator {0}", op));
                            }
                            if (isVec)
                            {
                                if (ID >= 0xc)
                                    ID += 4;
                                else
                                    ID++;
                            }
                            parent.UpdateVarCount(uint.Parse(word.Substring(3)), isVec);
                        }
                        else throw new Exception(string.Format("Unrecognized command {0}", word));
                        if (!isIfArg && canHaveArgs)
                            ConvertCmdParams(sReader);
                    }
                }

                private void ConvertCmdParams(CustomStringReader sReader)
                {
                    switch (ID)
                    {
                        case 0x01:
                        case 0x02:
                            ParamList.Add(parent.GetScriptValueID(sReader.ReadWord()));
                            break;
                        case 0x0b://button
                            uint arg = 0;
                            while (true)
                            {
                                sReader.ReadUntilAnyOfChars("(", true);
                                string word = sReader.ReadWord();
                                sReader.SkipWhiteSpace();
                                string append = sReader.ReadChar();
                                if (word == null)
                                    throw new Exception("SetButton argument cannot be null");
                                if (!script_data.buttons.Contains(word))
                                    throw new Exception(string.Format("unrecognized button {0}", word));
                                uint buttonID = (uint)(1 << script_data.buttons.IndexOf(word));
                                arg |= buttonID;
                                if (append != "+" && append != "," && append != ")")
                                    throw new Exception(string.Format("syntax error in SetButton args: {0}", append));
                                if (append == ")") break;
                            }
                            ParamList.Add(arg);
                            break;
                        case 0x0c://assignment for vars and vecs
                        case 0x0d:
                        case 0x0e:
                        case 0x0f:
                        case 0x10:
                        case 0x11:
                        case 0x12:
                        case 0x13:
                            while (true)
                            {
                                string word = sReader.ReadWord();
                                if (word == null) throw new Exception("variable assignment argument cannot be null");
                                ParamList.Add(parent.GetScriptValueID(word));

                                sReader.SkipWhiteSpace();//spaces won't cause exceptions here /shrug

                                string append = sReader.ReadChar();
                                if (append != ",")
                                {
                                    if (append != null) sReader.Position--;
                                    break;
                                }
                            }
                            break;
                        case 0x1e:
                            sReader.ReadUntilAnyOfChars("(", true);
                            while (true)
                            {
                                string word = sReader.ReadWord();
                                sReader.SkipWhiteSpace();
                                if (word == null || !word.StartsWith("var"))
                                    throw new Exception(string.Format("invalid argument in VarAbs command: {0}", word));
                                uint varID = uint.Parse(word.Substring(3));
                                ParamList.Add(varID);
                                parent.UpdateVarCount(varID, false);
                                string append = sReader.ReadChar();
                                if (append != ",")
                                {
                                    if (append == ")") break;
                                    else throw new Exception(string.Format("syntax error in VarAbs args: {0}", append));
                                }
                            }
                            break;
                        default:
                            sReader.ReadUntilAnyOfChars("(", true);
                            int readParams = 0;
                            while (true)
                            {
                                string word = sReader.ReadWord();
                                string append = sReader.ReadChar();
                                if (word == null)
                                {
                                    if (append == ",")
                                        throw new Exception(string.Format("null argument in {0}", script_data.cmds[ID]));
                                    else if (append != ")")
                                        throw new Exception(string.Format("syntax error in {0} args: {1}", script_data.cmds[ID], append));
                                }
                                else
                                {
                                    int listIndex = parent.GetCorrectIndexToCmdArgList(ID);
                                    int argNumber = readParams + 1;
                                    int type = 0;
                                    if (listIndex != -1 && argNumber < script_data.cmd_args[listIndex].Length)
                                    {
                                        type = script_data.cmd_args[listIndex][argNumber];
                                    }
                                    ParamList.Add(parent.GetParamIDFromType(word, type));
                                    
                                    readParams++;
                                }

                                if (append == ")") break;
                            }
                            break;
                    }
                }

                private void ConvertIfParams(CustomStringReader sReader)
                {
                    string word = sReader.ReadWord();
                    UInt32 reqID = 0;
                    if (word.StartsWith("req_"))
                    {
                        reqID = UInt32.Parse(word.Substring(4), System.Globalization.NumberStyles.HexNumber);
                    }
                    else if (script_data.if_chks.ContainsValue(word))
                    {
                        //get the Key used to index the value. This is probably inefficient and will need to be changed in the future
                        foreach (UInt32 key in script_data.if_chks.Keys)
                        {
                            if (script_data.if_chks[key] == word)
                            {
                                reqID = key;
                                break;
                            }
                        }
                    }
                    else
                        throw new Exception(string.Format("unrecognized 'requirement' {0}", word));

                    ParamList.Add(reqID);
                    if (sReader.ReadChar() != "(")
                        throw new Exception("syntax error: requirement ID must be followed by parentheses");

                    bool readNextArg = true;
                    while (readNextArg)
                    {
                        word = sReader.ReadWord();
                        if (script_data.if_chk_args.ContainsKey(reqID) && word != null)
                        {
                            switch (script_data.if_chk_args[reqID])
                            {
                                case 0:
                                    ParamList.Add(parent.GetScriptValueID(word));
                                    break;
                                case 1:
                                    ParamList.Add((uint)script_data.fighters.IndexOf(word));
                                    break;
                            }
                        }
                        else
                        {
                            if (word != null)
                            {
                                if (word.StartsWith("0x"))
                                    ParamList.Add(uint.Parse(word.Substring(2), System.Globalization.NumberStyles.HexNumber));
                                else
                                    ParamList.Add(uint.Parse(word));
                            }
                        }

                        string nextChar = sReader.ReadChar();
                        if (nextChar == ")")
                            readNextArg = false;
                        else if (nextChar != ",")
                            throw new Exception(string.Format("invalid syntax in If command: {0}", nextChar));
                    }
                }
            }

            public string DecompAct()
            {
                string text = "";
                byte lastCmdID = 0xff;
                int ifNestLevel = 0;
                byte relID = 0xFF;
                for (int cmdIndex = 0; cmdIndex < CmdList.Count; cmdIndex++)
                {
                    script.Act.Cmd cmd = CmdList[cmdIndex];

                    //control the nested level spaces
                    string ifPadding = "";
                    if (cmd.ID == 8 || cmd.ID == 9)
                        ifNestLevel--;
                    for (int i = 0; i < ifNestLevel; i++)
                    {
                        ifPadding += "\t";
                    }
                    //account for the "else if" statement, which messes up the nest level
                    if (((cmd.ID == 6 || cmd.ID == 7) && lastCmdID != 8) || cmd.ID == 8)
                        ifNestLevel++;

                    string cmdString = "";
                    string cmdParams = "";
                    switch (cmd.ID)
                    {
                        case 0x01://SetVar, uses notation [varX = Y]
                            cmdString += "var" + cmd.ParamList[0] + " = ";
                            cmdParams += GetScriptValue(cmd.ParamList[1]);
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x02://SetVec, uses notation [vecX = Y]
                            cmdString += "vec" + cmd.ParamList[0] + " = ";
                            cmdParams += GetScriptValue(cmd.ParamList[1]);
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x06://If
                        case 0x07://IfNot
                            cmdString += script_data.cmds[0x6] + "(";
                            if (cmd.ID == 0x7)
                                cmdString += "!";
                            int cmdAfterIndex = 1;
                            while (cmdIndex + cmdAfterIndex < CmdList.Count)
                            {
                                script.Act.Cmd cmdCurr = CmdList[cmdIndex + cmdAfterIndex - 1];
                                cmdParams += GetIfChk(cmdCurr.ParamList.ToArray());
                                //commands 0x16 to 0x19 (Or + OrNot + And + AndNot)
                                //believe it or not this next check is actually what the source code does
                                relID = (byte)(CmdList[cmdIndex + cmdAfterIndex].ID - 0x16);
                                if (relID <= 3)
                                {
                                    cmdParams += " ";
                                    if (relID / 2 == 0)
                                        cmdParams += "|| ";
                                    else
                                        cmdParams += "&& ";

                                    if (relID % 2 != 0)
                                        cmdParams += "!";
                                    cmdAfterIndex++;
                                }
                                else
                                {
                                    cmdIndex += cmdAfterIndex - 1;
                                    break;
                                }
                            }
                            cmdString += cmdParams + ") {" + "\r\n";
                            if (lastCmdID != 0x8)
                                text += ifPadding;
                            text += cmdString;
                            break;
                        case 0x08://Else
                            cmdString += ifPadding + "}" + "\r\n" + ifPadding;
                            //if next command is an "if" or "ifNot" don't put it on a separate line
                            if (CmdList[cmdIndex + 1].ID == 0x6 || CmdList[cmdIndex + 1].ID == 0x7)
                                cmdString += script_data.cmds[cmd.ID] + " ";
                            else
                                cmdString += script_data.cmds[cmd.ID] + " {" + "\r\n";
                            text += cmdString;
                            break;
                        case 0x09://EndIf
                            cmdString += "}" + "\r\n";//use the symbol instead of the name
                            text += ifPadding + cmdString;
                            break;
                        case 0x0b://SetButton
                            cmdString += script_data.cmds[cmd.ID] + "(";
                            List<string> cmdButtons = new List<string>();
                            //generate buttons from command
                            for (int i = 0; i < 4; i++)
                            {
                                int mask = 1 << i;
                                if ((cmd.ParamList[0] & mask) == mask)
                                {
                                    cmdButtons.Add(script_data.buttons[i]);
                                }
                            }
                            //write out button list
                            for (int i = 0; i < cmdButtons.Count; i++)
                            {
                                if (i != 0)
                                    cmdParams += "+";//surprisingly never used in vanilla smash 4 scripts, but we should support it
                                cmdParams += cmdButtons[i];
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x0c://var operators
                        case 0x0d:
                        case 0x0e:
                        case 0x0f:
                        case 0x10://vec operators
                        case 0x11:
                        case 0x12:
                        case 0x13:
                            relID = (byte)(cmd.ID - 0xc);
                            if (relID < 4)
                                cmdString += "var" + cmd.ParamList[0];
                            else
                                cmdString += "vec" + cmd.ParamList[0];

                            if (relID % 4 == 0)
                                cmdString += " += ";
                            else if (relID % 4 == 1)
                                cmdString += " -= ";
                            else if (relID % 4 == 2)
                                cmdString += " *= ";
                            else
                                cmdString += " /= ";

                            for (int i = 1; i < cmd.ParamCount; i++)
                            {
                                cmdParams += GetScriptValue(cmd.ParamList[i]);

                                if (i != cmd.ParamCount - 1)
                                    cmdParams += ", ";
                            }
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x1e://VarAbs, which supports arbitrary number of args
                            cmdString += script_data.cmds[cmd.ID] + "(";
                            for (int i = 0; i < cmd.ParamCount; i++)
                            {
                                cmdParams += "var" + cmd.ParamList[i];

                                if (i != cmd.ParamCount - 1)
                                    cmdParams += ", ";
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        default:
                            cmdString += script_data.cmds[cmd.ID] + "(";
                            string parsed = ParseCmdParams(cmd);
                            if (parsed != null)
                            {
                                cmdParams += parsed;
                            }
                            else
                            {
                                //hopefully I get to the point where this becomes obsolete
                                for (int i = 0; i < cmd.ParamCount; i++)
                                {
                                    cmdParams += "0x" + cmd.ParamList[i].ToString("X");

                                    if (i != cmd.ParamCount - 1)
                                        cmdParams += ", ";
                                }
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                    }
                    lastCmdID = cmd.ID;
                }

                return text;
            }

            public string ParseCmdParams(Cmd cmd)
            {
                int correctIndex = GetCorrectIndexToCmdArgList(cmd.ID);
                if (correctIndex == -1)
                    return null;
                else
                {
                    string cmdParams = "";
                    for (int i = 0; i < cmd.ParamCount; i++)
                    {
                        int argIndex = i + 1;
                        byte type;
                        if (argIndex < script_data.cmd_args[correctIndex].Length)
                            type = script_data.cmd_args[correctIndex][argIndex];
                        else
                            type = 0;

                        switch (type)
                        {
                            case 0:
                                cmdParams += "0x" + cmd.ParamList[i].ToString("X");
                                break;
                            case 1:
                                cmdParams += "var" + cmd.ParamList[i];
                                break;
                            case 2:
                                cmdParams += "vec" + cmd.ParamList[i];
                                break;
                            case 3:
                                cmdParams += GetScriptValue(cmd.ParamList[i]);
                                break;
                            default:
                                break;
                        }
                        if (i != cmd.ParamCount - 1)
                            cmdParams += ", ";
                    }
                    return cmdParams;
                }
            }

            public uint GetSize()
            {
                return ScriptFloatOffset + (4 * (uint)ScriptFloats.Count);
            }

            public UInt32 GetParamIDFromType(string param, int type)
            {
                UInt32 id;
                switch (type)
                {
                    case 0:
                        if (param.StartsWith("0x"))
                            id = UInt32.Parse(param.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        else
                            id = UInt32.Parse(param);
                        break;
                    case 1:
                    case 2:
                        if (!param.StartsWith("var") && !param.StartsWith("vec"))
                            throw new Exception(string.Format("param type {0} was neither a variable nor a vector", param));
                        id = UInt32.Parse(param.Substring(3));
                        UpdateVarCount(id, param.StartsWith("vec"));
                        break;
                    case 3:
                        id = GetScriptValueID(param);
                        break;
                    default:
                        throw new Exception("Invalid param type");
                }
                return id;
            }

            public int GetCorrectIndexToCmdArgList(int cmdID)
            {
                int correctIndex = -1;
                //can this be made faster?
                for (int i = 0; i < script_data.cmd_args.Count; i++)
                {
                    if (cmdID == script_data.cmd_args[i][0])
                    {
                        correctIndex = i;
                        break;
                    }
                }
                return correctIndex;
            }

            protected float GetScriptFloat(BinaryReader binReader, UInt32 cmdParam, UInt32 actPosition, UInt32 floatOffset)
            {
                float scriptFloat;
                Int32 binPosition = (Int32)binReader.BaseStream.Position;
                cmdParam -= 0x2000;
                binReader.BaseStream.Seek(actPosition + floatOffset + cmdParam * 4, SeekOrigin.Begin);
                scriptFloat = util.ReadReverseFloat(binReader);
                binReader.BaseStream.Seek(binPosition, SeekOrigin.Begin);
                return scriptFloat;
            }

            public void AddScriptFloat(float value)
            {
                if (!ScriptFloats.ContainsValue(value))
                {
                    UInt32 nextFloatID = (UInt32)(ScriptFloats.Keys.Count + 0x2000);
                    ScriptFloats.Add(nextFloatID, value);
                }
            }

            public string GetScriptValue(UInt32 paramID)
            {
                if (paramID < 0x1000)
                    return "var" + paramID;
                if (paramID >= 0x2000 && ScriptFloats.ContainsKey(paramID))
                    return ScriptFloats[paramID].ToString();
                else
                {
                    string value;
                    if (script_data.script_value_special.ContainsKey(paramID))
                        value = script_data.script_value_special[paramID];
                    else
                        value = "0x" + paramID.ToString("X4");

                    return value;
                }
            }

            public UInt32 GetScriptValueID(string param)
            {
                UInt32 ID = 0;
                if (param.StartsWith("0x"))
                {
                    ID = UInt32.Parse(param.Substring(2), System.Globalization.NumberStyles.HexNumber);
                }
                else if (float.TryParse(param, out float value))
                {
                    AddScriptFloat(value);
                    foreach (UInt32 floatID in ScriptFloats.Keys)
                    {
                        if (value == ScriptFloats[floatID])
                        {
                            ID = floatID;
                            break;
                        }
                    }
                }
                else if (param.StartsWith("var") || param.StartsWith("vec"))
                {
                    ID = UInt32.Parse(param.Substring(3));
                }
                else if (script_data.script_value_special.ContainsValue(param))
                {
                    foreach (UInt32 valueID in script_data.script_value_special.Keys)
                    {
                        if (param == script_data.script_value_special[valueID])
                        {
                            ID = valueID;
                            break;
                        }
                    }
                }
                else
                    throw new Exception(string.Format("unrecognized script_value {0}", param));

                return ID;
            }

            public string GetIfChk(UInt32[] cmdParams)
            {
                UInt32 reqID = cmdParams[0];
                string requirement = "";
                if (script_data.if_chks.ContainsKey(reqID))
                {
                    requirement += script_data.if_chks[reqID];
                }
                else
                {
                    requirement += "req_" + reqID.ToString("X");
                }
                requirement += "(";
                for (int i = 1; i < cmdParams.Length; i++)
                {
                    UInt32 currentParam = cmdParams[i];
                    if (script_data.if_chk_args.ContainsKey(reqID))
                    {
                        switch (script_data.if_chk_args[reqID])
                        {
                            case 0:
                                requirement += GetScriptValue(currentParam);
                                break;
                            case 1:
                                requirement += script_data.fighters[(int)currentParam];
                                break;
                        }
                    }
                    else
                    {
                        requirement += "0x" + currentParam.ToString("X");
                    }
                    //deal with adding commas for multiple args
                    if (i != cmdParams.Length - 1)
                        requirement += ", ";
                }
                return requirement + ")";
            }

            public void UpdateVarCount(uint varID, bool isVec)
            {
                if (isVec)
                    varID++;
                //if (varID > 24)
                //    throw new Exception("variable count exceeded maximum");
                if (varID > VarCount)
                    VarCount = (ushort)(varID + 1);
            }
        }//end of Act class
    }//end of Script class
}
