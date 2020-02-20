using System;
using System.Collections.Generic;

namespace compylador
{
    class Program
    {
        public static string[] signs = new string[] { "+", "-" };
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new RaulException("Não pode ter nada vazio");
            }

            string code = args[0];
            if (string.IsNullOrWhiteSpace(code))
            { //To começando a amar C#
                throw new RaulException("Não pode ter nada vazio...Espaços contam como vazio, se você não entendeu.");
            }

            foreach (string c in signs)
            {
                if(code[0] == c[0] || code[code.Length-1] == c[0]){
                    throw new RaulException("N da pra fazer isso né, amigão. Os sinais vêm ENTRE os números.");
                }
            }

            string[] numbers = code.Replace("+"," ").Replace("-"," ").Split(" ");
            
            List<char> code_signs = new List<char>();

            foreach (char c in code)    
            {
                if(!((c == '+') || (c == '-') || char.IsNumber(c))){
                    throw new RaulException($"Só aceito números e operações '+' e '-'! '{c}' não me parece estar dentro disso");
                }
                if((c == '+')||(c == '-')){
                    code_signs.Add(c);
                }
            }
            if((numbers.Length-1) != code_signs.Count){
                throw new RaulException("N da pra fazer isso né, amigão. Os sinais vêm ENTRE os números");
            }
            int result = 0;
            for (int i = 0; i < code_signs.Count; i++)
            {
                int cNum = 0;
                if(!Int32.TryParse(numbers[i], out cNum)) throw new FormatException($"Couldn't convert {numbers[i]} to int");
                result+= cNum;
                if(!Int32.TryParse(numbers[i+1], out cNum)) throw new FormatException($"Couldn't convert {numbers[i+1]} to int");
                if(code_signs[i] == '+'){
                    result+= cNum;
                }else if(code_signs[i] == '-'){
                    result -= cNum;
                }
            }
            Console.WriteLine(result);
            return 0;
        }
    }
}
