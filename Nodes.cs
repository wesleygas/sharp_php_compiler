using System;
using System.Collections.Generic;

abstract class Node{
    public dynamic value;
    public int id;
    public List<dynamic> children = new List<dynamic>();

    public abstract dynamic Evaluate(SymbolTable st);

    public abstract void AsmEvaluate(SymbolTable st);
}

class CommandBlock: Node {
    
    public CommandBlock(int id){
        this.id = id;
    }
    public void AddChild(dynamic child){
        children.Add(child);
    }
    public override dynamic Evaluate(SymbolTable st){
        foreach(dynamic child in children){
            child.Evaluate(st);
        }
        return null;
    }

    public override void AsmEvaluate(SymbolTable st){
        foreach(dynamic child in children){
            child.AsmEvaluate(st);
        }
    }
}

class IfNode: Node{
    public IfNode(int id ,dynamic condition, dynamic cond_true){
        this.id = id;
        children.Add(condition);
        children.Add(cond_true);
    }
    public IfNode(int id, dynamic condition, dynamic cond_true, dynamic cond_false){
        this.id = id;
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

    public override void AsmEvaluate(SymbolTable st){
        //O filho seta EBX com o resultado da condicao
        children[0].AsmEvaluate(st);

        //Se a condicao for falsa, pula pro condfalse se tem filho, se nao, vai pro final
        Collector.Collect("CMP EBX, False"); 
        if(children.Count == 3) Collector.Collect($"JE CONDFALSE_{id}");
        else Collector.Collect($"JE EXIT_{id}");

        children[1].AsmEvaluate(st);

        Collector.Collect($"JMP EXIT_{id}");

        //Se tem else
        if(children.Count == 3){
            Collector.Collect($"CONDFALSE_{id}:");
            children[2].AsmEvaluate(st);
        }

        Collector.Collect($"EXIT_{id}:");
    }
}

class WhileNode: Node{
    public WhileNode(int id, dynamic condition, dynamic cond_true){
        this.id = id;
        children.Add(condition);
        children.Add(cond_true); 
    }

    public override dynamic Evaluate(SymbolTable st){
        while(Convert.ToBoolean(children[0].Evaluate(st).Value)){
            children[1].Evaluate(st);
        }
        return null;
    }

    public override void AsmEvaluate(SymbolTable st){
        //Label marcando o inicio do loop
        Collector.Collect($"LOOP_{id}:");

        //Faz o evaluate da condicao que retorna o resultado em EBX
        children[0].AsmEvaluate(st);

        Collector.Collect("CMP EBX, False"); 
        Collector.Collect($"JE EXIT_{id}");

        children[1].AsmEvaluate(st);

        Collector.Collect($"JMP LOOP_{id}");

        Collector.Collect($"EXIT_{id}:");
    }

}

class Identifier: Node{

    public Identifier(int id, string name){
        this.id = id;
        this.value = name;
    }
    public override dynamic Evaluate(SymbolTable st){
        return st.Get(value);
    }

    public override void AsmEvaluate(SymbolTable st){
        //Pega o valor que esta na pilha (com o offset da variavel de nome value) e coloca em EBX
        st.AsmGet(value);
    }
}

class Assignment: Node{

    public  Assignment(int id, Node firstborn, Node second_son){
        this.id = id;
        children.Add(firstborn);
        children.Add(second_son);    
    }
    public override dynamic Evaluate(SymbolTable st){
        st.Set(children[0].value,children[1].Evaluate(st));
        return null;
    }
    public override void AsmEvaluate(SymbolTable st){
        children[1].AsmEvaluate(st); //Guarda o seu valor em EBX
        st.AsmSet(children[0].value);//Guarda EBX no offset da variavel com esse nome
    }
}

class Echo: Node{
    public Echo(int id, dynamic value){
        this.id = id;
        this.value = value;
    }

    public override dynamic Evaluate(SymbolTable st){
        //Ele se vira com o tipo, pode dar problema depois
        System.Console.WriteLine(value.Evaluate(st).Value);
        return null;
    }

