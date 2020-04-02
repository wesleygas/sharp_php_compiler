using System;
using System.Collections.Generic;
class SymbolTable
{
    private Dictionary<string,dynamic> table = new Dictionary<string,dynamic>();

    public dynamic Get(string name){
        dynamic valor; 
        if(table.TryGetValue(name, out valor)){
            return valor;
        }else{
            throw new RaulException($"'{name}' is not defined");
        }
    }

    public void Set(string name, dynamic value){
        if(table.ContainsKey(name)){
            table.Remove(name);
        }
        table.Add(name, value);
    }
}