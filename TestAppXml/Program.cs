using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using alps.net.api;
using alps.net.api.ALPS;
using alps.net.api.parsing;
using alps.net.api.StandardPASS;

namespace TestAppXml
{
    class Program
    {
        public static string folderPath = @"C:\Users\Tim\Dropbox\FH Joanneum\MAB\";

        static void Main(string[] args)
        {
            Program program = new Program();

            string[] files = Directory.GetFiles(folderPath + @"BPMN\Input", "*.bpmn");
            foreach (var file in files)
            {
                string fileName = file.Replace(folderPath + @"BPMN\Input\", "");
                fileName = fileName.Replace(".bpmn", "");
                Console.WriteLine(fileName);
                //string fileName = "Test Service";



                program.CleanFile(fileName);
                PASS.PASSElements pass = program.Bpmn2Pass(fileName);
                program.WritePASS(pass, fileName);
                BPMN.Definitions reverseBpmn = program.PASS2Bpmn(pass);
                program.WriteBPMN(reverseBpmn, fileName);

                // Compare Input and Reverse
                program.CompareModels(fileName);
            }
            
        }

        public void CleanFile (string fileName)
        {
            // File einlesen
            string rawFile = File.ReadAllText(folderPath + @"BPMN\Input\" + fileName + ".bpmn");

            // file reinigen
            string cleanFile = RemoveTags(rawFile, "<extensionElements>", "</extensionElements>");
            cleanFile = cleanFile.Replace("<extensionElements/>", "");
            cleanFile = cleanFile.Replace("xsi:type=\"tFormalExpression\"", "");
            cleanFile = cleanFile.Replace("&#10;", " ");
            XDocument xdoc = XDocument.Parse(cleanFile);

            // file speichern
            xdoc.Save(folderPath + @"BPMN\Clean\" + fileName + ".bpmn");
        }

        public PASS.PASSElements Bpmn2Pass(string fileName)
        {
            // clean file deserializen
            XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(BPMN.Definitions));
            System.IO.StreamReader file = new System.IO.StreamReader(folderPath + @"BPMN\Clean\" + fileName + ".bpmn");
            BPMN.Definitions bpmn = (BPMN.Definitions)reader.Deserialize(file);
            file.Close();

            // bpmn File Umwandeln
            SimpleBPMN.SimpleBPMN simpleBPMN = new BPMNTransformation().BPMN2SimpleBPMN(bpmn);
            PASS.PASSElements pass = new SimpleBPMNTransformation().SimpleBPMN2PASS(simpleBPMN);

            return pass;
        }

        public BPMN.Definitions PASS2Bpmn(PASS.PASSElements pass)
        {
            // PASS Umwandeln
            SimpleBPMN.SimpleBPMN simpleBPMN = new PASSTransformation().PASS2SimpleBPMN(pass);
            BPMN.Definitions bpmn = new SimpleBPMNTransformation().SimpleBPMN2BPMN(simpleBPMN);

            return bpmn;
        }


        public void WritePASS(PASS.PASSElements pass, string fileName)
        {
            IPASSReaderWriter io = PASSReaderWriter.getInstance();

            // Create Model
            IPASSProcessModel model = new PASSTransformation().PASS2ProcessModel(pass);

            // Export Model
            io.exportModel(model, folderPath + "/PASS/Output/" + fileName + ".owl");
        }

        public void WriteBPMN(BPMN.Definitions bpmn, string fileName)
        {
            var serializer = new XmlSerializer(typeof(BPMN.Definitions));
            var wfile = new System.IO.StreamWriter(folderPath + @"BPMN\Reverse\" + fileName + ".bpmn");
            serializer.Serialize(wfile, bpmn);
            wfile.Close();
        }

        public void CompareModels(string fileName)
        {
            XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(BPMN.Definitions));

            // clean file deserializen
            System.IO.StreamReader fileClean = new System.IO.StreamReader(folderPath + @"BPMN\Clean\" + fileName + ".bpmn");
            BPMN.Definitions bpmnClean = (BPMN.Definitions)reader.Deserialize(fileClean);
            fileClean.Close();

            // Reverse File deserialize
            System.IO.StreamReader fileReverse = new System.IO.StreamReader(folderPath + @"BPMN\Reverse\" + fileName + ".bpmn");
            BPMN.Definitions bpmnReverse = (BPMN.Definitions)reader.Deserialize(fileReverse);
            fileReverse.Close();

            bool isTheSame = bpmnClean.IsTheSame(bpmnReverse);

            Console.WriteLine(isTheSame);
        }

        public String RemoveTags(string text, string startTag, string endTag)
        {
            if (text.Contains(startTag))
            {
                // Find the position of the start and the end tag
                int foundS1 = text.IndexOf(startTag);
                int foundS2 = text.IndexOf(endTag);

                //RemoveTags everything between start and end tag
                text = text.Remove(foundS1, foundS2 + endTag.Length - foundS1);
                
                // call the function again (recursion)
                return RemoveTags(text, startTag, endTag);
            }
            else
            {
                //end of recursion
                return text;
            }
        }

        
        /*
        public void ReadOWL()//PASS.PASSElements simpleSBPM)
        {
            // import
            IPASSReaderWriter io = PASSReaderWriter.getInstance();

            // Prepare the paths to the structure-defining owl files
            IList<string> paths = new List<string>
            {
                "C:/Users/wds/Dropbox/FH Joanneum/MAB/standard_PASS_ont_v_1.1.0.owl",
                "C:/Users/wds/Dropbox/FH Joanneum/MAB/abstract-layered-pass-ont.owl",
            };
            // Load these files once (no future calls neded)
            io.loadOWLParsingStructure(paths);

            IList<IPASSProcessModel> models = io.loadModels(new List<string> { "C:/Users/wds/Dropbox/FH Joanneum/MAB/PASS/Alle Prozesselemente.owl" });
            IPASSProcessModel modelImport = models[0];

            IModelLayer test1 = modelImport.getBaseLayer();

            IFullySpecifiedSubject test2 = test1.getFullySpecifiedSubject(0);

            //IList<IPASSProcessModel> models = io.loadModels(new List<string> { "C:/Users/Tim/Dropbox/FH Joanneum/MAB/Visio/Drawing1.owl" });

            // Create Model
            //IPASSProcessModel model = new PASSTransformation().PASS2ProcessModel(simpleSBPM);

            io.exportModel(modelImport, "C:/Users/wds/Dropbox/FH Joanneum/MAB/PASS/Alle Prozesselemente.owl");

            //Console.WriteLine(io.exportModel(model, "C:/Users/wds/Dropbox/FH Joanneum/MAB/S-BPM/Output/Drawing3.owl"));
            //Console.WriteLine(io.exportModel(modelImport, "C:/Users/wds/Dropbox/FH Joanneum/MAB/S-BPM/Output/Drawing4.owl"));
        }
        */
    }
}
