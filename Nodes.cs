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

//*if, *while, *condition, *readline


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
        if(children[0].Evaluate(st) != 0){
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
        while(children[0].Evaluate(st) != 0){
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
        return a;
    }
}
class Echo: Node{
    public Echo(dynamic value){
        this.value = value;
    }

    public override dynamic Evaluate(SymbolTable st){
        System.Console.WriteLine(value.Evaluate(st));
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
        int a = children[0].Evaluate(st); //Force the return type to be an integer
        int b = children[1].Evaluate(st);
        
        switch(value){
            case '+':{
            return a + b;
            }
            case '-':{
                return a - b;
            }
            case '*':{
                return a * b;
            }
            case '/':{
                return a / b;
            }
            case "AND":{
                return ((a!=0) && (b!=0)) ? 1 : 0;
            }
            case "OR":{
                return ((a!=0) || (b!=0)) ? 1 : 0;
            }case "EQEQUAL":{
                return (a == b) ? 1 : 0;
            }case "GREATER":{
                return (a > b) ? 1 : 0;
            }case "LESS":{
                return (a < b) ? 1 : 0;
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
        int a = children[0].Evaluate(st);
        switch(value){
            case '+':{
                return +a;
            }
            case '-':{
                return -a;
            }
            case '!':{
                return (a == 0) ? 1 : 0;
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
        return this.value;
    }
}

class  NoOp: Node {

    public NoOp(){
        
    }
    public override dynamic Evaluate(SymbolTable st){
        return null;
    }
}
