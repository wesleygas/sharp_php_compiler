using System;

static class Parser {
    static Tokenizer tokens;
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
                node = new Assignment(node,ParseExpression());
                break;
            }
            case TokenTypes.KEYWORD:{
               switch(tokens.Current.Value){
                   case Keywords.ECHO:{
                       tokens.SelectNext();
                       node = new Echo(ParseExpression());
                       break;
                   }default:{
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


    static Node ParseFactor(){
        int value;
        Node node;
        switch(tokens.Current.Type){
            case TokenTypes.INT:{
                value = tokens.Current.Value;
                tokens.SelectNext();
                return new IntVal(value);
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
            case TokenTypes.IDEN:{
                node = new Identifier(tokens.Current.Value);
                tokens.SelectNext();
                return node;
            }
            case TokenTypes.LPAR:{
                tokens.SelectNext();
                node = ParseExpression();
                if(tokens.Current.Type == TokenTypes.RPAR){
                    tokens.SelectNext();
                    return node;
                } 
                else throw new RaulException($"Expected ')' at pos {tokens.Position}");
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
                N2 = ParseTerm();
                res = new BinOp('*', res, N2);
            }else if(tokens.Current.Type == TokenTypes.SLASH){
                tokens.SelectNext();
                N2 = ParseTerm();
                res = new BinOp('/', res, N2);
            }
        }
        return res;
    }
    static Node ParseExpression(){
        Node res = ParseTerm();
        Node N2 = null;
        while(tokens.CurrentIsSign()){
            if(tokens.Current.Type == TokenTypes.PLUS){
                tokens.SelectNext();
                N2 = ParseTerm();
                res = new BinOp('+', res, N2);
            }else if(tokens.Current.Type == TokenTypes.MINUS){
                tokens.SelectNext();
                N2 = ParseTerm();
                res = new BinOp('-', res, N2);
            }
        }
        return res;
    }

    public static Node Run(string code){
        tokens = new Tokenizer(code, true);
        Node root = ParseBlock();
        if(tokens.Current.Type != TokenTypes.EOF){
            throw new RaulException($"Expected EOF at pos {tokens.Position}");
        }
        return root;
    }

}