using System;
using System.Text.RegularExpressions;
static class Preproc{

    const string commentary_regex = @"\/\*(?s).*?\*\/";
    public static string Run(string code){
        string output = Regex.Replace(code, commentary_regex, "");
        
        return output;
    }

}