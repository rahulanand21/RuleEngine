using System;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;

namespace RuleEngine
{
    class Program
    {
        static  void Main(string[] args)
        {
            var input_signals = GetParentSignalsAsync();
            var ruleList = new List<Rule>();
            Console.WriteLine("Enter Rules.Press Enter to Check");

            while (true)
            {
                var rule=Console.ReadLine();
                if (string.IsNullOrWhiteSpace(rule))
                    break;
                else
                {
                    var wordList = rule.Split(' ');
                    ruleList.Add(
                        new Rule
                        {
                            Signal_Name = wordList[0],
                            Operator = wordList[1],
                            Limit = wordList[2]
                        }
                        );
                }
            }
            foreach (var s in input_signals)
            {
                var ruleListForSignal = ruleList.Where(r => r.Signal_Name == s.signal);
                Check(ruleListForSignal, s);
            }
        }

        static ParentSignal[] GetParentSignalsAsync()
        {
            return JsonConvert.DeserializeObject<ParentSignal[]>(File.ReadAllText("raw_data.json"));
        }

        private static bool ComputeCondition(string value,string optr,string limit, string value_type)
        {
            bool returnResult = false;
            try
            {
                switch (optr)
                {
                    case "<":
                        if (value_type == "String")
                            returnResult = false;
                        else

                        {
                            return
                                value_type == "Integer" ?

                                Convert.ToInt32(value) < Convert.ToInt32(limit) :
                                DateTime.Parse(value) < DateTime.Parse(limit)
                                ;
                        }
                        break;

                    case ">":
                        if (value_type == "String")
                            returnResult = false;
                        else
                        {
                            return
                                value_type == "Integer" ?

                                Convert.ToInt32(value) > Convert.ToInt32(limit) :
                                DateTime.Parse(value) > DateTime.Parse(limit)
                                ;
                        }
                        break;

                    case "!=":
                        returnResult = value != limit;
                        break;

                    case "==":
                        returnResult = value == limit;
                        break;

                }
            }
            catch (Exception ex)
            {
                returnResult = false;
            }

            return returnResult;
        }

        static bool Check(IEnumerable<Rule> ruleList,ParentSignal s)
        {
            bool returnResult = true;

            foreach (var rule in ruleList)
            {
                //Operand Operator Operand format
                //-->Operators : <,>,<=,>=,==                   ALT1< 240
                

                if(!ComputeCondition(s.value,rule.Operator,rule.Limit,s.value_type))
                {
                    Console.WriteLine("Parent Signal Name :" + s.signal + "\tValue:" + s.value + "\tType:" + s.value_type);
                    Console.WriteLine("Failed Rule :" + rule.Signal_Name + rule.Operator + rule.Limit);
                    Console.WriteLine("\n");
                    returnResult = false;
                }
                Console.WriteLine("\n");

            }
            return returnResult;
        }
    }
}
