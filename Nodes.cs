using System;
using System.Collections.Generic;

abstract class Node{
    public dynamic value;

    public List<dynamic> children = new List<dynamic>();

    public abstract dynamic Evaluate(SymbolTable st);
}

class CommandBlock: Node {
    
    public void AddChild(dynamic child){
        children.Add(child);
    }
    public override dynamic Evaluate(SymbolTable st){
        foreach(dynamic child in children){
            child.Evaluate(st);
        }
        return null;
    }
}

class IfNode: Node{
    public IfNode(dynamic condition, dynamic cond_true){
        children.Add(condition);
        children.Add(cond_true);
    }
    public IfNode(dynamic condition, dynamic cond_true, dynamic cond_false){
        children.Add(condition);
        children.Add(cond_true);
        children.Add(cond_false);
    }

    public override dynamic Evaluate(SymbolTable st){
        if(Convert.ToBoolean(children[0].Evaluate(st).Value)){
            children[1].Evaluate(st);
        }else if(children.Count == 3){
            children[2].Evaluate(st);
        }
        return null;
    }
}

class WhileNode: Node{
    public WhileNode(dynamic condition, dynamic cond_true){
        children.Add(condition);
        children.Add(cond_true); 
    }

    public override dynamic Evaluate(SymbolTable st){
        while(Convert.ToBoolean(children[0].Evaluate(st).Value)){
            children[1].Evaluate(st);
        }
        return null;
    }

}

class Identifier: Node{

    public Identifier(string name){
        this.value = name;
    }
    public override dynamic Evaluate(SymbolTable st){
        return st.Get(value);
    }
}

class Assignment: Node{

    public  Assignment(Node firstborn, Node second_son){
        children.Add(firstborn);
        children.Add(second_son);    
    }
    public override dynamic Evaluate(SymbolTable st){
        st.Set(children[0].value,children[1].Evaluate(st));
        return null;
    }
}

class ReadLineNode: Node{
    public override dynamic Evaluate(SymbolTable st){
        string input = System.Console.ReadLine();
        int a = int.Parse(input);
        return new Symbol(a,VarTypes.INT);
    }
}
class Echo: Node{
    public Echo(dynamic value){
        this.value = value;
    }

    public override dynamic Evaluate(SymbolTable st){
        //Ele se vira com o tipo, pode dar problema depois
        System.Console.WriteLine(value.Evaluate(st).Value);
        return null;
    }
}

class  BinOp: Node {
    public BinOp(dynamic value, Node firstborn, Node second_son){
        this.value = value;
        children.Add(firstborn);
        children.Add(second_son);
    }
    public override dynamic Evaluate(SymbolTable st){
        Symbol a = children[0].Evaluate(st); //Force the return type to be an integer
        Symbol b = children[1].Evaluate(st);
        //Se n eh concat sai daqui string!
        if(!(value.ToString() == '.'.ToString() | value.ToString() == "EQEQUAL") && (a.Tipo == VarTypes.STRING | b.Tipo == VarTypes.STRING)){
            throw new Exception($"Semantic error: Cannot operate '{value}' with types {a.Tipo} and {b.Tipo}");
        }
        
        switch(value){
            case '+':{
                return new Symbol(Convert.ToInt32(a.Value) + Convert.ToInt32(b.Value),VarTypes.INT);
            }
            case '-':{
                return new Symbol(Convert.ToInt32(a.Value) - Convert.ToInt32(b.Value),VarTypes.INT);
            }
            case '*':{
                return new Symbol(Convert.ToInt32(a.Value) * Convert.ToInt32(b.Value),VarTypes.INT);
            }
            case '/':{
                return new Symbol(Convert.ToInt32(a.Value) / Convert.ToInt32(b.Value),VarTypes.INT);
            }
            case '.':{
                //concatenate
                return new Symbol(String.Concat(a.Value.ToString(),b.Value.ToString()), VarTypes.STRING);
            }
            case "AND":{
                return new Symbol((Convert.ToBoolean(a.Value) && Convert.ToBoolean(b.Value)), VarTypes.BOOL);
            }
            case "OR":{
                return new Symbol((Convert.ToBoolean(a.Value) || Convert.ToBoolean(b.Value)), VarTypes.BOOL);
            }case "EQEQUAL":{
                return new Symbol((Convert.ToInt32(a.Value) == Convert.ToInt32(b.Value)), VarTypes.BOOL);
            }case "GREATER":{
                return new Symbol((Convert.ToInt32(a.Value) > Convert.ToInt32(b.Value)), VarTypes.BOOL);
            }case "LESS":{
                return new Symbol((Convert.ToInt32(a.Value) < Convert.ToInt32(b.Value)), VarTypes.BOOL);
            } 
            default:{
                throw new Exception($"Invalid value incountered in binop: {value}");
            }
        }
    }
}

class  UnOp: Node {

    public UnOp(dynamic value, Node child){
        this.value = value;
        children.Add(child);
    }
    public override dynamic Evaluate(SymbolTable st){
        Symbol symb = children[0].Evaluate(st);
        if(symb.Tipo == VarTypes.STRING){
            throw new Exception($"Semantic error: Cannot operate '{value}' with type {symb.Tipo}");
        }
        switch(value){
            case '+':{
                return new Symbol(+ Convert.ToInt32(symb.Value),VarTypes.INT);
            }
            case '-':{
                return new Symbol(- Convert.ToInt32(symb.Value),VarTypes.INT);
            }
            case '!':{
                return new Symbol(!Convert.ToBoolean(symb.Value),VarTypes.BOOL);
            }
            default:{
                throw new Exception("Invalid value incountered in binop");
            }
        }
    }
}

class  IntVal: Node {

    public IntVal(int value){
        this.value = value;
    }
    public override dynamic Evaluate(SymbolTable st){
        return new Symbol(this.value, VarTypes.INT);
    }
}

class StrVal: Node {
    public StrVal(String value){
        this.value = value;
    }
    public override dynamic Evaluate(SymbolTable st){
        return new Symbol(this.value, VarTypes.STRING);
    }
}

class BoolVal: Node {

    public BoolVal(bool value){
        this.value = value;
    }
    public override dynamic Evaluate(SymbolTable st){
        return new Symbol(this.value, VarTypes.BOOL);
    }
}

class  NoOp: Node {

    public NoOp(){
        
    }
    public override dynamic Evaluate(SymbolTable st){
        return null;
    }
}
