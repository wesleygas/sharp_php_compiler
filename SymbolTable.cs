using System;
using System.Collections.Generic;

public enum VarTypes{
    INT,
    BOOL,
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

class SymbolTable
{
    int currentOffset = 4;
    private Dictionary<string,Symbol> table = new Dictionary<string,Symbol>();
    private Dictionary<string,int> asmTable = new Dictionary<string,int>();
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

    public void AsmGet(string name){
        int varOffset = 0;
        if(asmTable.TryGetValue(name, out varOffset)){
            //Pega o valor da stack e bota em EBX pra ser usado
            Collector.Collect($"MOV EBX, [EBP-{varOffset}] ; recupera {name}");
        }else{
            throw new RaulException($"'{name}' is not defined");
        }
    }

    public void AsmSet(string name){
        int varOffset = 0;
        //Se ja existe essa var na symbol table
        if(asmTable.TryGetValue(name, out varOffset)){
            //Coloca o valor no offset da variavel
            Collector.Collect($"MOV [EBP-{varOffset}], EBX ; recupera {name}");
        }else{
            //Alocar espaço na pilha
            Collector.Collect("PUSH DWORD 0");
            //Guardar o offset dessa variavel
            asmTable.Add(name, currentOffset);
            //Guardar o valor da variavel na pilha
            Collector.Collect($"MOV [EBP-{currentOffset}], EBX ; aloca e empilha {name}");
            currentOffset+=4;
        }
        
    }
}