using System;

static class Parser {
    static Tokenizer tokens;

    static Node ParseProgram(){
        Node node;
        if(tokens.Current.Type != TokenTypes.START_PROG) throw new RaulException($"Expected '<?php' at {tokens.Position}");
        tokens.SelectNext();
        if(tokens.Current.Type != TokenTypes.END_PROG){
            node = ParseCommand(); 
            tokens.SelectNext();
        }else{
            node = new CommandBlock();
            tokens.SelectNext();
        }
        return node;
    }
    static Node ParseBlock(){
        CommandBlock node = new CommandBlock();
        if(tokens.Current.Type == TokenTypes.LBRACE){
            tokens.SelectNext();
            while(tokens.Current.Type != TokenTypes.RBRACE){
                node.AddChild(ParseCommand());     
            }
            tokens.SelectNext();
        }else{
            throw new RaulException($"Expected '{{' at {tokens.Position}");
        }
        return node;
    }

    static Node ParseCommand(){
        Node node;
        switch(tokens.Current.Type){
            case TokenTypes.IDEN:{
                node = new Identifier(tokens.Current.Value);
                tokens.SelectNext();
                if(tokens.Current.Type != TokenTypes.EQUAL){
                    throw new RaulException($"Expected '=' after Identifier at {tokens.Position}");
                }
                tokens.SelectNext();
                node = new Assignment(node,ParseRelExpression());
                break;
            }
            case TokenTypes.KEYWORD:{
               switch(tokens.Current.Value){
                   case Keywords.ECHO:{
                       tokens.SelectNext();
                       node = new Echo(ParseRelExpression());
                       break;
                   }
                   case Keywords.WHILE:{
                       tokens.SelectNext();
                       if(tokens.Current.Type != TokenTypes.LPAR) throw new RaulException($"Expected '(' after WHILE stmnt at {tokens.Position}");
                       tokens.SelectNext();
                       Node condition = ParseRelExpression();
                       if(tokens.Current.Type != TokenTypes.RPAR) throw new RaulException($"Expected ')' after WHILE stmnt at {tokens.Position}");
                       tokens.SelectNext();
                       node = new WhileNode(condition,ParseCommand());
                       return node;
                   }
                   case Keywords.IF:{
                       tokens.SelectNext();
                       if(tokens.Current.Type != TokenTypes.LPAR) throw new RaulException($"Expected '(' after IF stmnt at {tokens.Position}");
                       tokens.SelectNext();
                       Node condition = ParseRelExpression();
                       if(tokens.Current.Type != TokenTypes.RPAR) throw new RaulException($"Expected ')' after IF stmnt at {tokens.Position}");
                       tokens.SelectNext();
                       Node cond_true = ParseCommand();
                       if(tokens.Current.Type == TokenTypes.KEYWORD && tokens.Current.Value == Keywords.ELSE){
                           tokens.SelectNext();
                           node = new IfNode(condition,cond_true, ParseCommand());
                       }else{
                           node = new IfNode(condition,cond_true);
                       }
                       return node;
                   }
                   default:{
                       throw new RaulException($"Hum... Nao era pra esse erro ser possivel...");
                   }
               }
               break;
            }
            case TokenTypes.LBRACE:{
                node = ParseBlock();
                return node;
            }
            default:{
                throw new RaulException($"Unexpected symbol at {tokens.Position}");
            }
        }
        if(tokens.Current.Type == TokenTypes.SEMI) {
            tokens.SelectNext();
            return node;
        }else{
            throw new RaulException($"Expected ';' at {tokens.Position}");
        }
    }

