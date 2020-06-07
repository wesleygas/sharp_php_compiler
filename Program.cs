using System;
using System.Collections.Generic;
using System.IO; 

namespace compylador
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) throw new RaulException("Você precisa passar um arquivo");
            string code = File.ReadAllText(args[0]);
            if (string.IsNullOrWhiteSpace(code)) throw new RaulException("Não pode ter nada vazio...Espaços contam como vazio, se você não entendeu.");

            code = Preproc.Run(code);

            Node root = Parser.Run(code);
            SymbolTable st = new SymbolTable();
            //string alo = root.Evaluate(st);
            //Console.WriteLine(alo);
            root.AsmEvaluate(st);
            Collector.FileDump("output.asm");
            return 0;
        }
    }
}
