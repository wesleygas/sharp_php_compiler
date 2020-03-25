using System;
using System.Collections.Generic;

abstract class Node{
    public dynamic value;

    public List<dynamic> children = new List<dynamic>();

    public abstract dynamic Evaluate();
}

class  BinOp: Node {
    public BinOp(dynamic value, dynamic firstborn, dynamic second_son){
        this.value = value;
        children.Add(firstborn);
        children.Add(second_son);
    }

    public override dynamic Evaluate(){
        switch(value){
            case '+':{
            return children[0].Evaluate() + children[1].Evaluate();
        }
        case '-':{
            return children[0].Evaluate() - children[1].Evaluate();
        }
        case '*':{
            return children[0].Evaluate() * children[1].Evaluate();
        }
        case '/':{
            return children[0].Evaluate() / children[1].Evaluate();
        }
        default:{
            throw new Exception("Invalid value incountered in binop");
        }
        }
    }
}

class  UnOp: Node {

    public UnOp(dynamic value, dynamic child){
        this.value = value;
        children.Add(child);
    }
    public override dynamic Evaluate(){
        switch(value){
            case '+':{
                return +children[0].Evaluate();
            }
            case '-':{
                return -children[0].Evaluate();
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
    public override dynamic Evaluate(){
        return this.value;
    }
}

class  NoOp: Node {

    public NoOp(){
        
    }
    public override dynamic Evaluate(){
        return null;
    }
}