    static Node ParseRelExpression(){
        Node res = ParseExpression();
        Node N2 = null;
        while(tokens.CurrentIsRel()){
            string relation = tokens.Current.Type.ToString();
            tokens.SelectNext();
            N2 = ParseExpression();
            res = new BinOp(relation, res, N2);
        }
        return res;
    }
    static Node ParseFactor(){
        dynamic value;
        Node node;
        switch(tokens.Current.Type){
            case TokenTypes.INT:{
                value = tokens.Current.Value;
                tokens.SelectNext();
                return new IntVal(value);
            }
            case TokenTypes.STRING:{
                value = tokens.Current.Value;
                tokens.SelectNext();
                return new StrVal(value);
            }
            case TokenTypes.PLUS:{
                tokens.SelectNext();
                node = ParseFactor();
                return new UnOp('+', node);
            }
            case TokenTypes.MINUS:{
                tokens.SelectNext();
                node = ParseFactor();
                return new UnOp('-', node);
            }
            case TokenTypes.NOT:{
                tokens.SelectNext();
                node = ParseFactor();
                return new UnOp('!', node);
            }
            case TokenTypes.IDEN:{
                node = new Identifier(tokens.Current.Value);
                tokens.SelectNext();
                return node;
            }
            case TokenTypes.LPAR:{
                tokens.SelectNext();
                node = ParseRelExpression();
                if(tokens.Current.Type == TokenTypes.RPAR){
                    tokens.SelectNext();
                    return node;
                } 
                else throw new RaulException($"Expected ')' at pos {tokens.Position}");
            }
            case TokenTypes.KEYWORD:{
                switch(tokens.Current.Value){
                    case Keywords.READLINE:{
                        node = new ReadLineNode();
                        tokens.SelectNext();
                        if(tokens.Current.Type != TokenTypes.LPAR) throw new RaulException($"Expected '(' at pos {tokens.Position}");
                        tokens.SelectNext();
                        if(tokens.Current.Type != TokenTypes.RPAR) throw new RaulException($"Expected ')' at pos {tokens.Position}");
                        tokens.SelectNext();
                        return node;
                    }
                    case Keywords.TRUE:{
                        node = new BoolVal(true);
                        tokens.SelectNext();
                        return node;
                    }
                    case Keywords.FALSE:{
                        node = new BoolVal(false);
                        tokens.SelectNext();
                        return node;
                    } 
                    default:{
                        throw new RaulException($"Unexpected KEYWORD at pos {tokens.Position}");
                    }
                }
                
            }
            default:{
                throw new RaulException($"Unexpected symbol at {tokens.Position}");
            }
        }
    }
    static Node ParseTerm(){
        Node res = ParseFactor();
        Node N2 = null;
        while(tokens.CurrentIsTerm()){
            if(tokens.Current.Type == TokenTypes.STAR){
                tokens.SelectNext();
                N2 = ParseFactor();
                res = new BinOp('*', res, N2);
            }else if(tokens.Current.Type == TokenTypes.SLASH){
                tokens.SelectNext();
                N2 = ParseFactor();
                res = new BinOp('/', res, N2);
            }else if(tokens.Current.Type == TokenTypes.KEYWORD){
                if(tokens.Current.Value != Keywords.AND) throw new RaulException($"Unexpected KEYWORD at pos {tokens.Position}");
                tokens.SelectNext();
                N2 = ParseFactor();
                res = new BinOp("AND", res, N2);
            }
        }
        return res;
    }
    static Node ParseExpression(){
        Node res = ParseTerm();
        Node N2 = null;
        while(tokens.CurrentIsExpr()){
            if(tokens.Current.Type == TokenTypes.PLUS){
                tokens.SelectNext();
                N2 = ParseTerm();
                res = new BinOp('+', res, N2);
            }else if(tokens.Current.Type == TokenTypes.MINUS){
                tokens.SelectNext();
                N2 = ParseTerm();
                res = new BinOp('-', res, N2);
            }else if(tokens.Current.Type == TokenTypes.DOT){
                tokens.SelectNext();
                N2 = ParseTerm();
                res = new BinOp('.', res, N2);
            }else if(tokens.Current.Type == TokenTypes.KEYWORD){
                if(tokens.Current.Value != Keywords.OR) throw new RaulException($"Unexpected KEYWORD at pos {tokens.Position}");
                tokens.SelectNext();
                N2 = ParseFactor();
                res = new BinOp("OR", res, N2);
            }
        }
        return res;
    }

    public static Node Run(string code){
        tokens = new Tokenizer(code, true);
        Node root = ParseProgram();
        if(tokens.Current.Type != TokenTypes.EOF){
            throw new RaulException($"Expected EOF at pos {tokens.Position}, found {tokens.Current.Type}");
        }
        return root;
    }

}