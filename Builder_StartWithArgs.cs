﻿using System;
using QuickTools.QData;
using QuickTools.QCore;
using System.IO;
using QuickTools.QIO;

namespace ClownShellSourcesBuilder
{
    public partial class Builder
    {

        public void StartWithArgs()
        {

            this.Load();
            Key key; 
            this.QKeyManager.AllowDebugger = this.AllowDebugger;
            this.BinBuilder.AllowDeubbuger = this.AllowDebugger;

            for (int item = 0; item < QKeyManager.Keys.Count; item++)
            {
                key = QKeyManager.Keys[item];
                //Get.Wait(key);
                switch (key.Name)
                {
                    //DEBUGGER
                    case "DEBUGGER":
                        this.AllowDebugger = key.Value == "ON";
                        this.QKeyManager.AllowDebugger = this.AllowDebugger;
                        this.BinBuilder.AllowDeubbuger = this.AllowDebugger;
                        break;
                    case "VAR":
                        this.Stack.SetVariable(key.Value, "NULL");
                        break;
                    case "FUNCTION":
                        this.RunCommand(key.Value);
                        break;
                    case "BIN_PATH":
                        if(this.AllowDebugger)Get.Green($"BIN PATH DETECTED: [{key.Value}]");
                        if (!Directory.Exists(key.Value)) { Get.Red($"DIRECTORY NOT FOUND: [{key.Value}]"); return; }
                        this.BinBuilder.Source = key.Value;
                        break;
                    case "BRANCH":
                        this.BufferPackage.Branch = key.Value;
                        break;
                    case "NAME":
                        if (!this.IsDeleting)
                        {
                            if (this.AllowDebugger)Get.Green($"PACKAGE NAME DETECTED: [{key.Value}]");
                            BufferPackage.Name = key.Value;
                            break;
                        }
                        this.Delete(key.Value); 
                        break;
                    case "SOURCE_URL":
                        if (this.AllowDebugger) Get.Green($"PACKAGE SOURCE URL DETECTED: [{key.Value}]");
                        BufferPackage.Source = key.Value;
                        break;
                    case "CREATOR":
                        if (this.AllowDebugger) Get.Green($"PACKAGE CREATOR DETECTED: [{key.Value}]");
                        BufferPackage.Creator = key.Value;
                        break;
                    case "DESCRIPTION":
                        if (this.AllowDebugger) Get.Green($"PACKAGE DESCRIPTION DETECTED: [{key.Value}]");
                        BufferPackage.Description = key.Value;
                        break;
                    case "SOURCES_FILE":
                        bool exist = File.Exists(key.Value);
                        if (this.AllowDebugger) Get.Green($"PACKAGE SOURCES FILE DETECTED: [{key.Value}] EXIST: [{exist}]");
                        this.BinBuilder.FileName = key.Value;
                        if (exist) this.MakeBackup(); 
                        break;
                    default:
                        //$VARIABLE=/P/D/D/F;
                        if (key.Name[0] == '$')
                        {
                            string v = key.Name.Replace("$","");

                            this.Stack.UpdateVariable(v, key.Value);
                            if (this.AllowDebugger) Get.Print(v, key.Value);
                            break;
                        }
                        if (key.Name.Contains("(") &&
                        key.Name.Contains(")"))
                        {
                            this.CallFunction(ref key);
                            break;
                        }
                        else
                        {
                            throw new Exception($"INVALID FUNCTION DETECTED: [{key.Name}]\nFULL INPUT: {key}");
                        }
                }
                //Get.WaitTime(500);
            }

        }

        private void MakeBackup()
        {
            string file = this.BinBuilder.FileName; 
            if (this.AllowDebugger) Get.Yellow($"MAKING BACKUP FOR: [{file}]");
            string buffer = File.ReadAllText(file);
            File.WriteAllText($"{file}.backup", buffer);
        }
    }
}