    public override void AsmEvaluate(SymbolTable st){
        //Faz o evaluate pra setar o valor a ser printado em EBX
        value.AsmEvaluate(st);
        Collector.Collect("PUSH EBX"); //Empilha argumento
        Collector.Collect("CALL print"); //chama procedure
        Collector.Collect("POP EBX"); //Desempilha argumento
    }
}

class  BinOp: Node {
    public BinOp(int id, dynamic value, Node firstborn, Node second_son){
        this.id = id;
        this.value = value;
        children.Add(firstborn);
        children.Add(second_son);
    }
    public override dynamic Evaluate(SymbolTable st){
        Symbol a = children[0].Evaluate(st); //Force the return type to be an integer
        Symbol b = children[1].Evaluate(st);
           
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

    public override void AsmEvaluate(SymbolTable st){
        children[0].AsmEvaluate(st);
        Collector.Collect("PUSH EBX");
        
        //B (filho da direita) -> EBX
        children[1].AsmEvaluate(st);

        //A (filho da esquerda) -> EAX
        Collector.Collect("POP EAX");

        switch(value){
            case '+':{
                // A + B
                Collector.Collect("ADD EAX, EBX");
                Collector.Collect("MOV EBX, EAX");
                break;
            }
            case '-':{
                // A - B
                Collector.Collect("SUB EAX, EBX");
                Collector.Collect("MOV EBX, EAX");
                break; 
            }
            case '*':{
                // A*B
                //So funciona pq o 'lower result' ja eh guardado em EAX
                Collector.Collect("IMUL EAX, EBX");
                Collector.Collect("MOV EBX, EAX");
                break; 
            }
            case '/':{
                // DIV divide EDX:EAX pelo argumento e salva em EAX
                // A/B
                Collector.Collect("MOV EDX, 0");
                Collector.Collect("IDIV EBX");
                Collector.Collect("MOV EBX, EAX");
                break; 
            }
            case "AND":{
                Collector.Collect("AND EAX, EBX");
                Collector.Collect("MOV EBX, EAX");
                break; 
            }
            case "OR":{
                Collector.Collect("OR EAX, EBX");
                Collector.Collect("MOV EBX, EAX");
                break; 
            }case "EQEQUAL":{
                Collector.Collect("CMP EAX, EBX");
                Collector.Collect("CALL binop_je");
                break; 
            }case "GREATER":{
                Collector.Collect("CMP EAX, EBX");
                Collector.Collect("CALL binop_jg");
                break; 
            }case "LESS":{
                Collector.Collect("CMP EAX, EBX");
                Collector.Collect("CALL binop_jl");
                break; 
            } 
            default:{
                throw new Exception($"Invalid value incountered in binop: {value}");
            }
        }

        


    }
}

class  UnOp: Node {

    public UnOp(int id, dynamic value, Node child){
        this.id = id;
        this.value = value;
        children.Add(child);
    }
    public override dynamic Evaluate(SymbolTable st){
        Symbol symb = children[0].Evaluate(st);
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

    public override void AsmEvaluate(SymbolTable st){
        children[0].AsmEvaluate(st);

        switch(value){
            case '+':{
                //abs(x) = (x XOR y) - y
                //where y = x >>> 31
                Collector.Collect("MOV EAX, EBX"); //y = x
                Collector.Collect("SAR EAX, 31");  //y >> 31
                Collector.Collect("XOR EBX, EAX"); //x = x XOR y 
                Collector.Collect("SUB EBX, EAX"); //x = x - y
                break;
            }
            case '-':{
                Collector.Collect("NEG EBX");
                break;
            }
            case '!':{
                Collector.Collect("NOT EBX");
                break;
            }
            default:{
                throw new Exception("Invalid value incountered in binop");
            }
        }

    }
}

class  IntVal: Node {

    public IntVal(int id, int value){
        this.id = id;
        this.value = value;
    }
    public override dynamic Evaluate(SymbolTable st){
        return new Symbol(this.value, VarTypes.INT);
    }

    public override void AsmEvaluate(SymbolTable st){
        Collector.Collect("MOV EBX, " + this.value.ToString());
    }
}


class BoolVal: Node {

    public BoolVal(int id, bool value){
        this.id = id;
        this.value = value;
    }
    public override dynamic Evaluate(SymbolTable st){
        return new Symbol(this.value, VarTypes.BOOL);
    }

    public override void AsmEvaluate(SymbolTable st){
        Collector.Collect("MOV EBX, " + this.value.ToString());
    }
}

class  NoOp: Node {

    public NoOp(int id){
        this.id = id;
    }
    public override dynamic Evaluate(SymbolTable st){
        return null;
    }

    public override void AsmEvaluate(SymbolTable st){
        Collector.Collect("NOP");
    }
}
