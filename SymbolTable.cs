using System;
using System.Collections.Generic;

public enum VarTypes{
    INT,
    BOOL,
    STRING
}
class Symbol{
    private dynamic value;
    private VarTypes tipo;
    public Symbol(dynamic value,VarTypes type){
        this.Value = value;
        this.Tipo = type;
    }

    public VarTypes Tipo { get => tipo; set => tipo = value; }
    public dynamic Value { get => value; set => this.value = value; }
}

static class FuncTable
{
    private static Dictionary<string,dynamic> table = new Dictionary<string,dynamic>();

    public static dynamic Get(string name){
        dynamic valor; 
        if(table.TryGetValue(name, out valor)){
            return valor;
        }else{
            throw new RaulException($"Function '{name}' is not defined");
        }
    }

    public static void Set(string name, dynamic value){
        if(table.ContainsKey(name)){
            throw new RaulException($"Function '{name}' has already been defined");
        }
        table.Add(name, value);
    }

}

class SymbolTable
{
    private Dictionary<string,Symbol> table = new Dictionary<string,Symbol>();

    public dynamic Get(string name){
        Symbol valor; 
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

    public bool Contains(string name){
        return table.ContainsKey(name);
    }
}