using System;

static class Parser {
    static Tokenizer tokens;

    static Node ParseFactor(){
        int value;
        Node node;
        switch(tokens.Current.Type){
            case TokenTypes.INT:{
                value = tokens.Current.Value;
                tokens.selectNext();
                return new IntVal(value);
            }
            case TokenTypes.PLUS:{
                tokens.selectNext();
                node = ParseFactor();
                return new UnOp('+', node);
            }
            case TokenTypes.MINUS:{
                tokens.selectNext();
                node = ParseFactor();
                return new UnOp('-', node);
            }
            case TokenTypes.LPAR:{
                tokens.selectNext();
                node = parseExpression();
                if(tokens.Current.Type == TokenTypes.RPAR){
                    tokens.selectNext();
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
        while(tokens.currentIsTerm()){
            if(tokens.Current.Type == TokenTypes.STAR){
                tokens.selectNext();
                N2 = ParseTerm();
                res = new BinOp('*', res, N2);
            }else if(tokens.Current.Type == TokenTypes.SLASH){
                tokens.selectNext();
                N2 = ParseTerm();
                res = new BinOp('/', res, N2);
            }
        }
        return res;
    }
    static Node parseExpression(){
        Node res = ParseTerm();
        Node N2 = null;
        while(tokens.currentIsSign()){
            if(tokens.Current.Type == TokenTypes.PLUS){
                tokens.selectNext();
                N2 = ParseTerm();
                res = new BinOp('+', res, N2);
            }else if(tokens.Current.Type == TokenTypes.MINUS){
                tokens.selectNext();
                N2 = ParseTerm();
                res = new BinOp('-', res, N2);
            }
        }
        return res;
    }

    public static Node run(string code){
        tokens = new Tokenizer(code, true);
        Node root = parseExpression();
        if(tokens.Current.Type != TokenTypes.EOF){
            throw new RaulException($"Expected EOF at pos {tokens.Position}");
        }
        return root;
    }

}