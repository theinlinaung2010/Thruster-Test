using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Thruster_Test
{
    class CommandParser
    {
        string inputString = "";
        int currentLineNumber = 0;
        int errorLineNumber = 0;
        int currentCmdIndex = 0;
        List<string> commands;
        List<List<int>> parameters;
        string[] validCommands = { "lf", "lr", "fw", "rv", "st", "wt", "tr" };
        int[] minParas = { 3, 3, 1, 1, 0, 1, 0};
        int[] maxParas = { 3, 3, 2, 2, 1, 1, 0};
        
        public string InputString
        {
            get { return inputString; }
            set { inputString = value; }
        }

        public bool ParseInputString()
        {
            currentLineNumber = 0;
            errorLineNumber = 0;
            currentCmdIndex = 0;
            commands = new List<string>();
            parameters = new List<List<int>>();

            inputString = inputString.ToLower();
            if (inputString != "")
            {
                string[] lines = inputString.Split('\n');
                lines = lines.Where(x => !string.IsNullOrEmpty(x)).ToArray(); //eliminate null strings
                foreach (string line in lines)
                {
                    currentLineNumber++;
                    char[] chars = line.ToCharArray();
                    //Find the first index of a number to find the command text
                    int index = 0;
                    if (chars.Count() > 2)
                    {
                        while (Char.IsNumber(line, index) == false && index < chars.Length - 1)
                        {
                            index++;
                        }
                    }
                    else
                    {
                        index = 2;
                    }
                    if (index == 0)
                    {
                        errorLineNumber = currentLineNumber;
                        return false;
                    }
                    string command = line.Substring(0, index);
                    command = command.Replace(" ", "");
                    if (validCommands.Contains<string>(command))
                    {
                        commands.Add(command);
                    }
                    else
                    {
                        errorLineNumber = currentLineNumber;
                        return false;
                    }

                    List<int> intParas = new List<int>();
                    string remainingString = line.Substring(index);
                    Regex regex = new Regex(@"\s");
                    string[] stringParas = regex.Split(remainingString);
                    stringParas = stringParas.Where(x => !string.IsNullOrEmpty(x)).ToArray(); 
                    foreach (string stringPara in stringParas)
                    {
                        stringPara.Replace(" ", "");
                        int intPara;
                        if (int.TryParse(stringPara, out intPara) == false)
                        {
                            errorLineNumber = currentLineNumber;
                            return false;
                        }
                        else
                        {
                            intParas.Add(intPara);
                        }
                    }
                    int cmdIndex = -1;
                    for (int i = 0; i < validCommands.Length; i++)
                    {
                        if (validCommands[i] == command)
                        {
                            cmdIndex = i;
                            break;
                        }
                    }
                    
                    if (cmdIndex > -1 && intParas.Count >= minParas[cmdIndex] && intParas.Count <= maxParas[cmdIndex])
                    {
                        parameters.Add(intParas);
                    }
                    else
                    {
                        errorLineNumber = currentLineNumber;
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool NextCommand(out string cmd, out List<int>paras)
        {
            if (currentCmdIndex <= commands.Count - 1)
            {
                cmd = commands[currentCmdIndex];
                paras = parameters[currentCmdIndex];
                currentCmdIndex++;
                return true;
            }
            else
            {
                cmd = null;
                paras = null;
                return false;
            }
        }

        public int GetErrorLineNumber()
        {
            //-1 = No command
            //0 = All correct
            return errorLineNumber;
        }
    }
}

