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

//*Assignment, *echo, *identifier 

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
        default:{
            throw new Exception("Invalid value incountered in binop");
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
