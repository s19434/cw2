using Cwiczenie_2.models;
using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Cwiczenie_2
{
    public class Program
    {
        private const string DATE_FORMAT = "dd.MM.yyyy";
        private StreamWriter logPathWriter = new StreamWriter(@"..\..\..\data\log.txt");
        public Program(params string[] cases)
        {
            try
            {
                switch (cases.Length)
                {
                    case 0:
                        {
                            this.dataPackagePath = @"data\data.csv";
                            this.resultPath = @"..\..\..\data\result.xml";
                            this.type = "xml";
                            break;
                        }
                    case 1:
                        {
                            this.dataPackagePath = cases[0];
                            this.resultPath = @"..\..\..\data\result.xml";
                            this.type = "xml";
                            break;
                        }
                    case 2:
                        {
                            this.dataPackagePath = cases[0];
                            this.resultPath = cases[1];
                            this.type = "xml";
                            break;
                        }
                    case 3:
                        {
                            this.dataPackagePath = cases[0];
                            this.resultPath = cases[1];
                            this.type = cases[2];
                            break;
                        }
                    default:
                        throw new ArgumentException("Nieprawidlowa liczba argumentów");
                }
                if (!this.resultPath.Contains(this.type)) throw new InvalidOperationException("Nieprawidłowy typ pliku wyników");
                this.Init();
            }
            catch (Exception exc)
            {
                this.logPathWriter.WriteLine(exc.Message);
            }
        }


        public static void Main(string[] args)
        {
            args = new string[3];                     
            args[0] = @"data\dane.csv";               
            args[1] = @"..\..\..\data\result.json";   
            args[2] = "json";                         

            new Program(args);
        }

        
        private string dataPackagePath;
        private string resultPath;
        private string type;


        public void Init()
        {
            Category ctg = new Category
            {
                Date = DateTime.Today.ToString(Program.DATE_FORMAT),
                Author = "Oleksandr Rudenko"
            };
            FileStream fs = new FileStream(this.resultPath, FileMode.Create);
            Dictionary<string, int> count = new Dictionary<string, int>();

            //xml
            XmlSerializer xs = new XmlSerializer(ctg.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlWriterSettings xws = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            //json
            JsonSerializerOptions jso = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            using (StreamReader sr = new StreamReader(new FileInfo(this.dataPackagePath).OpenRead()))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] sLine = line.Split(',');
                    try
                    {
                        if (sLine.Length != 9 || sLine.Any(s => s.Equals("")) || sLine.Any(s => s.Equals(" "))) 
                            throw new InvalidOperationException("Niepoprawne dane, nie można dodać nowego ucznia: " + line);

                        if (ctg.students.Where(s => s.Id == "s" + sLine[4] && s.Name == sLine[0] && s.Surname == sLine[1]).Count() != 0) 
                            throw new InvalidOperationException("Powtarzające się dane występują, nie można dodać nowego ucznia: " + line);
                    }
                    catch (Exception exc)
                    {
                        this.logPathWriter.WriteLine(exc.Message);

                        continue;
                    }
                    if (sLine[2].StartsWith("Informatyka")) sLine[2] = "Computer Science";
                    else if (sLine[2].StartsWith("Sztuka Nowych Mediów")) sLine[2] = "New Media Art";
                    else if (sLine[2].StartsWith("Zarządzanie informacją")) sLine[2] = "Information Management";
                    else if (sLine[2].StartsWith("MBA dla branży IT")) sLine[2] = "MBA for IT";
                    Item student = new Item
                    {
                        Id = "s" + sLine[4],
                        Name = sLine[0],
                        Surname = sLine[1],
                        Birthdate = DateTime.Parse(sLine[5]).ToString(Program.DATE_FORMAT),
                        Email = sLine[6],
                        MothersName = sLine[7],
                        FathersName = sLine[8],
                        StudiesList = new Item.Studies
                        {
                            Name = sLine[2],
                            Type = sLine[3]
                        }
                    };
                    ctg.students.Add(student);
                    if (!count.ContainsKey(sLine[2])) count.Add(sLine[2], 1);
                    else count[sLine[2]]++;
                }
                List<string> studies = new List<string>(count.Keys);
                for (int i = 0; i < studies.Count; i++)
                {
                    Item activeStudy = new Item
                    {
                        Study = studies[i],
                        Number = count[studies[i]].ToString()
                    };
                    ctg.studies.Add(activeStudy);
                }
                //xml
                if (this.type.Equals("xml")) xs.Serialize(XmlWriter.Create(fs, xws), ctg, ns);
                //json
                else if (this.type.Equals("json"))
                {
                    fs.Close();
                    File.WriteAllText(this.resultPath, JsonSerializer.Serialize(ctg, jso));
                }
            }
            this.logPathWriter.Close();
        }
    }
}