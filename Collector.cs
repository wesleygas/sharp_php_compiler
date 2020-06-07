using System;
using System.Collections.Generic;
using System.IO; 
static class Collector{
    static List<string> commands = new List<string>();


    public static void Collect(string instruction){
        commands.Add(instruction);
    }

    public static string Dump(){
        string header = File.ReadAllText("header.asm");
        string footer = File.ReadAllText("footer.asm");

        return header + "\n\t" + String.Join("\n\t", commands) + "\n" + footer;
    }

    public static void FileDump(string filename){
        string header = File.ReadAllText("header.asm");
        string footer = File.ReadAllText("footer.asm");
        string content = header + "\n\t" + String.Join("\n\t", commands) + "\n" + footer;
        File.WriteAllText(filename,content);
    }

}